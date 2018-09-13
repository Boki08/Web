using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        //[ForeignKey("RentService")]
        //public int RentServiceId { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        public string Review { get; set; }
        [Required]
        public DateTime PostedDate { get; set; }
        [Required]
        public int Grade { get; set; }
       // [JsonIgnore]
        public virtual AppUser User { get; set; }
        //[JsonIgnore]
        public virtual Order Order { get; set; }
        //[JsonIgnore]
        //public virtual RentService RentService { get; set; }
    }
}