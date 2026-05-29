using Godot;
using System;
using System.Linq;

public partial class QueryExec : HBoxContainer
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

        if (string.IsNullOrEmpty(query.ProcedureName))
        {
            wynik = "Nie podałeś nazwy procedury";
        }
        else
        {
            
            var obiekty = lokalizacja.GetChildren().OfType<ObiektTabelowy>().ToList();
            bool znaleziono = false;

            foreach (var obiekt in obiekty)
            {
                var procs = obiekt.Procedures?.Where(p => p.ProcedureName == query.ProcedureName).ToList();
                if (procs == null || procs.Count == 0)
                    continue;

                foreach (var proc in procs)
                {
                    if (proc.Rodzaj == RodzajProcedury.Auto)
                    {
                        wynik += $"\nObiekt {obiekt.Name}: Procedura '{query.ProcedureName}' jest automatyczna i nie może być wywołana ręcznie.";
                        continue;
                    }
                    if (proc.Parameters.Count != query.Parameters.Count)
                    {
                        wynik += $"\nObiekt {obiekt.Name}: Nieprawidłowa liczba parametrów dla procedury '{query.ProcedureName}'.";
                        continue;
                    }

                    bool typyOk = true;
                    for (int i = 0; i < proc.Parameters.Count; i++)
                    {
                        if (proc.Parameters[i].Type != query.Parameters[i].Type)
                        {
                            wynik += $"\nObiekt {obiekt.Name}: Nieprawidłowy typ parametru {i + 1} dla procedury '{query.ProcedureName}'.";
                            typyOk = false;
                            break;
                        }
                    }
                    if (!typyOk) continue;

                    wynik=obiekt.Exec(query.ProcedureName, query.Parameters);
                    if (wynik == null)
                    {
                        wynik += $"\nObiekt {obiekt.Name}: Wykonano procedurę '{query.ProcedureName}' z parametrami: {string.Join(", ", query.Parameters.Select(p => p.Value))}";
                    }
                    znaleziono = true;
                }
            }

            if (!znaleziono)
                wynik = $"Nie znaleziono procedury o nazwie {query.ProcedureName} w żadnym obiekcie.";
        }

        lokalizacja.ProgresjaTury(wynik);
    }

    public ExecQuery BuildQuery()
    {
        var query = new ExecQuery();

        var execContainer = this;
        var procedura = execContainer.GetNode<LineEdit>("procedura");
        var parametry = execContainer.GetNode<LineEdit>("parametry");

        query.ProcedureName = procedura.Text?.Trim();
        query.ParseParameters(parametry.Text);

        return query;
    }
    public void ChangeBox(int id)
    {
        (GetParent().GetParent() as QueryBox)?.SetVisible(id);
        _optionButton = GetNode<OptionButton>("OptionButton");
        _optionButton.Selected = 3;
    }
}