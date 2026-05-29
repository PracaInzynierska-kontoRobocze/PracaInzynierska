using Godot;
using System;

public partial class Orapericus1_3 : LokalizacjaWalka
{
    public override void _Ready()
    {
        base._Ready();
        Teksty.Wyswietl("coś_nie_tak_uszkodzona", InfoPanel.TypPanelu.Jednorazowy);
    }
    public override string SprawdzWarunki()
    {
        if(!ZostaliPrzeciwicy())
        {
            WynikPozytywny("Ora1_3_sukces");
            return "Ora1_3_sukces";
        }
        return string.Empty;
    }

    
}
