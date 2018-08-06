using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class VehiclePicture
    {
        [Key]
        public int VehiclePictureId { get; set; }
        [Required]
        [ForeignKey("Vehicle")]
        public int VehicleId { get; set; }
        [Required]
        public string Data { get; set; }
        [JsonIgnore]
        public virtual Vehicle Vehicle { get; set; }
    }
}