using Microsoft.AspNetCore.Mvc;
using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public interface IContactsRepository
    {
        public Task<IEnumerable<Contact>>  GetAllContacts(int? page);
        public Task<Contact> GetContactsById(Guid id);
        public Task<Contact> CreateContact(ContactsDTO contacts);
        public Task<Contact> UpdateContact(ContactsDTO contacts,Guid id);
        public Task<Contact> DeleteContact(Guid id);
        public Task<IEnumerable<Subject>> GetAllSubject();
    }
}
