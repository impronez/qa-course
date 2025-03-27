using OrderProcessor.Abstractions;

namespace OrderProcessor.Models;

/*
OrderProcessor, который обрабатывает заказы в интернет-магазине.
Бизнес-логика (проверка доступности товара, расчет скидок, обработка заказа)
 */

public class OrderProcessor
{
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;
    
    public Dictionary<string, int> AvailableStock { get; private set; }
    public decimal DiscountRate { get; set; } = 0;

    public OrderProcessor(
        IInventoryService inventoryService,
        INotificationService notificationService)
    {
        _inventoryService = inventoryService;
        _notificationService = notificationService;
        AvailableStock = new Dictionary<string, int>();
    }

    // 1. Проверка доступности товара
    public bool CanProcessOrder(string itemId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        return _inventoryService.IsItemAvailable(itemId, quantity);
    }

    // 2. Расчет стоимости с учетом скидки
    public decimal CalculateTotalPrice(decimal basePrice, int quantity)
    {
        if (basePrice < 0 || quantity <= 0)
            throw new ArgumentException("Invalid price or quantity");

        decimal total = basePrice * quantity;
        return total * (1 - DiscountRate);
    }

    // 3. Оформление заказа
    public string PlaceOrder(string customerEmail, string itemId, int quantity)
    {
        if (!CanProcessOrder(itemId, quantity))
            throw new InvalidOperationException("Item not available in the requested quantity");

        // Снимаем товар со склада
        _inventoryService.ReduceStock(itemId, quantity);

        // Генерируем ID заказа
        string orderId = Guid.NewGuid().ToString();

        // Отправляем уведомление
        _notificationService.SendOrderConfirmation(customerEmail, orderId);

        return orderId;
    }

    // 4. Обновление остатков (мутация данных)
    public void UpdateStock(string itemId, int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative");

        AvailableStock[itemId] = newStock;
    }

    // 5. Установка скидки (мутация данных)
    public void ApplyDiscount(decimal discountRate)
    {
        if (discountRate < 0 || discountRate > 0.5m) // Максимум 50% скидки
            throw new ArgumentException("Discount rate must be between 0% and 50%");

        DiscountRate = discountRate;
    }
}