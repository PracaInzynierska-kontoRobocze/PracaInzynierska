using System.Collections.Generic;


    public class DeleteQuery
    {
        public string TableName { get; set; }
        public List<Condition> Conditions { get; set; } = new();
    }
