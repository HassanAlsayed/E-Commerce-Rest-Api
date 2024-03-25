using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using WebStoreApi.Data;
using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly WebStoreDbContext _db;

        public ContactsRepository(WebStoreDbContext db)
        {
            _db = db;
        }

        public async Task<Contact> CreateContact(ContactsDTO contact)
        {
            var subject = await _db.Subjects.FindAsync(contact.SubjectId);
            if (subject is null)
            {

                return null;
            }
            var ContactModel = new Contact()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone ?? "",
                Subjects = subject,
                Message = contact.Message,
                CreatedAt = DateTime.Now,
            };
            await _db.Contacts.AddAsync(ContactModel);
            await _db.SaveChangesAsync();
           
            return ContactModel;
        }

        public async Task<Contact> DeleteContact(Guid id)
        {
            var DeletedContact = await _db.Contacts.Include(x => x.Subjects).FirstOrDefaultAsync(x => x.Id == id);
            _db.Contacts.Remove(DeletedContact);
            _db.SaveChangesAsync();
            return DeletedContact;
        }

        public async Task<IEnumerable<Contact>> GetAllContacts(int? page)
        {
            if (page is null || page < 0)
            {
                page = 1;
            }
            int pageSize = 5;
            int totalPage = 0;
            double count = _db.Contacts.Count();
            totalPage = (int)Math.Ceiling(count / pageSize);
            var contacts = await _db.Contacts.Include(x => x.Subjects)
                .OrderBy(x => x.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return contacts;
        }

        public async Task<IEnumerable<Subject>> GetAllSubject()
        {
            return await _db.Subjects.ToListAsync();
        }

        public async Task<Contact> GetContactsById(Guid id)
        {
            var contact = await _db.Contacts.Include(x =>x.Subjects).SingleOrDefaultAsync(x => x.Id == id);
            if(contact is not null)
            {
                return contact;
            }
            return null;
        }

        public async Task<Contact> UpdateContact(ContactsDTO contact, Guid id)
        {
            var subject = await _db.Subjects.FindAsync(contact.SubjectId);
            if (subject is null)
            {

                throw new ArgumentException("Subject not found");
            }
            var UpdatedContact = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if(UpdatedContact is null)
            {
                throw new ArgumentException("Contact not found");
            }

            UpdatedContact.FirstName = contact.FirstName;
            UpdatedContact.LastName = contact.LastName;
            UpdatedContact.Email = contact.Email;
            UpdatedContact.Phone = contact.Phone ?? "";
            UpdatedContact.Subjects = subject;
            UpdatedContact.Message = contact.Message;
            UpdatedContact.CreatedAt = DateTime.Now;
            
             await _db.SaveChangesAsync();
             return UpdatedContact;
        }
    }
}
