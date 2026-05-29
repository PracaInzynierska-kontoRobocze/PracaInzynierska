using Godot;
using System.Linq;
using System.Xml;

public partial class Tabela : Control
{

    private GridContainer grid;
    private LineEdit nameLabel;
    private Button _btnLewo;
    private Button _btnPrawo;
    private GridContainer proceduresGrid;
    private LineEdit proceduresLabel;
    public ObiektTabelowy ObiektMaster { get; set; }
    public override void _Ready()
    {
        grid = GetNode<GridContainer>("Panel/VBoxContainer/GridContainer");
        nameLabel = GetNode<LineEdit>("Panel/NazwaTabeli");
        _btnLewo = GetNode<Button>("Panel/Lewo");
        _btnPrawo = GetNode<Button>("Panel/Prawo");

        proceduresGrid = GetNode<GridContainer>("Panel2/VBoxContainer/GridContainer");
        proceduresLabel = GetNode<LineEdit>("Panel2/NazwaTabeli");

    }

    public void LoadTable(Table table)
    {
        nameLabel.Text = table.Name;
        nameLabel.FocusEntered += DeselectAllFields; 

        var widoczneKolumny = table.Columns.Where(c => c.Visible).ToList();
        grid.Columns = widoczneKolumny.Count;
        grid.QueueFreeChildren();

        foreach (var column in widoczneKolumny)
        {
            var header = new LineEdit
            {
                Text = column.Name,
                Editable = false,
                Flat = true,
                Alignment = HorizontalAlignment.Center,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                FocusMode = FocusModeEnum.Click
            };
            header.AddThemeColorOverride("font_uneditable_color", new Color(1, 1, 0));
            header.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
            header.FocusEntered += DeselectAllFields;
            grid.AddChild(header);
        }

        foreach (var row in table.Rows)
        {
            foreach (var column in widoczneKolumny)
            {
                var cellEdit = new LineEdit();
                cellEdit.Text = row.Cells.TryGetValue(column.Name, out var cell)
                    ? (cell.Visible ? (cell.Value?.ToString() ?? "") : "*")
                    : "*";

                cellEdit.Editable = false;
                cellEdit.Flat = true;
                cellEdit.Alignment = HorizontalAlignment.Center;
                cellEdit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                cellEdit.FocusMode = FocusModeEnum.Click;
                cellEdit.SelectAllOnFocus = true;

                cellEdit.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
                cellEdit.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());

                cellEdit.FocusEntered += DeselectAllFields;

                grid.AddChild(cellEdit);
            }
        }
        LoadProcedures();
    }

    public void LoadProcedures()
    {
        proceduresLabel.Text = "PROCEDURY";
        proceduresGrid.QueueFreeChildren();

        if (ObiektMaster == null || ObiektMaster.Procedures == null || ObiektMaster.Procedures.Count == 0)
            return;

        var proc = ObiektMaster.Procedures[ObiektMaster._currentProcedureIndex];
        proceduresGrid.Columns = 2;

        string paramsString = (proc.Parameters != null && proc.Parameters.Count > 0)
            ? string.Join(", ", proc.Parameters.Select(p => $"{p.Name} {p.Type.ToString().ToUpper()}"))
            : "---";

        var dane = new[] {
        ("Nazwa:", proc.ProcedureName),
        ("Parametry:", paramsString),
        ("Typ:", proc.Rodzaj.ToString())
    };

        foreach (var (label, wartosc) in dane)
        {
            var lbl = new Label
            {
                Text = label,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            lbl.AddThemeColorOverride("font_color", new Color(1, 1, 0));
            proceduresGrid.AddChild(lbl);

            var val = new LineEdit
            {
                Text = wartosc,
                Editable = false,
                Flat = true,
                Alignment = HorizontalAlignment.Left, 
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                FocusMode = FocusModeEnum.Click,
                SelectAllOnFocus = true
            };

            val.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
            val.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());

            val.FocusEntered += DeselectAllFields;

            proceduresGrid.AddChild(val);
        }

        proceduresLabel.Text = $"PROCEDURA ({ObiektMaster._currentProcedureIndex + 1}/{ObiektMaster.Procedures.Count})";
    }


    public void OpenNext() { ObiektMaster.OpenNextTable(); }
    public void OpenPrev() { ObiektMaster.OpenPreviousTable(); }
    public void OpenNextProc() { ObiektMaster.NextProcedure(); }
    public void OpenPrevProc() { ObiektMaster.PrevProcedure(); }

    public void HideLewo() { _btnLewo.Visible = false; }

    public void HidePrawo() { _btnPrawo.Visible = false; }

    private void DeselectAllFields()
    {
        nameLabel.Deselect();

        foreach (var child in grid.GetChildren())
        {
            if (child is LineEdit le) le.Deselect();
        }

        foreach (var child in proceduresGrid.GetChildren())
        {
            if (child is LineEdit le) le.Deselect();
        }
    }

}
public static class GodotExtensions
{
    public static void QueueFreeChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
            (child as Node)?.QueueFree();
    }
}

