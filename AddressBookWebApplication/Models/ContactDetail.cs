using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBook.Api.Models
{


    public class ContactDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ContactDetailId { get; set; }
        [Required]
        public int ContactId { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required]
        public int ContactTypeId { get; set; }
    }

}
