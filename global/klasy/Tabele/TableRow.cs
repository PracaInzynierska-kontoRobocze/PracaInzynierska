using System.Collections.Generic;


    public class TableRow
    {
        public Dictionary<string, TableCell> Cells { get; set; } = new();

        public void AddCell(string columnName, TableCell cell)
        {
            Cells[columnName] = cell;
        }
    }

