using System.Collections.Generic;


public partial class StatekZwiadowczy : Przeciwnik
{
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
            { "System", ("'Radar'", false, 1) },
            { "Status", ("'Online'", false, 1) },
            { "Uwagi", ("'Kluczowe'", false, 99) },
            { "Edycja", (true, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(nazwaTabeli).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "System", ("'Załoga'", false, 1) },
            { "Status", (12, false, 1) }, 
            { "Uwagi", ("'Nieedytowane'", false, 99) },
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });
    }

    public bool CzyRadarDziala()
    {
        var tabela = Tabela(Nazwa);
        foreach (var row in tabela.Rows)
        {
            if (row.Cells.TryGetValue("System", out var sysCell) && sysCell.Value?.ToString() == "'Radar'")
            {
                if (row.Cells.TryGetValue("Status", out var statCell))
                {
                    return statCell.Value?.ToString() == "'Online'";
                }
            }
        }
        return false;
    }

    public bool SprobujZablokowac(string nazwaTabeliCel, string kolumna, string nowaWartosc)
    {
        if (!CzyRadarDziala())
            return false;

        bool czyWylaczaRadar = (nazwaTabeliCel == Nazwa) &&
                               (kolumna == "Status") &&
                               (nowaWartosc != "'Online'");

        if (czyWylaczaRadar)
            return false; 

        return true;
    }

    public override string WarunkiObiketuUpdate(UpdateQuery query)
    {
        if(SprobujZablokowac(query.TableName,query.ColumnToUpdate,query.NewValue))
        {
            return "Zwiadowczy zablokował próbę wykonania zapytania! Uważaj, statek obronny został powiadomiony i wystrzeli o turę szybciej!";
        }
        return base.WarunkiObiketuUpdate(query);
    }
}