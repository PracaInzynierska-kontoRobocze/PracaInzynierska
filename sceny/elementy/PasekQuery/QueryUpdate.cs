using Godot;
using System;
using System.Linq;

public partial class QueryUpdate : HBoxContainer
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
                        wynik = "\n" + table.ExecuteUpdate(query,table.ObiektMaster);
                }
            }
        }
        lokalizacja.ProgresjaTury(wynik);
    }

    public UpdateQuery BuildQuery()
    {
        var query = new UpdateQuery();

        var updateContainer = this;

        var tabela = updateContainer.GetNode<LineEdit>("tabela");
        var kolumna = updateContainer.GetNode<LineEdit>("kolumna");
        var nowa = updateContainer.GetNode<LineEdit>("nowa_wartosc");

        var warunek1 = updateContainer.GetNode<LineEdit>("warunek1");
        var op = updateContainer.GetNode<OptionButton>("operator");
        var warunek2 = updateContainer.GetNode<LineEdit>("warunek2");

        query.TableName = tabela.Text.Trim();
        query.ColumnToUpdate = kolumna.Text.Trim();
        query.NewValue = nowa.Text.Trim();

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
        _optionButton.Selected = 1;
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

    public string TableName { get; set; }
    public string ColumnToUpdate { get; set; }
    public string NewValueRaw { get; set; }

}
