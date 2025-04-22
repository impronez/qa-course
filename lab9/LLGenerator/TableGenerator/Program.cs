using LLGenerator.Models;
using LLGenerator.Utilities;

namespace LLGenerator;

public class Program
{
    public static Table Main()
    {
        var filePath = @"..\..\text.txt";
        var expressions = FileReader.ReadExpressionsFromFile(filePath);

        var generator = new TableGenerator(expressions);
        var table = generator.GenerateTableFromExpressions();

        table.WriteToFile(@"..\..\out.csv");

        return table;
    }
}