using System.Collections.Generic;


public partial class Stacja : Przeciwnik
{
    public StatekZwiadowczy _zwiadowca;
    public override void _Ready()
    {
        base._Ready();

        string nazwaTabeli = Nazwa;

        DodajTabele(nazwaTabeli);
        Tabela(nazwaTabeli).DodajKolumne("System", true, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Natężenie", true, typeof(int));
        Tabela(nazwaTabeli).DodajKolumne("Status", true, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Uwagi", false, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Edycja", false, typeof(bool));
        Tabela(nazwaTabeli).DodajKolumne("Kasowanie", false, typeof(bool));

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Zasilanie_Bariery'", false, 1) }, 
            { "Natężenie", (15, false, 1) },
            { "Status", ("'Online'", false, 1) },          
            { "Uwagi", ("'Kluczowe'", false, 99) },
            { "Edycja", (true, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Łączność'", false, 1) },       
            { "Natężenie", (5, false, 1) },
            { "Status", ("'Online'", false, 1) },         
            { "Uwagi", ("'Nieedytowane'", false, 99) },  
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Chłodzenie'", false, 1) },     
            { "Natężenie", (8, false, 1) },
            { "Status", ("'Online'", false, 1) },          
            { "Uwagi", ("'Nieedytowane'", false, 99) },    
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        var procedura = new ExecQuery
        {
            ProcedureName = "Zasil_Tarczę",
            Rodzaj = RodzajProcedury.Auto,
            Parameters = new List<ExecParameter>
            {
                new ExecParameter
                {
                    Name = "natężenie",
                    Type = ExecParameterType.Number
                }
            }
        };

        Procedures.Add(procedura);

    }

    public bool CzyZasilanieBarieryOdciete()
    {
        var tabela = Tabela(Nazwa);
        foreach (var row in tabela.Rows)
        {
            if (row.Cells.TryGetValue("System", out var sysCell) && sysCell.Value?.ToString() == "'Zasilanie_Bariery'")
            {
                if (row.Cells.TryGetValue("Natężenie", out var natCell))
                {
                    if (int.TryParse(natCell.Value?.ToString(), out int natezenie))
                    {
                        return natezenie == 0;
                    }
                }
            }
        }
        return false;
    }

    public override string WarunkiObiketuUpdate(UpdateQuery query)
    {
        if (_zwiadowca.SprobujZablokowac(query.TableName, query.ColumnToUpdate, query.NewValue))
        {
            return "Zwiadowczy zablokował próbę wykonania zapytania! Uważaj, statek obronny został powiadomiony i wystrzeli o turę szybciej!";
        }
        return base.WarunkiObiketuUpdate(query);
    }
}