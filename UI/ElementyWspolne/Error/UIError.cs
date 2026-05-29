using Godot;

public partial class UIError : Control
{
    public string MessageText { get; set; }
    public override void _Ready()
    {
        GetNode<Label>("Panel/RichTextLabel").Text = MessageText;
        GetNode<Button>("Button").Pressed += () => QueueFree();
    }
}
