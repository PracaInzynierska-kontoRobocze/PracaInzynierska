using System.Collections.Generic;
using System.Linq;
using Godot;

    public partial class StatusLokalizacji:Node
    {
        public static StatusLokalizacji Instance { get; set; }
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public Color Kolor { get; set; }
        public List<StatusLokalizacji> StatusyLokalizacji { get; set; }
        public override void _Ready()
        {
            Instance = this;
            StatusyLokalizacji = new List<StatusLokalizacji> {
            new StatusLokalizacji { Id = 1, Nazwa = "Dostępny", Kolor = Color.Color8(255, 255, 0) },
            new StatusLokalizacji { Id = 2, Nazwa = "W trakcie", Kolor = Color.Color8(255, 165, 0) },
            new StatusLokalizacji { Id = 3, Nazwa = "Zakończony", Kolor = Color.Color8(0, 255, 0) },
            new StatusLokalizacji { Id = 4, Nazwa = "Niedostępny", Kolor = Color.Color8(255, 0, 0) }
            };
        }
        public StatusLokalizacji GetStatusById(int Id)
        {
            return StatusyLokalizacji.FirstOrDefault(s => s.Id == Id);
        }
    }

