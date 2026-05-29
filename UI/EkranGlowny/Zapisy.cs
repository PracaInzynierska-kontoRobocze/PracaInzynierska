using Godot;
using System;

public partial class Zapisy : Control
{

    private void Back()
    {
        GetTree().ChangeSceneToFile("res://UI/EkranGlowny/ekran_glowny.tscn");
    }
}
