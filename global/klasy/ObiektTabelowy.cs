using Godot;
using System.Collections.Generic;


    public partial class ObiektTabelowy:CharacterBody2D
    {

        public string Nazwa { get; set; }
        public List<Table> Tables { get; set; } = new();
        public List<ExecQuery> Procedures { get; set; } = new();
        public LokalizacjaQuery Lokalizacja;


        private double lastClickTime = 0;
        private const double doubleClickThreshold = 1;
        public bool isOpen;

        public override void _Ready()
        {
        base._Ready();
        Nazwa = (string)GetMeta("Nazwa"); 
        }
         public void _on_button_pressed()
        {
            
            double currentTime = Time.GetTicksMsec() / 1000.0;
            if (currentTime - lastClickTime < doubleClickThreshold)
            {
                lastClickTime = 0;
                if (isOpen)
                {
                    CloseTable();
                    isOpen = false;
                }
                else
                {
                    Lokalizacja?.ZamknijWszystkieTabele();
                    OpenTable();
                    isOpen = true;
                }


            }
            else
            {
                lastClickTime = currentTime;
            }
        }

        public Tabela _currentTableInstance;
        public int _currentTableIndex = 0;
    public int _currentProcedureIndex = 0; 
    public void OpenTable()
    {
        PackedScene uiScene = GD.Load<PackedScene>("res://sceny/elementy/Tabela/tabela.tscn");
        _currentTableInstance = uiScene.Instantiate<Tabela>();

        GetTree().Root.AddChild(_currentTableInstance);
        _currentTableInstance.ObiektMaster = this;
        _currentTableInstance.LoadTable(Tables[_currentTableIndex]);
        if (_currentTableIndex >= Tables.Count - 1)
            _currentTableInstance.HidePrawo();
        if (_currentTableIndex <= 0)
            _currentTableInstance.HideLewo();
    }
        public void CloseTable()
        {
            if (_currentTableInstance != null)
            {
                _currentTableInstance.QueueFree();
                _currentTableInstance = null;
            }
        }

        public Table Tabela(string name)
        {
            var tabela = Tables.Find(t => t.Name == name);
            if (tabela == null) { Global.UIError($"Nie ma tabeli {name}"); return null; }
            else return tabela;

        }
        public void DodajTabele(string nazwa)
        {
            var table = new Table
            {
                Name = nazwa,
                ObiektMaster = this
            };
            Tables.Add(table);
        }
        public void ZarejestrujWLokalizacji(LokalizacjaQuery lokalizacja)
        {
            Lokalizacja = lokalizacja;
            lokalizacja.DodajObiekt(this);
        }
        public void Skasuj()
        {
        Lokalizacja.SkasujObiekt(this);
        QueueFree();
        }
    public void OpenNextTable()
        {
            CloseTable();
            _currentTableIndex = _currentTableIndex + 1; ;
            OpenTable();
        }

        public void OpenPreviousTable()
        {
            CloseTable();
            _currentTableIndex = _currentTableIndex - 1; ;
            OpenTable();
        }

    public void NextProcedure()
    {
        if (_currentProcedureIndex < Procedures.Count - 1)
        {
            _currentProcedureIndex++;
            _currentTableInstance.LoadProcedures();
        }
    }

    public void PrevProcedure()
    {
        if (_currentProcedureIndex > 0)
        {
            _currentProcedureIndex--;
            _currentTableInstance.LoadProcedures(); 
        }
    }

    public virtual string Exec(string nazwa, List<ExecParameter> parametry) { return null; }

    public virtual string WarunkiObiketuUpdate(UpdateQuery query) { return string.Empty; }
}

