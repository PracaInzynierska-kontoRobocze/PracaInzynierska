using System.Collections.Generic;


public partial class StatekObronny : Przeciwnik
{
    public StatekZwiadowczy _zwiadowca;  
    public int _turyDoStrzalu = 6;

    public override void _Ready()
    {
        base._Ready();

        string nazwaTabeli = Nazwa;

        DodajTabele(nazwaTabeli);
        Tabela(nazwaTabeli).DodajKolumne("System", true, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Status", true, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Uwagi", false, typeof(string));
        Tabela(nazwaTabeli).DodajKolumne("Edycja", false, typeof(bool));
        Tabela(nazwaTabeli).DodajKolumne("Kasowanie", false, typeof(bool));

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Napęd'", false, 1) },
            { "Status", ("'Online'", false, 1) },
            { "Uwagi", ("'Nieedytowalne'", false, 99) },
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Generator_Bariery'", false, 1) },
            { "Status", ("'Online'", false, 1) },
            { "Uwagi", ("'Kluczowe'", false, 99) },
            { "Edycja", (true, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Uzbrojenie'", false, 1) },
            { "Status", ("'Online'", false, 1) },
            { "Uwagi", ("'Nieedytowane'", false, 99) },
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        var procedura = new ExecQuery
        {
            ProcedureName = "Wystrzał",
            Rodzaj = RodzajProcedury.Auto,
            Parameters = new List<ExecParameter>
            {
                new ExecParameter
                {
                    Name = "tury_do_wystrzału",
                    Type = ExecParameterType.Number
                }
            }
        };

        Procedures.Add(procedura);

    }

    public bool CzyBarieraWylaczona()
    {
        var tabela = Tabela(Nazwa);
        foreach (var row in tabela.Rows)
        {
            if (row.Cells.TryGetValue("System", out var sysCell) && sysCell.Value?.ToString() == "'Generator_Bariery'")
            {
                if (row.Cells.TryGetValue("Status", out var statCell))
                {
                    return statCell.Value?.ToString() != "'Online'";
                }
            }
        }
        return false;
    }


    public void  OdliczTure()
    {
        _turyDoStrzalu--;

        if (_turyDoStrzalu == 2)
        {
            Teksty.Wyswietl("uwaga_strzal_2tury", InfoPanel.TypPanelu.Info);
        }

    }

    public override string WarunkiObiketuUpdate(UpdateQuery query)
    {
        if (_zwiadowca.SprobujZablokowac(query.TableName, query.ColumnToUpdate, query.NewValue))
        {
            return "Statek zwiadowczy zablokował próbę wykonania zapytania! Uważaj, statek obronny został powiadomiony i wystrzeli o turę szybciej!";
        }
        return base.WarunkiObiketuUpdate(query);
    }
}