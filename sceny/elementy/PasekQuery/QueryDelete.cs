using Godot;
using System;
using System.Linq;

public partial class QueryDelete : HBoxContainer
{
    private OptionButton _optionButton;

    private void _on_button_pressed()
    {
        string wynik = null;
        var query = BuildQuery();

        var lokalizacja = GetTree().Root.GetChildren()
            .OfType<LokalizacjaQuery>()
            .FirstOrDefault();
        lokalizacja.ZamknijWszystkieTabele();

        if (String.IsNullOrEmpty(query.TableName)) { wynik = $"Nie podałeś nazwy tabeli"; }
        else
        {
            var tables = lokalizacja.GetTablesByName(query.TableName);
            if (tables.Count == 0) { wynik = $"Nie istnieje tabela o nazwie {query.TableName}"; }
            else
            {
                foreach (var table in tables)
                {
                    if (table.Name == query.TableName)
                        wynik = "\n" + table.ExecuteDelete(query);
                }
            }
        }
        lokalizacja.ProgresjaTury(wynik);
    }

    public DeleteQuery BuildQuery()
    {
        var query = new DeleteQuery();

        var tabela = GetNode<LineEdit>("tabela");
        var warunek1 = GetNode<LineEdit>("warunek1");
        var op = GetNode<OptionButton>("operator");
        var warunek2 = GetNode<LineEdit>("warunek2");

        query.TableName = tabela.Text.Trim();

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
            foreach (HBoxContainer hbox in dodatkowe.GetChildren())
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

        return query;
    }

    public void ChangeBox(int id)
    {
        (GetParent().GetParent() as QueryBox)?.SetVisible(id);
        _optionButton = GetNode<OptionButton>("OptionButton");
        _optionButton.Selected = 2;
    }

    private void ViewAdditional()
    {
        var parent = GetParent();
        var dodat = parent.GetNode<Control>("Dodatkowe");
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
}