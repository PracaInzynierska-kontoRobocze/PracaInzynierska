using Godot;
using System;
using static Mapa;

public partial class Mapa : Node2D
{

    private TextureRect galaktykaA;
    private TextureRect galaktykaB;
    private Label nazwaGalaktyki;
    public class Galaktyka
    {
        public Texture2D Tekstura { get; set; }
        public string Nazwa { get; set; }

        public Galaktyka(Texture2D tekstura, string nazwa)
        {
            Tekstura = tekstura;
            Nazwa = nazwa;
        }
    }
    private Galaktyka[] galaktyki;

    private Vector2 startPos;
    private float slideWidth;

    private bool sliding = false;
    public override void _Ready()
    {
        galaktykaA = GetNode<TextureRect>("Galaktyki/GalaktykaA");
        galaktykaB = GetNode<TextureRect>("Galaktyki/GalaktykaB");
        nazwaGalaktyki = GetNode<Label>("NazwaGalaktyki");
        galaktyki = new Galaktyka[]
        {
                new Galaktyka(
        GD.Load<Texture2D>("res://assety/grafika/obiekty/kosmos/galaktyki/Orapericus.png"),
        "Orapericus"
    ),
    new Galaktyka(
        GD.Load<Texture2D>("res://assety/grafika/obiekty/kosmos/galaktyki/Termisa.png"),
        "Termisa"
    )

        };

        startPos = galaktykaA.Position;
        slideWidth = GetNode<Control>("Galaktyki").Size.X;

        galaktykaA.Position = startPos;

        galaktykaA.Texture = galaktyki[Parametry.Instance.WybranaGalaktyka].Tekstura;
        nazwaGalaktyki.Text = galaktyki[Parametry.Instance.WybranaGalaktyka].Nazwa;
        Teksty.Wyswietl("start_gry", InfoPanel.TypPanelu.Jednorazowy);
    }

    private void _on_prawo_pressed()
    {
        SlideNext();
    }
    private void _on_lewo_pressed()
    {
        SlidePrev();
    }

    private async void SlideNext()
    {
        if (sliding) return;
        sliding = true;

        int nextIndex = (Parametry.Instance.WybranaGalaktyka + 1) % galaktyki.Length;
        float offsetX = galaktykaA.Size.X;
        float screenWidth = GetViewportRect().Size.X;
        galaktykaB.Texture = galaktyki[nextIndex].Tekstura;
        galaktykaB.Position = new Vector2(screenWidth, startPos.Y);
        nazwaGalaktyki.Text = galaktyki[nextIndex].Nazwa;
        var tween = CreateTween();
        tween.TweenProperty(galaktykaA, "position:x", -screenWidth - galaktykaA.Size.X, 0.5)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(galaktykaB, "position:x", startPos.X, 0.5)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");

        Parametry.Instance.WybranaGalaktyka = nextIndex;

        var temp = galaktykaA;
        galaktykaA = galaktykaB;
        galaktykaB = temp;

        sliding = false;
    }

    private async void SlidePrev()
    {
        if (sliding) return;
        sliding = true;

        int prevIndex = (Parametry.Instance.WybranaGalaktyka - 1 + galaktyki.Length) % galaktyki.Length;
        float offsetX = galaktykaA.Size.X;
        float screenWidth = GetViewportRect().Size.X;
        galaktykaB.Texture = galaktyki[prevIndex].Tekstura;
        galaktykaB.Position = new Vector2(-screenWidth, startPos.Y);
        nazwaGalaktyki.Text = galaktyki[prevIndex].Nazwa;

        var tween = CreateTween();
        tween.TweenProperty(galaktykaA, "position:x", screenWidth, 0.5)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(galaktykaB, "position:x", -startPos.X, 0.5)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");

        Parametry.Instance.WybranaGalaktyka = prevIndex;

        var temp = galaktykaA;
        galaktykaA = galaktykaB;
        galaktykaB = temp;

        sliding = false;
    }

    private void WybierzGalaktykę()
    {
        if (!sliding)
        {
            if (Parametry.Instance.WybranaGalaktyka == 1)
            {
                GetTree().ChangeSceneToFile("res://sceny/lokalizacje/Termisa/Termisa.tscn");
            }
            else if (Parametry.Instance.WybranaGalaktyka == 0)
            {
                GetTree().ChangeSceneToFile("res://sceny/lokalizacje/Orapericus/Orapericus.tscn");
            }

        }
    }
}
