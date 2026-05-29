using Godot;
using System;

public partial class Wynik : Control
{
    public string Text { get; set; }
    public override void _Ready()
    {
        var label = GetNode<RichTextLabel>("Panel/RichTextLabel");
        label.Text = Text;
    }
    private void _on_button_pressed() 
    {
        QueueFree();
    }
}
