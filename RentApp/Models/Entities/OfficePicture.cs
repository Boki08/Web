using Newtonsoft.Json;
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
        public int OfficePictureId { get; set; }
        [Required]
        [ForeignKey("Office")]
        public int OfficeId { get; set; }
        [Required]
        public string Data { get; set; }
        [JsonIgnore]
        public virtual Office Office { get; set; }
    }
}