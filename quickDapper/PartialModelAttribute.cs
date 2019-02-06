using System;
using System.Collections.Generic;
using System.Text;

namespace quickDapper
{
    public class PartialModelAttribute : Attribute
    {
        public PartialModelAttribute(Type mainType)
        {
            this.MainType = mainType;
        }

        public Type MainType { get; }
    }
}
