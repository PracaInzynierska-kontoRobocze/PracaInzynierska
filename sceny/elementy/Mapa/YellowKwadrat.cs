using Godot;

public partial class YellowKwadrat : Control
{
    private Lokalizacja.DaneLokalizacji lokalizacja;
    private ColorRect colorRect;
    override public void _Ready()
    {
        lokalizacja = Lokalizacja.Instance.GetLokalizacjaById((int)GetMeta("Lokalizacja"));
        colorRect = GetNode<ColorRect>("ColorRect");
        colorRect.Color = lokalizacja.Status.Kolor;
    }

    public void ButtonPressed()
    {
        if (lokalizacja.Status.Id == 1 || lokalizacja.Status.Id == 2)
        {
            GetTree().ChangeSceneToFile(lokalizacja.Path);
        }
    }
}
