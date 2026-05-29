using Godot;
using System.Linq;


public partial class Parametry : Node
    {
        public static Parametry Instance { get; private set; }
        public Slot CurrentSlot { get; set; }
        public double CzasGry { get; set; }
        public int WybranaGalaktyka { get; set; }
        public int WybranaLokalizacja { get; set; }
    public override void _Ready()
        {
            Instance = this;
        }

    public void RefreshParametry()
    {
        var datetime = Time.GetDatetimeDictFromSystem();
        CurrentSlot.Data = string.Format("{0:00}/{1:00}/{2} {3:00}:{4:00}",
            datetime["day"],
            datetime["month"],
            datetime["year"],
            datetime["hour"],
            datetime["minute"]
        );
        CurrentSlot.Czas += CzasGry;
            CzasGry = 0;
            CurrentSlot.Poziomy = LiczbaUkonczonychPoziomow();
    }

    public void Clear() 
    {
        CurrentSlot = null;
        CzasGry = 0;
        WybranaGalaktyka = 0;
        WybranaLokalizacja = 0;
    }

    public int LiczbaUkonczonychPoziomow()
    {
        var poziomy= Lokalizacja.Instance.Lokalizacje.Where(l => l.Status.Id == 3&l.ParentId!=0).ToList();
        return poziomy.Count;
    }
    public override void _Process(double delta)
        {
            CzasGry += delta;
        }
    }
