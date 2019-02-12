using quickDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapperTest
{
    [TableName("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Username { get; set; }
        [ServerField]
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    [PartialModel(typeof(User))]
    public class UserCreateModel
    {
        public string Data { get; set; }
        public string Username { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Trip
    {
        [PKey]
        public int TripId { get; set; }
        public double Cost { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class TripUpdateModel
    {
        public double Cost { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
