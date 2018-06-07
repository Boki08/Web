using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Pricing
    {
        [Key]
        public int PricingId { get; set; }
        [Required]
        [ForeignKey("RentService")]
        public int RentServiceId { get; set; }
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public List<Component> Components { get; set; }
        public virtual RentService RentService { get; set; }
        public virtual AppUser User { get; set; }
    }
}