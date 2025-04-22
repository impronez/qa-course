namespace LLGenerator.Models;

public struct Expression
{
    public Data Key;
    public List<Data> Values;

    public Expression(Data key)
    {
        Key = key;
        Values = new List<Data>();
    }
}