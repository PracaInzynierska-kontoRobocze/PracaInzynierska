using Godot;
using System;
using System.Linq;

public partial class Orapericus1_4 : LokalizacjaWalka
{
    private StatekZwiadowczy _zwiadowca;
    private StatekObronny _obronca;
    private Stacja _stacja;

    public override void _Ready()
    {
        base._Ready();
        Teksty.Wyswietl("uwazaj_statki", InfoPanel.TypPanelu.Podpowiedz);
        _zwiadowca = GetChildren().OfType<StatekZwiadowczy>().FirstOrDefault();
        _obronca = GetChildren().OfType<StatekObronny>().FirstOrDefault();
        _stacja = GetChildren().OfType<Stacja>().FirstOrDefault();
        _stacja._zwiadowca = _zwiadowca;
        _obronca._zwiadowca = _zwiadowca;
    }

    public override string SprawdzWarunki() 
    {
        if (CzyBarieraZneutralizowana())
        {
            WynikPozytywny("Ora1_4_sukces");
            return "Sukces!"; 
        }
        if(_obronca._turyDoStrzalu <= 0)
        {
            WynikNegatywny("Ora1_4_porazka");
            return "Przegrana!";
        }


        return string.Empty;
    }

    private bool CzyBarieraZneutralizowana()
    {
        bool drogaA = _stacja != null && _stacja.CzyZasilanieBarieryOdciete();

        bool drogaB = _obronca != null && _obronca.CzyBarieraWylaczona();

        return drogaA || drogaB;
    }

    public override void TuraPrzeciwika()
    {
        base.TuraPrzeciwika();
        _obronca.OdliczTure();
    }

}