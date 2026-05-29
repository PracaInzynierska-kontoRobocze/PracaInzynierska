using Godot;
using System.Collections.Generic;
using System.Collections.Frozen;
using static InfoPanel;

public static class Teksty
{
    private record Dane(string Tekst);
    
    private static readonly FrozenDictionary<string, string> Mapa = new Dictionary<string, string>
    {
        // Informacje
        ["pierwszy_select"] = "Aby przejąć kontrolę nad wrogimi systemami, musimy najpierw sprawdzić, z czym mamy do czynienia. Polecenie SELECT pozwala na odczytanie zawartości baz danych jednostek wroga. Twoim obecnym celem jest przeskanowanie tabeli i zlokalizowanie parametrów kontrolnych obiektu.",

        ["start_gry"] = "Witaj w grze Querynauci. Przed nami długa misja. Na mapie wybierz galaktykę 'Orapericus', a następnie wyznacz kurs na pierwszy układ oznaczony kolorem żółtym(który oznacza dostępny, czerwone oznaczają zablokowane, pomarańczowe w trakcie, a zielone ukończone). W razie  problemów z obsługą interfejsu lub tworzenia zapytań, instrukcje i treści edukacyjne znajdziesz w menu [przycisk ESC] -> Pomoc. Pierwszy system obronny jest słabo zabezpieczony przez przeciwników, więc bez pośpiechu – opanuj interfejs i wykonywanie zapytań. Na razie nic Ci nie grozi",

        ["dwa_obiekty"] = "W tym sektorze operuje więcej niż jedna jednostka. Każda z nich posiada własną, niezależną tabelę z unikalnym przedrostkiem identyfikującym konkretny model. Zapytania ogólne zostaną odrzucone – musisz precyzyjnie adresować tabele konkretnych obiektów.",

        ["coś_nie_tak_uszkodzona"] = "Sygnał tej sondy jest skrajnie niestabilny. Wygląda na to, że jednostka doznała poważnych uszkodzeń fizycznych, co doprowadziło do częściowej korupcji bazy danych i osłabienia komunikacji z bazą. Odczyt struktur może zwracać anomalie.",

        ["konkretna_kolumna"] = "Pobieranie całego zestawu danych generuje zbyt duży szum sieciowy na osłabionym łączu i powoduje anomalie w zwracanych danych. Zawęź swoje zapytanie bezpośrednio do konkretnej kolumny, która przechowuje poszukiwane przez nas informacje.",

        ["kontretna_komórka"] = "Wygląda na to, że komórka przechowująca poprawny kod dezaktywacyjny jest szczególnie problematyczna. Aby zapytanie przeszło pomyślnie, spróbuj precyzyjnie wycelować w pojedynczą, konkretną komórkę za pomocą dokładnego warunku WHERE w zapytaniu. Jeśli zapytanie zwróci tylko jedną komórkę, to obciążenie powinno być na tyle małe, że kod wyświetli się poprawnie",


        //Opisy
        ["sonda_dezaktywacja_ok"] = "Udało sie dezaktywować sondę. Odleciała ona bezwładnie w losowym kierunku.",
        ["sonda_dezaktywacja_er"] = "Wpisano nieprawidłowy kod dezaktywacyjny. Sonda nadal jest aktywna i czujna.",
        ["uwaga_strzal_2tury"] = "Wygląda na to, że Statek Obronny jest gotowy do strzału. Odda go za 2 tury, więc nie zostało Ci wiele czasu. Od Twojego następnego zapytania zależą losy tej potyczki.",

        //Poziomy
        ["Ora1_1_sukces"] = "Udało Ci się wpisać poprawny kod! Sonda została dezaktywowana i już nie stanowi dla Ciebie problemu. Możesz lecieć dalej.",
        ["Ora1_2_sukces"] = "Udało Ci się pozbyć nieznośnych sond! Możesz lecieć dalej.",
        ["Ora1_3_sukces"] = "Mimo trudności tą sondę również udało się pokonać. Leć dalej, trzymam kciuki, że nie napotkasz już na drodze podobnych komplikacji.",
        ["Ora1_4_sukces"] = "Bariera została wyłączona. Teraz już nic nie powinno stać na Twojej drodze do planety.",
        ["Ora1_4_porazka"] = "Przegrana! Potrzebowałeś zbyt wiele tur i statek obronny zdążył zniszczyć Cię swoim strzałem.",

        //Podpowiedzi
        ["uwazaj_statki"] = "Uwaga! Statek zwiadowczy aktywnie monitoruje lokalny ruch sieciowy. Jeśli wykryje nasze próby ingerencji w systemy jednostki obronnej lub stacji, natychmiast zablokuje zapytanie i poinformuje resztę floty – a to drastycznie przyspieszy ładowanie dział orbitalnych przez statek obronny! Bezpieczniej będzie wyeliminować radar zwiadowcy na samym początku.\nAby utorować sobie drogę do planety, musimy w jakiś sposób unieszkodliwić barierę energetyczną statku obronnego. Z analizy wynika, że jej generator pobiera moc bezpośrednio z pobliskiej stacji."
    }.ToFrozenDictionary();

    public static string Pobierz(string nazwa) =>
        Mapa.GetValueOrDefault(nazwa, $"ERROR: Błąd wczytywania tekstu (nie znaleziono tekstu {nazwa})");

    public static bool CzyUzyty(string nazwa)
    {
        return DB.Instance.CzyTekstBylPokazany(nazwa);
    }
    public static void OznaczJakoPokazane(string nazwa)
    {
        DB.Instance.ZapiszPokazanyTekst(nazwa);
    }

    public static void Wyswietl(string klucz, InfoPanel.TypPanelu typ)
    {
        if (typ == InfoPanel.TypPanelu.Podpowiedz)
        {
            if (Ustawienia.Instance.podpowiedzi) { return; }
        }
        if (typ == TypPanelu.Jednorazowy) { if (Teksty.CzyUzyty(klucz)) { return; } else { OznaczJakoPokazane(klucz); } }

        var tree = (SceneTree)Engine.GetMainLoop();
        if (tree == null) return;

        var infoScene = GD.Load<PackedScene>("res://sceny/elementy/InfoPanel/InfoPanel.tscn");
        if (infoScene == null) return;

        InfoPanel panel = infoScene.Instantiate<InfoPanel>();

        tree.Root.AddChild(panel);

        panel.PokazWiadomosc(Pobierz(klucz), typ);
        
    }
}