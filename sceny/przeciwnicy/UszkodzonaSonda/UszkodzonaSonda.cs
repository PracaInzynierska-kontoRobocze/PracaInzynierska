using System.Collections.Generic;

public partial class UszkodzonaSonda : Sonda
{
    public override void _Ready()
    {
        base._Ready();

        List<Table> noweTabele = new List<Table>();

        foreach (var stara in Tables)
        {
            var zabezpieczona = new ZabezpieczonaTabela
            {
                Name = stara.Name,
                Columns = stara.Columns,
                Rows = stara.Rows,
                Kod = prawidlowyKod.ToString()

            };
            noweTabele.Add(zabezpieczona);
        }

        Tables = noweTabele;
    }
}