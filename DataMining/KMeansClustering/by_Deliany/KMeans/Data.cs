using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    public class Data
    {
        public string ID { get; set; }
        public List<double> Attributes { get; set; }

        public Data()
        {
            Attributes = new List<double>();
        }
    }
}
