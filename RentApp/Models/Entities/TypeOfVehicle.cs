using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class TypeOfVehicle
    {
        [Key]
        public int TypeId { get; set; }
        [Required]
        public string Type { get; set; }
        [JsonIgnore]
        public virtual List<Vehicle> Vehicles { get; set; }

    }
}