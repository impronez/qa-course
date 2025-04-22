namespace LLGenerator.Models;

public struct Row
{
    public uint Number;
    
    public string Value;
    
    public uint? Transition;
    
    public bool HasShift;
    
    public List<string> GuidingSet;
    
    public bool HasError;

    public uint? Stack; 
    
    public bool IsEnd;
}