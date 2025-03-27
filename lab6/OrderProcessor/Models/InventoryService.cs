using OrderProcessor.Abstractions;

namespace OrderProcessor.Models;

public class InventoryService : IInventoryService
{
    public bool IsItemAvailable(string itemId, int quantity) => quantity <= 100; // Заглушка
    public void ReduceStock(string itemId, int quantity) => Console.WriteLine($"Reduced stock: {itemId} x{quantity}");
}