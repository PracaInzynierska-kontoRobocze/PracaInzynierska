using Godot;
using System;
using System.IO;
using Microsoft.Data.Sqlite;

public partial class DB : Node
{

    public static DB Instance { get; private set; }
    string godotPath = "user://zapisy.db";
    string realPath;
    string directory;


    public override void _Ready()
    {
        realPath = ProjectSettings.GlobalizePath(godotPath);
        directory = Path.GetDirectoryName(realPath);
        Instance = this;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        ;
        Init();

    }

    private void Init()
    {
        using var connection = GetConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS SaveSlots (
                 Id INTEGER PRIMARY KEY,
                 Nazwa TEXT NOT NULL,
                 Data TEXT NOT NULL,
                 Poziomy INTEGER NOT NULL DEFAULT 0,
                 Czas REAL NOT NULL DEFAULT 0.0
                );

            CREATE TABLE IF NOT EXISTS SaveData (
                SlotId INTEGER,
                Key TEXT,
                Value TEXT
                ); 
            CREATE TABLE IF NOT EXISTS SaveLokalizacje (
                SlotId INTEGER,
                LokalizacjaId INTEGER,
                StatusId INTEGER,
                PRIMARY KEY (SlotId, LokalizacjaId)
                );
             CREATE TABLE IF NOT EXISTS SaveUstawienia (
                Id INTEGER PRIMARY KEY,
                Key TEXT UNIQUE,
                Value TEXT
                );
            CREATE TABLE IF NOT EXISTS SaveJednorazowe (
                SlotId INTEGER,
                Klucz TEXT,
                PRIMARY KEY (SlotId, Klucz)
        );
                    ";
        command.ExecuteNonQuery();
    }

    public SqliteConnection GetConnection()
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = realPath
        };

        var connection = new SqliteConnection(builder.ConnectionString);

        connection.Open();

        return connection;
    }
    public Slot GetSlot(int id)
    {
        using var connection = GetConnection();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Nazwa, Data, Poziomy, Czas
            FROM SaveSlots
            WHERE Id = @id
        ";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Slot
        {
            Id = reader.GetInt32(0),
            Nazwa = reader.GetString(1),
            Data = reader.GetString(2),
            Poziomy = reader.GetInt32(3),
            Czas = reader.GetDouble(4)
        };
    }

    public void Save()
    {
        SaveSlotData();
        SaveLokalizacjeData();
    }

    public void SaveLokalizacjeData()
    {
        foreach (var lokalizacja in Lokalizacja.Instance.Lokalizacje)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO SaveLokalizacje (SlotId, LokalizacjaId, StatusId)
                VALUES (@slotId, @lokalizacjaId, @statusId)
                 ";
            command.Parameters.AddWithValue("@slotId", Parametry.Instance.CurrentSlot.Id);
            command.Parameters.AddWithValue("@lokalizacjaId", lokalizacja.Id);
            command.Parameters.AddWithValue("@statusId", lokalizacja.Status.Id);
            command.ExecuteNonQuery();
        }
    }
    public void SaveSlotData()
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO SaveSlots (Id, Nazwa, Data, Poziomy, Czas)
            VALUES (@id, @nazwa, @data, @poziomy, @czas)
             ";
        Parametry.Instance.RefreshParametry();
        command.Parameters.AddWithValue("@id", Parametry.Instance.CurrentSlot.Id);
        command.Parameters.AddWithValue("@nazwa", Parametry.Instance.CurrentSlot.Nazwa ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@data", Parametry.Instance.CurrentSlot.Data ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@poziomy", Parametry.Instance.CurrentSlot.Poziomy);
        command.Parameters.AddWithValue("@czas", Parametry.Instance.CurrentSlot.Czas);

        command.ExecuteNonQuery();
    }
    public void Load(int id)
    {
        LoadSlot(id);
        LoadLokalizacje(id);
    }
    public void LoadSlot(int id)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = @"
        SELECT Id, Nazwa, Data, Poziomy, Czas
        FROM SaveSlots
        WHERE Id = @id
         ";

        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            Parametry.Instance.CurrentSlot = new Slot
            {
                Id = reader.GetInt32(0),
                Nazwa = reader.IsDBNull(1) ? null : reader.GetString(1),
                Data = reader.IsDBNull(2) ? null : reader.GetString(2),
                Poziomy = reader.GetInt32(3),
                Czas = reader.GetDouble(4)
            };
        }

    }
    public void LoadLokalizacje(int id)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = @"
        SELECT LokalizacjaId, StatusId 
        FROM SaveLokalizacje 
        WHERE SlotId = @slotId ";

        command.Parameters.AddWithValue("@slotId", id);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int lokalizacjaId = reader.GetInt32(0);
            int statusId = reader.GetInt32(1);
            Lokalizacja.Instance.UpdateStatus(lokalizacjaId, statusId);
        }
    }

    public void DeleteSlot(int id)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = @"
        DELETE FROM SaveData WHERE SlotId = @slotId;
        DELETE FROM SaveLokalizacje WHERE SlotId = @slotId;
        DELETE FROM SaveSlots WHERE Id = @slotId;
        DELETE FROM SaveJednorazowe WHERE SlotId = @slotId;
        ";

        command.Parameters.AddWithValue("@slotId", id);

        using var reader = command.ExecuteReader();
    }

    public void SaveUstawienia(Vector2I rozdzielczosc, string tryb, bool podpowiedzi)
    {
        using var connection = GetConnection();
        using var cmd = connection.CreateCommand();
        void Save(string key, string value)
        {
            cmd.CommandText = @"
            INSERT INTO SaveUstawienia (Key, Value)
            VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = @value;
        ";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);

            cmd.ExecuteNonQuery();
        }

        Save("width", rozdzielczosc.X.ToString());
        Save("height", rozdzielczosc.Y.ToString());
        Save("tryb", tryb);
        Save("podpowiedzi", podpowiedzi ? "1" : "0");
    }
    public (Vector2I, string, bool) LoadUstawienia()
    {
        Vector2I rozdzielczosc = new Vector2I(1920, 1080);
        string tryb = "Pełny ekran";
        bool podpowiedzi = true;

        using var connection = GetConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Key, Value FROM SaveUstawienia";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string key = reader.GetString(0);
            string value = reader.GetString(1);

            switch (key)
            {
                case "width":
                    rozdzielczosc.X = int.Parse(value);
                    break;

                case "height":
                    rozdzielczosc.Y = int.Parse(value);
                    break;

                case "tryb":
                    tryb = value;
                    break;

                case "podpowiedzi":
                    podpowiedzi = value == "1";
                    break;
            }
        }

        return (rozdzielczosc, tryb, podpowiedzi);
    }

    public bool CzyTekstBylPokazany(string klucz)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "SELECT 1 FROM SaveJednorazowe WHERE SlotId = @slotId AND Klucz = @klucz";
        command.Parameters.AddWithValue("@slotId", Parametry.Instance.CurrentSlot.Id);
        command.Parameters.AddWithValue("@klucz", klucz);

        using var reader = command.ExecuteReader();
        return reader.HasRows;
    }

    public void ZapiszPokazanyTekst(string klucz)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "INSERT OR IGNORE INTO SaveJednorazowe (SlotId, Klucz) VALUES (@slotId, @klucz)";
        command.Parameters.AddWithValue("@slotId", Parametry.Instance.CurrentSlot.Id);
        command.Parameters.AddWithValue("@klucz", klucz);

        command.ExecuteNonQuery();
    }
    public void ResetujJednorazoweWiadomosci()
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM SaveJednorazowe WHERE SlotId = @slotId";
        command.Parameters.AddWithValue("@slotId", Parametry.Instance.CurrentSlot.Id);

        command.ExecuteNonQuery();
    }
}



