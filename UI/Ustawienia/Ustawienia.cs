using Godot;
using System;

public partial class Ustawienia : Control
{
    private ItemList rozdzielczoscList;
    private ItemList trybList;
    private ItemList podpowiedziList;
    public static Ustawienia Instance { get; set; }
    public MenuEsc parent { get; set; }
    public Vector2I wybranaRozdzielczosc;
    public string tryb;
    public bool podpowiedzi;

    public override void _Ready()
    {
        Instance = this;

        rozdzielczoscList = GetNodeOrNull<ItemList>("Panel/Rozdzielczosc");
        trybList = GetNodeOrNull<ItemList>("Panel/Tryb");
        podpowiedziList = GetNodeOrNull<ItemList>("Panel/Podpowiedzi");

        var data = DB.Instance.LoadUstawienia();
        wybranaRozdzielczosc = data.Item1;
        tryb = data.Item2;
        podpowiedzi = data.Item3;
        if(Parametry.Instance.CurrentSlot == null)
        {
            var label4 = GetNodeOrNull<Label>("Panel/Label4");
            var resetButton = GetNodeOrNull<Button>("Panel/ResetButton");

            if (label4 != null) label4.Visible = false;
            if (resetButton != null) resetButton.Visible = false;
        }
        ZastosujUstawienia();
        UstawSelectedUI();
    }

    private readonly Vector2I[] DostepneRozdzielczosci = new Vector2I[]
    {
    new Vector2I(2560, 1440), 
    new Vector2I(1920, 1080), 
    new Vector2I(1600, 900),  
    new Vector2I(1366, 768),
    new Vector2I(1280, 720),
    new Vector2I(1920, 1200),
    new Vector2I(1680, 1050),
    new Vector2I(1440, 900),
    new Vector2I(1280, 800),
    new Vector2I(1280, 960),
    new Vector2I(1024, 768),
    new Vector2I(800, 600)
    };
    private void Zapisz()
    {
        
        if (rozdzielczoscList != null && rozdzielczoscList.GetSelectedItems().Length > 0)
        {
            int index = rozdzielczoscList.GetSelectedItems()[0];
            wybranaRozdzielczosc = DostepneRozdzielczosci[index];
        }

        if (trybList != null && trybList.GetSelectedItems().Length > 0)
        {
            int index = trybList.GetSelectedItems()[0];
            tryb = trybList.GetItemText(index);
        }

        if (podpowiedziList != null && podpowiedziList.GetSelectedItems().Length > 0)
        {
            int index = podpowiedziList.GetSelectedItems()[0];
            podpowiedzi = index == 0;
        }

        DB.Instance.SaveUstawienia(wybranaRozdzielczosc, tryb, podpowiedzi);
        ZastosujUstawienia();
    }


    private void Zamknij() 
    {
        if (parent != null)
            parent.CloseTop();
        QueueFree(); 
    }

    private void ZastosujUstawienia()
    {
        DisplayServer.WindowSetSize(wybranaRozdzielczosc);

        if (tryb == "Pełny ekran")
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        else
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

        if (tryb != "Pełny ekran")
        {
            var screenSize = DisplayServer.ScreenGetSize();
            var windowSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - windowSize / 2);
        }
    }

    private void UstawSelectedUI()
    {
        if (rozdzielczoscList != null)
        {
            string szukanaRozdzielczosc = $"{wybranaRozdzielczosc.X}x{wybranaRozdzielczosc.Y}";
            for (int i = 0; i < rozdzielczoscList.ItemCount; i++)
            {
                if (rozdzielczoscList.GetItemText(i).StartsWith(szukanaRozdzielczosc))
                {
                    rozdzielczoscList.Select(i);
                    break;
                }
            }
        }

        if (trybList != null)
        {
            for (int i = 0; i < trybList.ItemCount; i++)
            {
                if (trybList.GetItemText(i).ToLower() == tryb.ToLower())
                {
                    trybList.Select(i);
                    break;
                }
            }
        }

        if (podpowiedziList != null)
        {
            string podpowiedziText = podpowiedzi ? "Wł." : "Wył.";
            for (int i = 0; i < podpowiedziList.ItemCount; i++)
            {
                if (podpowiedziList.GetItemText(i) == podpowiedziText)
                {
                    podpowiedziList.Select(i);
                    break;
                }
            }
        }
    }

    private void OnResetWiadomosciPressed()
    {
        DB.Instance.ResetujJednorazoweWiadomosci();
    }
}