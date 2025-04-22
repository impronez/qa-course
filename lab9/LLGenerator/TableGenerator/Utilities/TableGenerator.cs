using LLGenerator.Models;
using Expression = LLGenerator.Models.Expression;

namespace LLGenerator.Utilities;

public class TableGenerator(List<Expression> expressions)
{
    private const string EndSymbol = "#";
    private const string EmptyTransitionSymbol = "ε";
    
    private readonly List<Expression> _expressions = expressions;
    private List<uint> _emptyGuidelineIds = [];
    
    
    public Table GenerateTableFromExpressions()
    {
        var table = new Table();

        for (int i = 0; i < _expressions.Count; i++)
        {
            table.Rows.AddRange(GetRowsFromExpression(_expressions[i], i));
        }

        ProcessEmptyGuidingSet(table);
        
        table.Rows.Sort((x, y) => x.Number.CompareTo(y.Number));

        return table;
    }

    private void ProcessEmptyGuidingSet(Table table)
    {
        SortEmptyGuidelineIds(table);
        
        var rowsDict = table.Rows.ToDictionary(r => r.Number);

        for (int i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];

            if (!_emptyGuidelineIds.Contains(row.Number))
                continue;
            
            if (row.Value == EmptyTransitionSymbol)
            {
                table.Rows[i] = row with { GuidingSet = GetGuidingSet(table, table.Rows[i].Number) };
            }
            else
            {
                var guidingSet = table.Rows
                    .Where(r => r.Value == row.Value && r.Stack.HasValue)
                    .Select(r => rowsDict.TryGetValue(r.Stack.Value, out var guideRow) ? guideRow.Value : null)
                    .Where(rowValue => rowValue != null)
                    .SelectMany(rowValue => GetGuidingSetByValue(table, rowValue!))
                    .Distinct()
                    .ToList();

                table.Rows[i] = row with { GuidingSet = guidingSet };
            }
            
            _emptyGuidelineIds.Remove(row.Number);
        }
    }

    private List<string> GetGuidingSetByValue(Table table, string value)
    {
        return table.Rows
            .Where(r => r.Value == value)
            .SelectMany(r => r.GuidingSet)
            .Where(guide => guide != EmptyTransitionSymbol)
            .Distinct()
            .ToList();
    }

    private List<string> GetGuidingSet(Table table, uint key)
    {
        var guidingSet = new List<string>();
        foreach (var row in table.Rows)
        {
            if (row.Transition == key)
                guidingSet.AddRange(row.GuidingSet);
        }
        
        return guidingSet.Distinct().ToList();
    }

    private void SortEmptyGuidelineIds(Table table)
    {
        _emptyGuidelineIds = _emptyGuidelineIds
            .OrderBy(id => 
            {
                var row = table.Rows.FirstOrDefault(r => r.Number == id);
                return row.Value == EmptyTransitionSymbol ? 1 : 0;
            })
            .ToList();
    }

    private List<Row> GetRowsFromExpression(Expression expression, int expIndex)
    {
        var rows = new List<Row>();
        
        rows.Add(GetRowByExpressionKey(expression, expIndex));
        for (int i = 0; i < expression.Values.Count; i++)
        {
            rows.Add(GetRowByExpressionElement(expression, i));
        }

        return rows;
    }

    private Row GetRowByExpressionElement(Expression exp, int dataIndex)
    {
        var data = exp.Values[dataIndex];
        uint? transition = data.IsTerm
            ? (dataIndex < exp.Values.Count - 1 ? exp.Values[dataIndex + 1].Id : null)
            : _expressions.FirstOrDefault(exp => exp.Key.Value == data.Value).Key.Id;
        
        uint? stack = (!data.IsTerm && dataIndex < exp.Values.Count - 1)
            ? exp.Values[dataIndex + 1].Id
            : null;

        var guidingSet = data.IsTerm
            ? [data.Value]
            : GetGuidingSetByData(data.Value);
        if (guidingSet.Contains(EmptyTransitionSymbol))
            _emptyGuidelineIds.Add(data.Id);
        
        var row = new Row
        {
            Number = data.Id,
            Value = data.Value,
            Transition = transition,
            HasShift = data.IsTerm,
            HasError = true,
            Stack = stack,
            IsEnd = data.Value == EndSymbol,
            GuidingSet = guidingSet
        };
        
        return row;
    }

    private List<string> GetGuidingSetByData(string key)
    {
        var guidingSet = new List<string>();
        foreach (var exp in _expressions)
        {
            if (exp.Values[0].Value == EmptyTransitionSymbol
                
                )
                continue;
            
            if (exp.Key.Value == key && exp.Values[0].IsTerm)
                guidingSet.Add(exp.Values[0].Value);
            
            if (exp.Key.Value == key && !exp.Values[0].IsTerm)
                guidingSet.AddRange(GetGuidingSetByData(exp.Values[0].Value));
        }
        
        return guidingSet.Distinct().ToList();
    }

    private List<string> GetGuidingSetByKey(Expression exp)
    {
        var data = exp.Values[0];

        var guidingSet = data.Value == EmptyTransitionSymbol
            ? [EmptyTransitionSymbol]
            : data.IsTerm
                ? [data.Value]
                : GetGuidingSetByData(exp.Values[0].Value);
            
        return guidingSet;
    }

    private Row GetRowByExpressionKey(Expression exp, int expIndex)
    {
        var hasError = expIndex + 1 >= _expressions.Count 
                       || _expressions[expIndex].Key.Value != _expressions[expIndex + 1].Key.Value;
        
        var guidingSet = GetGuidingSetByKey(exp);
        if (guidingSet.Contains(EmptyTransitionSymbol))
            _emptyGuidelineIds.Add(exp.Key.Id);
        
        var row = new Row
        {
            Number = exp.Key.Id,
            Value = exp.Key.Value,
            Transition = exp.Values[0].Id,
            HasShift = false,
            HasError = hasError,
            Stack = null,
            IsEnd = false,
            GuidingSet = guidingSet
        };

        return row;
    }
}