namespace LLGenerator.Models;

public class Table
{
    public List<Row> Rows { get; private set; } = [];

    public void WriteToFile(string path)
    {
        using var writer = new StreamWriter(path);
        
        writer.WriteLine("№;Символ;Переход;Сдвиг;Н. мн-во;Ошибка;Стек;Конец");

        foreach (var row in Rows)
        {
            var rowString = $"{row.Number};{row.Value};{row.Transition};{row.HasShift};{string.Join(" ", row.GuidingSet)};{row.HasError}; {row.Stack};{row.IsEnd}";
            writer.WriteLine(rowString);
        }
        
        writer.Close();
    }
}