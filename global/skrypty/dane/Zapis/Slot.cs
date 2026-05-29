using Godot;
using System;

    public partial class Slot : Node
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Data { get; set; }
        public int Poziomy { get; set; }
        public double Czas { get; set; }
        public override void _Ready()
        {
            var nazwaLabel = GetNode<Label>("Button/Panel/Nazwa");
            Id = (int)GetMeta("Slot");
            var slot = DB.Instance.GetSlot(Id);
            if (slot != null)
            {
                Data = slot.Data;
                Poziomy = slot.Poziomy;
                Czas = slot.Czas;
                Nazwa=slot.Nazwa;
                var dataLabel = GetNode<Label>("Button/Panel/Data");
                var poziomyLabel = GetNode<Label>("Button/Panel/Poziomy");
                var czasLabel = GetNode<Label>("Button/Panel/Czas");
                dataLabel.Text = Data;
                poziomyLabel.Text = Poziomy.ToString();
                var span = TimeSpan.FromSeconds(Czas);
                czasLabel.Text = $"{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
            }
            else 
            {
                Nazwa = "PUSTY SLOT";
            }
            nazwaLabel.Text = Nazwa;
        }

        private void OnSlotPressed()
        {
            Lokalizacja.Instance.StanFabryczny();
            Parametry.Instance.Clear();
        if (Nazwa == "PUSTY SLOT")
            {
                Nazwa = $"Slot {Id}";
            }
            else { 
            DB.Instance.Load(Id);
            }
            Parametry.Instance.CurrentSlot = this;
            GetTree().ChangeSceneToFile("res://UI/Mapa/mapa.tscn");
        }
        
        private void DeleteSlot()
    {
            DB.Instance.DeleteSlot(Id);
            GetTree().ReloadCurrentScene();
    }
}
