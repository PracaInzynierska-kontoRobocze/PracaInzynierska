using Godot;
using System;

public partial class Orapericus1_2 : LokalizacjaWalka
{
    public override void _Ready()
    {
        base._Ready();
        Teksty.Wyswietl("dwa_obiekty", InfoPanel.TypPanelu.Jednorazowy);
    }
    public override string SprawdzWarunki()
    {
       if(!ZostaliPrzeciwicy())
        {
            WynikPozytywny("Ora1_2_sukces");
        }
        return string.Empty;
    }

    
}
