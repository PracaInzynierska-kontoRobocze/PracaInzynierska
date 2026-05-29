using Godot;
using System.Collections.Generic;
using System.Linq;
using static InfoPanel;

public partial class LokalizacjaQuery : Node
{
    public List<ObiektTabelowy> _obiekty = [];

    public void DodajObiekt(ObiektTabelowy obiekt)
    {
        _obiekty.Add(obiekt);
    }
    public void SkasujObiekt(ObiektTabelowy obiekt)
    {
        _obiekty.Remove(obiekt);
    }
    public override void _Ready()
    {
        ZasilObiekty();
        Parametry.Instance.WybranaLokalizacja = (int)GetMeta("Lokalizacja");
    }

    public List<Table> GetAllTables()
    {
        ZasilObiekty();
        return _obiekty.SelectMany(o => o.Tables).ToList();
    }

    public List<Table> GetTablesByName(string name)
    {
        ZasilObiekty();
        return _obiekty
     .SelectMany(o => o.Tables)
     .Where(t => t.Name == name)
     .ToList();
    }

    public void ZasilObiekty() 
    {
        _obiekty.Clear();
        foreach (var obiekt in GetChildren().OfType<ObiektTabelowy>())
        {
            obiekt.ZarejestrujWLokalizacji(this);
        }
    }

    public virtual void ProgresjaTury(string wynik) { }
    public void ZamknijWszystkieTabele()
    {
        foreach (var obiekt in _obiekty)
        {
            obiekt.CloseTable();
            obiekt.isOpen = false;
        }
    }

    public override void _ExitTree()
    {
        Parametry.Instance.WybranaLokalizacja = 0;
        ZamknijWszystkieTabele();
        base._ExitTree();
    }

    public void WyswietlPanel(string tresc, InfoPanel.TypPanelu typ)
    {
        Teksty.Wyswietl(tresc, typ);
    }
}
