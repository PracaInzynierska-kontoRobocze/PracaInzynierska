using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ExecQuery
{
    public string ProcedureName { get; set; }
    public List<ExecParameter> Parameters { get; set; } = new();
    public RodzajProcedury Rodzaj { get; set; } = RodzajProcedury.Ręczna;

    public void ParseParameters(string parametersText)
    {
        Parameters.Clear();
        var splitted = SplitParameters(parametersText);
        for (int i = 0; i < splitted.Count; i++)
        {
            var val = splitted[i];
            ExecParameterType type;
            if (val.StartsWith("'") && val.EndsWith("'"))
            {
                val = val.Substring(1, val.Length - 2);
                type = ExecParameterType.Text;
            }
            else if (double.TryParse(val, out _))
            {
                type = ExecParameterType.Number;
            }
            else
            {
                type = ExecParameterType.Text;
            }

            Parameters.Add(new ExecParameter
            {
                Name = $"param{i + 1}",
                Value = val,
                Type = type
            });
        }
    }

    private List<string> SplitParameters(string input)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in input)
        {
            if (c == '\'')
                inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString().Trim());
                sb.Clear();
                continue;
            }
            sb.Append(c);
        }
        if (sb.Length > 0)
            result.Add(sb.ToString().Trim());

        return result;
    }


}

public enum ExecParameterType
{
    Number,
    Text
}
public enum RodzajProcedury
{
    Ręczna,
    Auto
}

public class ExecParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
    public ExecParameterType Type { get; set; }

    public object GetTypedValue()
    {
        return Type switch
        {
            ExecParameterType.Number => double.TryParse(Value, out var num) ? num : 0,
            ExecParameterType.Text => Value,
            _ => Value
        };
    }
}
