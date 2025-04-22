using LLGenerator.Models;

namespace LLGenerator.Utilities;

public static class FileReader
{
    private const char OrOperator = '|';
    private const string Arrow = "->";
    
    public static List<Expression> ReadExpressionsFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        uint counter = 1;
        var expressions = new List<Expression>();
        
        using var streamReader = new StreamReader(filePath);
        while (!streamReader.EndOfStream)
        {
            var expression = streamReader.ReadLine();
            if (string.IsNullOrEmpty(expression))
                break;
            
            ParseExpression(expression, expressions, ref counter);
        }
        
        ReindexingExpressions(expressions, ref counter);

        return expressions;
    }

    private static void ReindexingExpressions(List<Expression> expressions, ref uint idCounter)
    {
        foreach (var exp in expressions)
        {
            for (int i = 0; i < exp.Values.Count; i++)
            {
                var value = exp.Values[i];
                value.Id = idCounter++;
                exp.Values[i] = value;
            }
        }
    }

    private static void ParseExpression(string stringExpression, List<Expression> expressions, ref uint idCounter)
    {
        var split = stringExpression.Split(Arrow);
        if (split.Length != 2)
            throw new ArgumentException($"Expression '{stringExpression}' is not valid. Example: <noterm> -> <noterm> term");

        var key = new Data(split[0], idCounter++);
        var values = split[1].Split(' ');

        var expression = new Expression(key);
        
        for (int i = 0; i < values.Length; i++)
        {
            if (string.IsNullOrEmpty(values[i]))
                continue;

            if (values[i] == OrOperator.ToString())
            {
                if (i + 1 >= values.Length)
                    throw new ArgumentException($"Invalid expression format: {stringExpression}. '{OrOperator}' operator in the end of the expression");
                
                var str = $"{key.Value} -> {string.Join(" ", values.Skip(i + 1))}";
                ParseExpression(str, expressions, ref idCounter);
                break;
            }
            
            var data = new Data(values[i], 0);
            
            expression.Values.Add(data);
        }
        
        expressions.Add(expression);
    }

    public static Table ReadTableFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        throw new NotImplementedException();
    }
}