using System.Linq;
using System;
using System.Collections.Generic;

public class ZabezpieczonaTabela : Table
{
    private Dictionary<TableCell, object> _backupWartosci = new();
    private Random _rng = new Random();
    private string[] _glitchChars = { "Ą", "ć", "ć", "O", "ó", "ó", "9", "!", "@", "§", "Δ", "Ψ", "░", "█" };
    public string Kod { get; set; }

    public override string ExecuteSelect(SelectQuery query, int poziomSelect)
    {
        foreach (var rowReset in Rows)
        {
            foreach (var cellReset in rowReset.Cells.Values)
            {
                cellReset.Visible = false;
            }
        }

        bool czyGwiazdka = query.SelectedColumns.Count != 1 || query.SelectedColumns[0] == "*";
        bool czyZnalezionoKod = false;

        PrzywrocOryginaly();

        var columnsToShow = (query.SelectedColumns.Count == 0 || query.SelectedColumns[0] == "*")
            ? Columns.Select(c => c.Name).ToList()
            : query.SelectedColumns;

        var pasujaceKomorki = new List<TableCell>();
        foreach (var row in Rows)
        {
            bool matches = true;
            if (query.Conditions.Count > 0)
            {
                bool first = true;
                foreach (var cond in query.Conditions)
                {
                    bool res = (cond.Column == cond.Value && cond.Operator == "=") ? true : EvaluateCondition(cond, row);
                    if (first) { matches = res; first = false; }
                    else
                    {
                        if (cond.LogicOperator == "AND") matches &= res;
                        else if (cond.LogicOperator == "OR") matches |= res;
                    }
                }
            }

            if (matches)
            {
                foreach (var colName in columnsToShow)
                {
                    if (row.Cells.TryGetValue(colName, out var cell))
                    {
                        if (cell.Dostepnosc <= poziomSelect)
                        {
                            pasujaceKomorki.Add(cell);
                        }
                    }
                }
            }
        }

        foreach (var cell in pasujaceKomorki)
        {
            bool czyToTenKod = Kod != null && cell.Value?.ToString() == Kod;
            if (czyToTenKod) czyZnalezionoKod = true;


            bool krzaczTęKomórkę = czyGwiazdka || (czyToTenKod && pasujaceKomorki.Count > 1);

            if (krzaczTęKomórkę)
            {
                if (!_backupWartosci.ContainsKey(cell))
                    _backupWartosci[cell] = cell.Value;

                string baseStr = cell.Value?.ToString() ?? "####";
                cell.Value = GenerujKrzaki(baseStr.Length);
            }

            cell.Visible = true;
        }

        if (czyGwiazdka && pasujaceKomorki.Count > 0)
        {
            Teksty.Wyswietl("konkretna_kolumna", InfoPanel.TypPanelu.Jednorazowy);
        }
        else if (czyZnalezionoKod && pasujaceKomorki.Count > 1)
        {
            Teksty.Wyswietl("kontretna_komórka", InfoPanel.TypPanelu.Jednorazowy);
        }

        return $"Odkryto {pasujaceKomorki.Count} pasujących komórek w tabeli {Name}.";
    }

    private void PrzywrocOryginaly()
    {
        foreach (var wpis in _backupWartosci)
        {
            wpis.Key.Value = wpis.Value;
        }
        _backupWartosci.Clear();
    }

    private string GenerujKrzaki(int dlugosc)
    {
        string wynik = "";
        for (int i = 0; i < dlugosc; i++)
            wynik += _glitchChars[_rng.Next(_glitchChars.Length)];
        return wynik;
    }
}