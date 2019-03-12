using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    [PartialModel(typeof(Bike))]
    public class BikeMini
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
