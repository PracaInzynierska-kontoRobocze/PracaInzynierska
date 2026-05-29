using Godot;
using System.Collections.Generic;

public partial class MenuEsc : Control
{
    private Stack<Control> _stack = new();
    private PackedScene scenaUstawienia;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        MouseFilter = MouseFilterEnum.Stop;
        scenaUstawienia = GD.Load<PackedScene>("res://UI/Ustawienia/Ustawienia.tscn");
        Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && Parametry.Instance.CurrentSlot is not null)
        {
            GetViewport().SetInputAsHandled();
            HandleEsc();
        }
    }

    public void HandleEsc()
    {
        if (_stack.Count == 0)
        {
            GetTree().Paused = true; 
            _stack.Push(this);
            Show();
            GetNode<Control>("Panel").Show(); 
            GrabFocus();
        }
        else
        {
            CloseTop();
        }
    }

    public void PushMenu(Control menu)
    {
        if (_stack.Count > 0 && _stack.Peek() != this)
        {
            _stack.Peek().Hide();
        }
        else if (_stack.Count > 0 && _stack.Peek() == this)
        {
            GetNode<Control>("Panel").Hide();
        }

        _stack.Push(menu);
        menu.Show();
    }


    public void CloseTop()
    {
        while (_stack.Count > 0 && !IsInstanceValid(_stack.Peek()))
        {
            _stack.Pop();
        }
        if (_stack.Count <= 1)
        {
            Hide();
            _stack.Clear();
            GetTree().Paused = false; 
            return;
        }

        var top = _stack.Pop();
        top.Hide();

        if (top != this)
        {
            top.QueueFree();
        }

        if (_stack.Count > 0)
        {
            var prev = _stack.Peek();
            prev.Show();

            if (prev == this)
            {
                GetNode<Control>("Panel").Show();
            }

            prev.GrabFocus();
        }
    }

    private void OnUstawieniaPressed()
    {
        var instancjaUstawien = scenaUstawienia.Instantiate<Ustawienia>();
        AddChild(instancjaUstawien);
        instancjaUstawien.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        instancjaUstawien.MoveToFront();
        instancjaUstawien.parent = this;
        PushMenu(instancjaUstawien);
    }


    private void OnWznowPressed() => CloseTop();

    private void OnMapaPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://UI/Mapa/mapa.tscn");
        CloseTop();
    }

    private void OnMenuGlownePressed()
    {
        Parametry.Instance.CurrentSlot = null;
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://UI/EkranGlowny/ekran_glowny.tscn");
        CloseTop();
    }

    private void OnPomocPressed()
    {
        var pomoc = GD.Load<PackedScene>("res://UI/Pomoc/Pomoc.tscn").Instantiate<Pomoc>();
        GetTree().Root.AddChild(pomoc);
        CloseTop();
    }

    private void OnWyjdzPressed() => GetTree().Quit();
}