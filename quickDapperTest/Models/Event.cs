using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime Date { get; set; }
        public int Track { get; set; }
    }
}
