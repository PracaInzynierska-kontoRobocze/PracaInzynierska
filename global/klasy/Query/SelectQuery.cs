using System.Collections.Generic;


    public class SelectQuery
    {
        public string TableName { get; set; }
        public List<string> SelectedColumns { get; set; } = new();
        public List<Condition> Conditions { get; set; } = new();
    }

