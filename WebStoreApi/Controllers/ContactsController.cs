using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStoreApi.Data;
using WebStoreApi.DTO;
using WebStoreApi.Repository;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsRepository _contact;

        public ContactsController(IContactsRepository contact)
        {
            _contact = contact;
        }
        [HttpGet("subject")]
        public async Task<IActionResult> GetAllSubjects()
        {
            return Ok(await _contact.GetAllSubject());
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllContacts(int? page) {
         
           return Ok(await _contact.GetAllContacts(page));
         }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactsById([FromRoute] Guid id)
        {
            return Ok(await _contact.GetContactsById(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateContacts([FromBody] ContactsDTO contacts)
        {
           var contact = await _contact.CreateContact(contacts);
            return CreatedAtAction(nameof(GetContactsById), new {id = contact.Id}, contact);
            
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContacts([FromBody] ContactsDTO contacts,[FromRoute] Guid id)
        {
            return Ok(await _contact.UpdateContact(contacts,id));
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContacts([FromRoute] Guid id)
        {
            return Ok(await _contact.DeleteContact(id));
        }
    }
}
