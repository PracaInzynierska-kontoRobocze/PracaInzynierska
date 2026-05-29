using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class TableCell
    {
        public object Value { get; set; }
        public bool Visible { get; set; } = false;
        public int Dostepnosc { get; set; } = 0;

        public TableCell(object value, bool visible = true, int dostepnosc = 0)
        {
            Value = value;
            Visible = visible;
            Dostepnosc = dostepnosc;
        }
    }

