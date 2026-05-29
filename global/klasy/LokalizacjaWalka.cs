using Godot;
using System.Linq;
using static InfoPanel;
using static System.Formats.Asn1.AsnWriter;

public partial class LokalizacjaWalka : LokalizacjaQuery
{
    public int Tura { get; set; } = 1;
    public Label TuraLicznik;
    public PackedScene Wynik;
    public string WynikWarunku { get; set; }
    public int LokalizacjaId { get; set; }

    public override void _Ready()
    {
        base._Ready();
        LokalizacjaId= (int)GetMeta("Lokalizacja");
        TuraLicznik = GetNode<Label>("Tura");
        Wynik = GD.Load<PackedScene>("res://global/sceny/WynikQuery.tscn");
    }
    public override void ProgresjaTury(string wynik) 
    {
        var wynikWarunku = SprawdzWarunki();
        if (wynikWarunku != string.Empty) return;
        TuraPrzeciwika();
        wynikWarunku = SprawdzWarunki();
        if (wynikWarunku != string.Empty) return;
        ZaktualizujLicznik();
        PokazWynik(wynik);
    }
    public virtual void TuraPrzeciwika()
    {

    }
    public virtual string SprawdzWarunki()
    {
        return string.Empty;
    }
    public void ZaktualizujLicznik() 
    {
        Tura++;
        TuraLicznik.Text = "Tura: " + Tura;
    }
    public void PokazWynik(string wynik) 
    {
        var wynikInstance = Wynik.Instantiate<Wynik>();
        wynikInstance.Text = wynik;
        GetTree().Root.AddChild(wynikInstance);
    }

    public virtual void WynikPozytywny(string text)
    {
        ZaliczPoziom();
        WyswietlPanel(text, InfoPanel.TypPanelu.KoniecPoziomu);
    }

    public virtual void WynikNegatywny(string text)
    {
        PrzegranyPoziom();
        WyswietlPanel(text, InfoPanel.TypPanelu.KoniecPoziomu);
    }

    public void ZaliczPoziom() 
    {
        Lokalizacja.Instance.UpdateStatus(LokalizacjaId, 3);
        DB.Instance.Save();
    }

    public void PrzegranyPoziom()
    {
        //TODO przegrany poziom (na chwilę obecną brak konsekwencji przegranej)
    }

    public bool ZostaliPrzeciwicy()
    {
        return _obiekty.OfType<Przeciwnik>().Any();
    }


}
