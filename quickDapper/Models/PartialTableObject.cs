using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapper
{
    public class PartialTableObject
    {
        public Type MainTable { get; set; }
        public string QueryString { get; set; }
        public string InsertString { get; set; }
        public string UpdateString { get; set; }
    }
}
