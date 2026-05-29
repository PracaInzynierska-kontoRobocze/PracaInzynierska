using Godot;
using System;

public partial class Error : Control
{

    private RichTextLabel label;
    public string text;
    public override void _Ready()
    {
        this.AnchorLeft = 0.5f;
        this.AnchorTop = 0.5f;
        this.AnchorRight = 0.5f;
        this.AnchorBottom = 0.5f;

        this.OffsetLeft = -this.Size.X / 2;
        this.OffsetTop = -this.Size.Y / 2;
        this.OffsetRight = this.Size.X / 2;
        this.OffsetBottom = this.Size.Y / 2;

        label = GetNode<RichTextLabel>("Panel/RichTextLabel");
        label.Text = text;
    }

    private void Close()
    {
        this.QueueFree();
    }

}
