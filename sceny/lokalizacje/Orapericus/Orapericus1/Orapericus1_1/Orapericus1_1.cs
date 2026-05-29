using Godot;
using System;

public partial class Orapericus1_1 : LokalizacjaWalka
{
    public override void _Ready()
    {
        base._Ready();
        Teksty.Wyswietl("pierwszy_select", InfoPanel.TypPanelu.Jednorazowy);
    }
    public override string SprawdzWarunki()
    {
       if(!ZostaliPrzeciwicy())
        {
            WynikPozytywny("Ora1_1_sukces");
            return "Sukces";
        }
        return string.Empty;
    }

    
}
