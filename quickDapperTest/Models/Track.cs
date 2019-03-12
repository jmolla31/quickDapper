using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool DirtTrack { get; set; }
        [ServerField]
        public Guid TrackId { get; set; }
    }
}
