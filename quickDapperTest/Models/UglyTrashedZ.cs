using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    [PartialModel(typeof(Bike))]
    public class UglyTrashedZ
    {
        [PKey]
        public int BikeNumber { get; set; }
        public int Mileage { get; set; }
    }
}
