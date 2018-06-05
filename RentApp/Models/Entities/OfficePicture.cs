using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class OfficePicture
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Office")]
        public int OfficeID { get; set; }
        public string Data { get; set; }
    }
}