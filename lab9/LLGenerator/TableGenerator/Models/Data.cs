namespace LLGenerator.Models;

public struct Data
{
    public uint Id;
    public string Value;
    public bool IsTerm;

    public Data(string value, uint id)
    {
        Id = id;
        
        value = value.Trim();
        
        IsTerm = IsNonTerminal(value);
        Value = IsTerm ? value : value.Trim(['<', '>']);;
    }

    private static bool IsNonTerminal(string value)
    {
        return value[0] != '<' && value[^1] != '>';
    }
}