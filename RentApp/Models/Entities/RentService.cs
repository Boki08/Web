﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class RentService
    {
        [Key]
        public int RentServiceId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Logo { get; set; }
        [Required]
        public string Description { get; set; }
        public double Grade { get; set; }

        public List<Order> Orders { get; set; }
        public List<Office> Offices { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }
}