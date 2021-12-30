using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    class Outline
    {
        public string Name { get; set; }
        public DoubleCollection Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
