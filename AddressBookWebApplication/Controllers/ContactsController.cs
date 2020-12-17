using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressBook.Api.Data;
using AddressBook.Api.Models;
using AddressBook.Api.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace AddressBook.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AddressBookApiContext context;
        private readonly IDataRepository<Contact> contactRepository;
        private readonly IDataRepository<ContactDetail> contactDetailRepository;
        private readonly ILogger<ContactsController> logger;

        public ContactsController(AddressBookApiContext context, IDataRepository<Contact> contactRepository, IDataRepository<ContactDetail> contactDetailRepository, ILogger<ContactsController> logger)
        {
            this.context = context;
            this.contactRepository = contactRepository;
            this.contactDetailRepository = contactDetailRepository;
            this.logger = logger;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContact()
        {
            return await context.Contacts.ToListAsync();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            try
            {
                var contact = await context.Contacts.FindAsync(id);

                if (contact == null)
                {
                    return NotFound();
                }
                var detail =  await context.ContactDetails.FirstOrDefaultAsync(x => x.ContactId == id);
                contact.ContactDetail = detail;
                return contact;
            }
            catch(Exception e)
            {
                logger.LogError("Get error: " + e.Message);
                throw;
            }

        }

        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            if (id != contact.ContactId)
            {
                return BadRequest();
            }

            context.Entry(contact).State = EntityState.Modified;
            try
            {
                contactRepository.Update(contact);
                contactDetailRepository.Update(contact.ContactDetail);
                var save = await contactRepository.SaveAsync(contact);
                if (save != null)
                {
                    logger.LogInformation("Updated contact: #" + save.ContactId.ToString());
                }
                context.Entry(contact.ContactDetail).State = EntityState.Modified;
                var detail = await contactDetailRepository.SaveAsync(contact.ContactDetail);
                if (save != null)
                {
                    logger.LogInformation("Updated contact detail: #" + detail.ContactDetailId.ToString());
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    logger.LogError("Updating concurrency error: Contact does not exist.");
                    throw;
                }
            }
            catch(Exception e)
            {
                logger.LogError("Updating error:" + e.Message);
            }

            return NoContent();
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                contactRepository.Add(contact);
                var save = await contactRepository.SaveAsync(contact);
                return CreatedAtAction("GetContact", new { id = contact.ContactId }, contact);
            }
            catch(Exception e)
            {
                logger.LogError("Posting error: " + e.Message);
                throw;
            }
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var details = context.ContactDetails.Where(x => x.ContactId == id).ToList<ContactDetail>();
                if (details!= null)
                {
                    context.ContactDetails.RemoveRange(details);
                    await context.SaveChangesAsync(true);
                    logger.LogInformation("Contact details removed for: #" + id.ToString());
                }
                var contact = await context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    return NotFound();
                }

                contactRepository.Delete(contact);
                await contactRepository.SaveAsync(contact);

                return contact;
            }
            catch(Exception e)
            {
                logger.LogError("Delete error: " + e.Message);
                throw;
            }
        }

        private bool ContactExists(int id)
        {
            return context.Contacts.Any(e => e.ContactId == id);
        }
    }
}
