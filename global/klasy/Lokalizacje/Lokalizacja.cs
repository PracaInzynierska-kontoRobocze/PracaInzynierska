using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Lokalizacja : Node
{
    public static Lokalizacja Instance { get; private set; }

    public class DaneLokalizacji
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public StatusLokalizacji Status { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public List<int> OdblokowujePoziomy { get; set; } = new(); 

    }

    public List<DaneLokalizacji> Lokalizacje { get; private set; } = new();
    private Dictionary<int, DaneLokalizacji> _cache = new();

    public override void _Ready()
    {
        Instance = this;
        StanFabryczny();
    }

 

    public DaneLokalizacji GetLokalizacjaById(int id)
    {
        return _cache.GetValueOrDefault(id);
    }

    public void UpdateStatus(int id, int statusId)
    {
        if (id < 1) return;
        var lokalizacja = GetLokalizacjaById(id);
        if (lokalizacja == null) return;

        lokalizacja.Status = StatusLokalizacji.Instance.GetStatusById(statusId);

        if (lokalizacja.ParentId != 0)
        {
            var siblings = Lokalizacje.Where(l => l.ParentId == lokalizacja.ParentId).ToList();

            if (siblings.Any())
            {
                bool allStatus3 = siblings.All(l => l.Status?.Id == 3);
                bool anyStatus3 = siblings.Any(l => l.Status?.Id == 3);

                if (allStatus3)
                    UpdateStatus(lokalizacja.ParentId, 3);
                else if (anyStatus3)
                    UpdateStatus(lokalizacja.ParentId, 2);
            }
        }
        if (statusId == 3 && lokalizacja.OdblokowujePoziomy != null)
        {
            foreach (var odblokId in lokalizacja.OdblokowujePoziomy)
            {
                var odblokowywany = GetLokalizacjaById(odblokId);
                if (odblokowywany != null && odblokowywany.Status.Id != 1)
                {
                    odblokowywany.Status = StatusLokalizacji.Instance.GetStatusById(1); 
                }
            }
        }
    }

    public void StanFabryczny()
    {
        Lokalizacje = new List<DaneLokalizacji>
{
    new DaneLokalizacji
    {
        Id = -1,
        Nazwa = "NULL",
        Status = StatusLokalizacji.Instance.GetStatusById(4),
        Path = "",
        ParentId = 0,
        OdblokowujePoziomy = new List<int>()
    },
    new DaneLokalizacji
    {
        Id = 1,
        Nazwa = "Orapericus1",
        Status = StatusLokalizacji.Instance.GetStatusById(1),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1.tscn",
        ParentId = -1,
        OdblokowujePoziomy = new List<int>()
    },
    new DaneLokalizacji
    {
        Id = 2,
        Nazwa = "Orapericus1_1",
        Status = StatusLokalizacji.Instance.GetStatusById(1),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1_1/Orapericus1_1.tscn",
        ParentId = 1,
        OdblokowujePoziomy = new List<int> { 3 }
    },
    new DaneLokalizacji
    {
        Id = 3,
        Nazwa = "Orapericus1_2",
        Status = StatusLokalizacji.Instance.GetStatusById(4),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1_2/Orapericus1_2.tscn",
        ParentId = 1,
        OdblokowujePoziomy = new List<int> { 4 }
    },
    new DaneLokalizacji
    {
        Id = 4,
        Nazwa = "Orapericus1_3",
        Status = StatusLokalizacji.Instance.GetStatusById(4),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1_3/Orapericus1_3.tscn",
        ParentId = 1,
        OdblokowujePoziomy = new List<int> { 5 }
    },
    new DaneLokalizacji
    {
        Id = 5,
        Nazwa = "Orapericus1_4",
        Status = StatusLokalizacji.Instance.GetStatusById(4),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1_4/Orapericus1_4.tscn",
        ParentId = 1,
        OdblokowujePoziomy = new List<int> { 6 }
    },
    new DaneLokalizacji
    {
        Id = 6,
        Nazwa = "Orapericus1_5",
        Status = StatusLokalizacji.Instance.GetStatusById(4),
        Path = "res://sceny/lokalizacje/Orapericus/Orapericus1/Orapericus1_5/Orapericus1_5.tscn",
        ParentId = 1,
        OdblokowujePoziomy = new List<int>() 
    }
};

        _cache = Lokalizacje.ToDictionary(l => l.Id);
    }
}