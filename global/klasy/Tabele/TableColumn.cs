using System;


    public class TableColumn
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Type Typ { get; set; }

        public TableColumn(string name, bool visible = true, Type typ = null)
        {
            Name = name;
            Visible = visible;
            Typ = typ;
        }
    }

