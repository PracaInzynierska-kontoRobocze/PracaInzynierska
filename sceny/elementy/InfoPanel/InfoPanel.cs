using Godot;
using System;

public partial class InfoPanel : CanvasLayer
{
    public enum TypPanelu { Info, KoniecPoziomu, Podpowiedz,Jednorazowy }

    private RichTextLabel _label;
    private Button _okButton;
    private TypPanelu aktualnyTyp = TypPanelu.Info;

    public override void _Ready()
    {
        _label = GetNode<RichTextLabel>("%TekstLabel");
        _okButton = GetNode<Button>("%OkButton");

        ProcessMode = ProcessModeEnum.Always;
    }

    public void PokazWiadomosc(string tresc, TypPanelu typ)
    {
       
        if (_label != null)
            _label.Text = $"[center]{tresc}[/center]";
        aktualnyTyp = typ;

    }

    private void OnOkPressed()
    {
        if (aktualnyTyp == TypPanelu.KoniecPoziomu)
        {
            GetTree().Paused = false;
            var lokalizacjaId = Parametry.Instance.WybranaLokalizacja;
            var parent_id = Lokalizacja.Instance.GetLokalizacjaById(lokalizacjaId).ParentId;
            var file = Lokalizacja.Instance.GetLokalizacjaById(parent_id).Path;
            if (file != null) { GetTree().ChangeSceneToFile(file); }
            else { GetTree().ChangeSceneToFile("res://UI/Mapa/mapa.tscn"); }

            QueueFree();
        }
        else
        {
            QueueFree();
        }
    }


}