using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Debug : Control
{
    LineEdit PoziomSelecta;
    LineEdit Roz1;
    LineEdit Roz2;
    public override void _Ready()
    {
        PoziomSelecta= GetNode<LineEdit>("Panel/SelectPoziom");
        PoziomSelecta.Text = Atrybuty.QueryPoziom.ToString();
        Roz1 =  GetNode<LineEdit>("Panel/Roz1");
        Roz2 =  GetNode<LineEdit>("Panel/Roz2");
        Vector2I windowSize = DisplayServer.WindowGetSize();
        Roz1.Text = windowSize.X.ToString();
        Roz2.Text = windowSize.Y.ToString();

    }

    public void _on_full_pressed() 
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
    }
    public void _on_win_pressed()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed); 
    }



    public void Save()
    {
        Atrybuty.QueryPoziom = Convert.ToInt32(PoziomSelecta.Text);
        DisplayServer.WindowSetSize(new Vector2I(Int32.Parse(Roz1.Text), Int32.Parse(Roz2.Text)));

    }

    public void Close()
    {
        this.QueueFree();
    }

    public void ZapisManual()
    {
        DB.Instance.Save();
    }

    public void ZaliczObecnyPoziom()
    {
        Lokalizacja.Instance.UpdateStatus(Parametry.Instance.WybranaLokalizacja, 3);
        DB.Instance.Save();
    }

    public void PokazPanel()
    {
        Teksty.Wyswietl("Ora1_3_sukces", InfoPanel.TypPanelu.KoniecPoziomu);
    }

}
