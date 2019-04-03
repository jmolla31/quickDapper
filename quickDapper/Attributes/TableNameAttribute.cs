using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapper
{
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tName)
        {
            this.TableName = tName;
        }

        public string TableName { get; private set; }
    }
}
