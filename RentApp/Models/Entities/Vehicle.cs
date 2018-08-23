using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        [Required]
        [ForeignKey("RentService")]
        public int RentServiceId { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string YearOfManufacturing { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        public string Pictures { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool Available { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [JsonIgnore]
        public virtual RentService RentService { get; set; }
        [JsonIgnore]
        public virtual List<string> VehiclePictures { get; set; }
    }
}