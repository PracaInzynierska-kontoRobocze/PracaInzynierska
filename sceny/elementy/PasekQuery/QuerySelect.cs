using Godot;
using System;
using System.Linq;

public partial class QuerySelect : HBoxContainer
{
    private void _on_button_pressed()
    {
        string wynik = null;
        var query = BuildQuery();

        var lokalizacja = GetTree().Root.GetChildren()
                            .OfType<LokalizacjaQuery>()
                            .FirstOrDefault();

        if (lokalizacja == null) return;

        lokalizacja.ZamknijWszystkieTabele();

        if (string.IsNullOrEmpty(query.TableName)) { wynik = "Nie podałeś nazwy tabeli"; }
        else
        {
            var tables = lokalizacja.GetTablesByName(query.TableName);
            if (tables.Count == 0) { wynik = $"Nie istnieje tabela o nazwie {query.TableName}"; }
            else
            {
                foreach (var table in tables)
                {
                    if (table.Name == query.TableName)
                        wynik = "\n" + table.ExecuteSelect(query, Atrybuty.QueryPoziom);
                }
            }
        }
        lokalizacja.ProgresjaTury(wynik);
    }

    public SelectQuery BuildQuery()
    {
        var query = new SelectQuery();

        var kolumny = GetNode<LineEdit>("kolumny");
        var tabela = GetNode<LineEdit>("tabela");
        var warunek1 = GetNode<LineEdit>("warunek1");
        var op = GetNode<OptionButton>("operator");
        var warunek2 = GetNode<LineEdit>("warunek2");

        query.SelectedColumns = kolumny.Text
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .ToList();

        query.TableName = tabela.Text?.Trim();

        if (!string.IsNullOrEmpty(warunek1.Text))
        {
            query.Conditions.Add(new Condition
            {
                LogicOperator = "AND",
                Column = warunek1.Text,
                Operator = op.GetItemText(op.Selected),
                Value = warunek2.Text
            });
        }

        var dodatkowe = GetParent().GetNode<VBoxContainer>("Dodatkowe");

        if (dodatkowe.Visible)
        {
            foreach (var child in dodatkowe.GetChildren())
            {
                if (child is HBoxContainer hbox)
                {
                    var logic = hbox.GetNode<OptionButton>("OptionButton");
                    var col = hbox.GetNode<LineEdit>("LineEdit");
                    var oper = hbox.GetNode<OptionButton>("operator");
                    var val = hbox.GetNode<LineEdit>("LineEdit2");

                    if (!string.IsNullOrEmpty(col.Text))
                    {
                        query.Conditions.Add(new Condition
                        {
                            LogicOperator = logic.GetItemText(logic.Selected),
                            Column = col.Text,
                            Operator = oper.GetItemText(oper.Selected),
                            Value = val.Text
                        });
                    }
                }
            }
        }
        return query;
    }

    private void ViewAdditional()
    {
        var parent = GetParent();
        var dodat = parent.GetNode<VBoxContainer>("Dodatkowe");
        var przycisk = parent.GetNode<Button>("PrzyciskDodatkowe");

        if (przycisk.Text == "+")
        {
            dodat.Visible = true;
            przycisk.Text = "-";
        }
        else
        {
            dodat.Visible = false;
            przycisk.Text = "+";
        }
    }

    public void ChangeBox(int id)
    {
        (GetParent().GetParent() as QueryBox)?.SetVisible(id);
        var optBtn = GetNode<OptionButton>("OptionButton");
        optBtn.Selected = 0;
    }
}