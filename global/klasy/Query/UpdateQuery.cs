using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class UpdateQuery
    {
        public string TableName { get; set; }
        public string ColumnToUpdate { get; set; }
        public string NewValue { get; set; }

        public List<Condition> Conditions { get; set; } = new();
    }

