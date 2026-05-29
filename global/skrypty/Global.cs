using Godot;
using System;
using System.Diagnostics;

public partial class Global : Node
{
    private static Global instance;
    private PackedScene errorScene;
    private PackedScene debugScene;
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DebugOn"))
        {
            DebugOn();
        }

    }
    override public void  _Ready()
    {
        instance = this;
        errorScene = GD.Load<PackedScene>("res://UI/ElementyWspolne/Error/error.tscn");
        debugScene = GD.Load<PackedScene>("res://global/sceny/debug.tscn");
    }

    public static void UIError(string message)
    {
        var errorInstance = instance.errorScene.Instantiate<UIError>();

        errorInstance.MessageText = message;

        instance.GetTree().CurrentScene.AddChild(errorInstance);
    }

    public static  void ThrowError(string message)
    {
        GD.PrintErr(message);
        Debugger.Break();       
    }

    public void DebugOn()
    {
        var debugInstance = debugScene.Instantiate<Debug>();
        GetTree().CurrentScene.AddChild(debugInstance);
    }

}
