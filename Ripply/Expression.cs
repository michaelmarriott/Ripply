using System;
using System.Collections.Generic;
using System.Text;

namespace Ripply
{
    public class Expression
    {
        public Expression(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

    }
}
