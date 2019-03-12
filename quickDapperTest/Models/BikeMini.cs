using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    [PartialModel(typeof(Bike))]
    public class BikeMini
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
    }
}
