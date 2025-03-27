using OrderProcessor.Abstractions;

namespace OrderProcessor.Models;

public class EmailNotificationService : INotificationService
{
    public void SendOrderConfirmation(string customerEmail, string orderId) 
        => Console.WriteLine($"Sent confirmation to {customerEmail}, Order ID: {orderId}");
}