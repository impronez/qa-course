namespace OrderProcessor.Abstractions;

public interface IInventoryService
{
    bool IsItemAvailable(string itemId, int quantity);
    void ReduceStock(string itemId, int quantity);
}