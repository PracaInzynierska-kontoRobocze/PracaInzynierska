using Godot;
using System;
using System.Collections.Generic;

public partial class Pomoc : Control
{
    private TabBar _zakladki;
    private Control _kontener;
    private List<RichTextLabel> _opisy = new List<RichTextLabel>();

    public override void _Ready()
    {
        _zakladki = GetNode<TabBar>("Panel/Zakladki");
        _kontener = GetNode<Control>("Panel/Zawartosc");

        foreach (var child in _kontener.GetChildren())
        {
            if (child is RichTextLabel opis)
            {
                _opisy.Add(opis);
            }
        }

        UstawZakladke(0);
    }

    private void OnZakladkaZmieniona(long tab)
    {
        UstawZakladke((int)tab);
    }

    private void OnLewoPressed()
    {
        int nowa = _zakladki.CurrentTab - 1;
        if (nowa < 0) nowa = _zakladki.TabCount - 1;
        UstawZakladke(nowa);
    }

    private void OnPrawoPressed()
    {
        int nowa = _zakladki.CurrentTab + 1;
        if (nowa >= _zakladki.TabCount) nowa = 0;
        UstawZakladke(nowa);
    }

    private void UstawZakladke(int index)
    {
        _zakladki.CurrentTab = index;

        for (int i = 0; i < _opisy.Count; i++)
        {
            _opisy[i].Visible = (i == index);
        }
    }

    private void Zamknij()
    {
        QueueFree();
    }
}