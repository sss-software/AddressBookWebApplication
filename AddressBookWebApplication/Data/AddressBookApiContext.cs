using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AddressBook.Api.Models;

namespace AddressBook.Api.Data
{
    public class AddressBookApiContext : DbContext
    {
        public AddressBookApiContext (DbContextOptions<AddressBookApiContext> options)
            : base(options)
        {
        }

        public DbSet<AddressBook.Api.Models.Contact> Contacts { get; set; }
        public DbSet<AddressBook.Api.Models.ContactDetail> ContactDetails { get; set; }
    }
}
