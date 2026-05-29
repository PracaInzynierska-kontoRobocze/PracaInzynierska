using Godot;
using System;

public partial class EkranGłówny : Control
{

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToFile("res://UI/EkranGlowny/zapisy.tscn");
    }

    private void OnWyjdzPressed()
    {
        GetTree().Quit();
    }
    private void OnUstawieniaPressed()
    {
        var ustawienia = ustawieniaScene.Instantiate();

        AddChild(ustawienia);
    }



    private Sprite2D _star;
    private Label _wersja;
    PackedScene ustawieniaScene;
    public override void _Ready()
    {

        _star = GetNode<Sprite2D>("Star");
        _wersja= GetNode<Label>("Wersja");
        _wersja.Text = "Wersja: dev_" + PobierzWersjeGry();

        _star.Scale = Vector2.Zero;

        StartStarCycle();
        ustawieniaScene = GD.Load<PackedScene>("res://UI/Ustawienia/Ustawienia.tscn");
    }

    private string PobierzWersjeGry()
    {
        string sciezka = "res://version.txt";

        if (!FileAccess.FileExists(sciezka))
        {
            return "unknown";
        }

        using var file = FileAccess.Open(sciezka, FileAccess.ModeFlags.Read);

        if (file != null)
        {

            string shaHash = file.GetAsText().Trim();
            return shaHash;
        }

        return "error_loading_version";
    }

    private async void StartStarCycle()
    {
        while (true)
        {
            _star.GlobalPosition = GetRandomPositionInControl();
            _star.Scale = Vector2.Zero;

            float growTime = (float)GD.RandRange(0.8f, 1.3f);
            float shrinkTime = (float)GD.RandRange(0.2f, 0.5f);
            float delay = (float)GD.RandRange(0.15f, 0.35f);

            Tween tween = GetTree().CreateTween();

            tween.TweenProperty(_star, "scale", new Vector2(0.045f, 0.045f), growTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);

            tween.TweenProperty(_star, "scale", Vector2.Zero, shrinkTime)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.In);

            await ToSignal(tween, "finished");
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }
    }


    private Vector2 GetRandomPositionInControl()
    {
        Rect2 rect = GetGlobalRect();

        float x = (float)GD.RandRange(rect.Position.X, rect.End.X);
        float y = (float)GD.RandRange(rect.Position.Y, rect.End.Y);

        return new Vector2(x, y);
    }
}


