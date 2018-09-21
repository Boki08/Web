using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public bool Activated { get; set; }
        [Required]
        public bool ProfileEdited { get; set; }
        public string DocumentPicture { get; set; }
        //public virtual List<Comment> Comments { get; set; }
        public virtual List<RentService> RentServices { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}