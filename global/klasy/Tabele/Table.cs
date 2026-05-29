using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


    public class Table
    {
        public string Name { get; set; }
        public List<TableColumn> Columns { get; set; } = new();
        public List<TableRow> Rows { get; set; } = new();
        public ObiektTabelowy ObiektMaster { get; set; }


    public void DodajKolumne(string nazwa, bool widoczna, Type typ)
    {
        var column = new TableColumn(nazwa, widoczna, typ);
        Columns.Add(column);
    }

    public TableColumn Kolumna(string nazwa)
        {
            var kolumna = Columns.FirstOrDefault(c => c.Name == nazwa);
            if (kolumna == null)
            {
                Global.ThrowError($"[CRITICAL] Nie ma takiej kolumny jak {nazwa}. Sprawdź czy poprawnie tworzysz obiekt.");
                return null;
            }
            else return kolumna;
        }

    public void DodajWiersz(Dictionary<string, (object Wartosc, bool Widoczna, int Dostepnosc)> wartosci)
    {
        var row = new TableRow();

        foreach (var kolumna in Columns)
        {
            if (wartosci.TryGetValue(kolumna.Name, out var daneKomorki))
            {
                var cell = new TableCell(daneKomorki.Wartosc, daneKomorki.Widoczna, daneKomorki.Dostepnosc);
                row.Cells[kolumna.Name] = cell;
            }
            else
            {
                Global.ThrowError($"[CRITICAL] Brak kolumny {kolumna.Name} przy dodawaniu wiersza do tabeli . Sprawdź czy poprawnie tworzysz obiekt.");
            }
        }

        Rows.Add(row);
    }


    public virtual string ExecuteSelect(SelectQuery query, int poziomSelect)
        {
            int licznik = 0;

            List<string> columnsToShow;
            if (
            query.SelectedColumns.Count == 0 ||
            (query.SelectedColumns.Count == 1 && query.SelectedColumns[0] == "*")
)
            {
                columnsToShow = Columns.Select(c => c.Name).ToList();
            }
            else
            {
                columnsToShow = query.SelectedColumns;
            }

            foreach (var row in Rows)
            {
                bool rowMatches = true;
                bool first = true;

                foreach (var condition in query.Conditions)
                {
                    bool result = EvaluateCondition(condition, row);

                    if (row.Cells.TryGetValue(condition.Column, out var cell))
                    {
                        var cellValue = cell.Value?.ToString() ?? "";

                        switch (condition.Operator)
                        {
                            case "=":
                                result = cellValue == condition.Value;
                                break;
                            case "!=":
                                result = cellValue != condition.Value;
                                break;
                            case ">":
                                if (double.TryParse(cellValue, out var num1) &&
                                    double.TryParse(condition.Value, out var num2))
                                    result = num1 > num2;
                                break;
                            case "<":
                                if (double.TryParse(cellValue, out var num3) &&
                                    double.TryParse(condition.Value, out var num4))
                                    result = num3 < num4;
                                break;
                            case "LIKE":
                                result = cellValue.Contains(condition.Value);
                                break;
                        }
                    }

                    if (first)
                    {
                        rowMatches = result;
                        first = false;
                    }
                    else
                    {
                        if (condition.LogicOperator == "AND")
                            rowMatches = rowMatches && result;
                        else if (condition.LogicOperator == "OR")
                            rowMatches = rowMatches || result;
                    }
                }

                if (rowMatches)
                {
                    foreach (var colName in columnsToShow)
                    {
                        if (row.Cells.ContainsKey(colName))
                        {
                            var cell = row.Cells[colName];

                            if (cell.Dostepnosc <= poziomSelect)
                            {
                                cell.Visible = true;
                                licznik++;
                            }
                        }
                    }
                }
                
            }
            return $"Odkryto {licznik} pasujących komórek w tabeli {Name}.";
        }

    public string ExecuteUpdate(UpdateQuery query,ObiektTabelowy obiekt)
        {
            int licznik = 0;


            var column = Columns.FirstOrDefault(c => c.Name == query.ColumnToUpdate);
            if (column == null)
            {
                
                return $"Brak kolumny {query.ColumnToUpdate}"; 
            }

            object convertedValue = null;
            try
            {
                convertedValue = Convert.ChangeType(query.NewValue, column.Typ);
            }
            catch
            {
                return $"[UPDATE] Nie można przekonwertować '{query.NewValue}' na typ {column.Typ.Name}.";
                
            }

            foreach (var row in Rows)
            {
                bool rowMatches = true;
                bool first = true;

                foreach (var condition in query.Conditions)
                {
                    bool result = EvaluateCondition(condition, row);

                    if (first)
                    {
                        rowMatches = result;
                        first = false;
                    }
                    else
                    {
                        if (condition.LogicOperator == "AND")
                            rowMatches = rowMatches && result;
                        else if (condition.LogicOperator == "OR")
                            rowMatches = rowMatches || result;
                    }
                }

                if (rowMatches)
                {
                    if (!row.Cells.ContainsKey("Edycja"))
                    {
                        return $"[UPDATE] Brak kolumny Edycja – nie mogę sprawdzić możliwości edycji.";
                    }

                    var editableCell = row.Cells["Edycja"];
                    if (editableCell.Value is bool ed && !ed)
                    {
                        GD.Print($"[UPDATE] Wiersz pominięty – Edycja=false");
                    }
                    string warunkiObiektu= obiekt.WarunkiObiketuUpdate(query);
                    if (warunkiObiektu != string.Empty){ return warunkiObiektu; }
                    row.Cells[query.ColumnToUpdate].Value = convertedValue;
                    licznik++;
                }
                
            }

            return $"Zaktualizowano {licznik} wierszy w tabeli {Name}.";
        }

    public string ExecuteDelete(DeleteQuery query)
        {
            if (query.TableName != Name)
                return $"Brak tabeli {Name}";

            if (!Columns.Any(c => c.Name == "Kasowanie"))
            {
                GD.PrintErr($"Tabela {Name} nie ma kolumny 'Kasowanie'");
                return $"Tabela {Name} nie ma kolumny 'Kasowanie'";
            }

            var rowsToDelete = new List<TableRow>();

            foreach (var row in Rows)
            {
                bool rowMatches = true;
                bool first = true;

                foreach (var condition in query.Conditions)
                {
                    bool result = EvaluateCondition(condition, row);

                    if (first)
                    {
                        rowMatches = result;
                        first = false;
                    }
                    else
                    {
                        if (condition.LogicOperator == "AND")
                            rowMatches &= result;
                        else if (condition.LogicOperator == "OR")
                            rowMatches |= result;
                    }
                }

                if (!rowMatches)
                    continue;

                if (!row.Cells.TryGetValue("Kasowanie", out var kasCell))
                    continue;

                if (kasCell.Value is bool canDelete && canDelete)
                {
                    rowsToDelete.Add(row);
                }
            }

            foreach (var row in rowsToDelete)
            { 
                Rows.Remove(row);
                
            }
            return $"Usunięto {rowsToDelete.Count} wierszy z tabeli {Name}.";
        }

    public bool EvaluateCondition(Condition cond, TableRow row)
    {
        string leftValue = cond.Column;
        string rightValue = cond.Value;


        if (row.Cells.TryGetValue(leftValue, out var cell))
            leftValue = cell.Value?.ToString() ?? "";
        if (row.Cells.TryGetValue(rightValue, out var cell2))
            rightValue = cell2.Value?.ToString() ?? "";


        if (double.TryParse(leftValue, out var numLeft) &&
            double.TryParse(rightValue, out var numRight))
        {
            switch (cond.Operator)
            {
                case "=": return numLeft == numRight;
                case "!=": return numLeft != numRight;
                case ">": return numLeft > numRight;
                case "<": return numLeft < numRight;
                case ">=": return numLeft >= numRight;
                case "<=": return numLeft <= numRight;
            }
        }
        else
        {

            switch (cond.Operator)
            {
                case "=": return leftValue == rightValue;
                case "!=": return leftValue != rightValue;
                case "LIKE": return leftValue.Contains(rightValue);
            }
        }

        return false;
    }


}

    

