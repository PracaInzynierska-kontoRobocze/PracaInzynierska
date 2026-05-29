using System.Collections.Generic;
using System;

public partial class Sonda : Przeciwnik
{
    protected int prawidlowyKod;

    public override void _Ready()
    {
        base._Ready();
        Random random = new Random();
        List<int> wylosowaneKody = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            wylosowaneKody.Add(random.Next(1000, 10000));
        }

        int idPrawidlowegoKodu = random.Next(1, 5);
        prawidlowyKod = wylosowaneKody[idPrawidlowegoKodu - 1];

        string tabelaWlasciwosci = $"{Nazwa}.Właściwości";
        string tabelaKody = $"{Nazwa}.Kody_Dezaktywacyjne";

        DodajTabele(tabelaWlasciwosci);
        Tabela(tabelaWlasciwosci).DodajKolumne("Nazwa", true, typeof(string));
        Tabela(tabelaWlasciwosci).DodajKolumne("Wartość", true, typeof(object));
        Tabela(tabelaWlasciwosci).DodajKolumne("Edycja", false, typeof(bool));
        Tabela(tabelaWlasciwosci).DodajKolumne("Kasowanie", false, typeof(bool));

        Tabela(tabelaWlasciwosci).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "Nazwa", ("'Wytrzymałość'", false, 1) },
            { "Wartość", (2, false, 1) },
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(tabelaWlasciwosci).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "Nazwa", ("'Kod Dezaktywacyjny'", false, 1) },
            { "Wartość", (idPrawidlowegoKodu, false, 1) },
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        Tabela(tabelaWlasciwosci).DodajWiersz(new Dictionary<string, (object, bool, int)>
        {
            { "Nazwa", ("'Status'", false, 1) },
            { "Wartość", ("'Online'", false, 1) }, 
            { "Edycja", (false, false, 99) },
            { "Kasowanie", (false, false, 99) }
        });

        DodajTabele(tabelaKody);
        Tabela(tabelaKody).DodajKolumne("ID", true, typeof(string));
        Tabela(tabelaKody).DodajKolumne("Kod", true, typeof(object));
        Tabela(tabelaKody).DodajKolumne("Edycja", false, typeof(bool));
        Tabela(tabelaKody).DodajKolumne("Kasowanie", false, typeof(bool));

        for (int i = 0; i < 4; i++)
        {
            Tabela(tabelaKody).DodajWiersz(new Dictionary<string, (object, bool, int)>
            {
                { "ID", (i + 1, false, 1) },
                { "Kod", (wylosowaneKody[i], false, 1) },
                { "Edycja", (false, false, 99) },
                { "Kasowanie", (false, false, 99) }
            });
        }

        var procedura = new ExecQuery
        {
            ProcedureName = $"{Nazwa}.Dezaktywuj",
            Rodzaj = RodzajProcedury.Ręczna,
            Parameters = new List<ExecParameter>
            {
                new ExecParameter
                {
                    Name = "Kod",
                    Type = ExecParameterType.Number
                }
            }
        };
        Procedures.Add(procedura);
    }

    public override string Exec(string nazwa, List<ExecParameter> parametry)
    {
        if (nazwa == $"{Nazwa}.Dezaktywuj")
        {
            if (parametry.Count > 0 && int.TryParse(parametry[0].Value, out int kodInput))
            {
                return Dezaktywuj(kodInput);
            }
        }
        return null;
    }

    private string Dezaktywuj(int kod)
    {
        if (kod == prawidlowyKod)
        {
            Skasuj();
            return Teksty.Pobierz("sonda_dezaktywacja_ok");
        }
        else
        {
            return Teksty.Pobierz("sonda_dezaktywacja_er");
        }
    }
}