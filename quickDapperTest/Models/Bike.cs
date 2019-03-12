using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    public class Bike
    {
        [PKey]
        public int BikeNumber { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int Mileage { get; set; }
    }
}
