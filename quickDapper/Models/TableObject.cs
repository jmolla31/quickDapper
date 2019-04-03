using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapper
{
    public class TableObject
    {
        public Type Type { get; set; }
        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public string QueryString { get; set; }
        public string InsertString { get; set; }
        public string UpdateString { get; set; }
    }
}
