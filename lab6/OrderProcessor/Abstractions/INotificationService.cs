namespace OrderProcessor.Abstractions;

public interface INotificationService
{
    void SendOrderConfirmation(string customerEmail, string orderId);
}