using OrderProcessor.Models;

namespace OrderProcessor;

class Program
{
    static void Main(string[] args)
    {
        var inventoryService = new InventoryService();
        var notificationService = new EmailNotificationService();
        var orderProcessor = new Models.OrderProcessor(inventoryService, notificationService);

        orderProcessor.ApplyDiscount(0.1m); // Установка скидки 10%

        if (orderProcessor.CanProcessOrder("item123", 2))
        {
            decimal totalPrice = orderProcessor.CalculateTotalPrice(100, 2); // 100 ₽ x 2 = 200 ₽ → 180 ₽ со скидкой
            string orderId = orderProcessor.PlaceOrder("user@example.com", "item123", 2);
            Console.WriteLine($"Order placed! ID: {orderId}, Total: {totalPrice} ₽");
        }
    }
}