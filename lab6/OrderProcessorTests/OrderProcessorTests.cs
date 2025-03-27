using Moq;
using OrderProcessor.Abstractions;

namespace OrderProcessorTests;

public class OrderProcessorTests
{
    // CanProcessOrder
    [Fact]
    public void CanProcessOrder_WhenQuantityIsZero_ThrowsArgumentException()
    {
        // Arrange
        var quantity = 0;
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();

        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => orderProcessor.CanProcessOrder("test", quantity));
    }
    
    [Fact]
    public void CanProcessOrder_WhenQuantityIsCorrect_ReturnsTrue()
    {
        // Arrange
        var itemId = "id1";
        var quantity = 1;
        
        var inventoryServiceMock = new Mock<IInventoryService>();
        var notificationServiceMock = new Mock<INotificationService>();
        
        inventoryServiceMock
            .Setup(x => x.IsItemAvailable(itemId, quantity))
            .Returns(true);
        
        var orderProcessor = new OrderProcessor.Models.OrderProcessor(inventoryServiceMock.Object, notificationServiceMock.Object);

        // Act
        var success = orderProcessor.CanProcessOrder(itemId, quantity);
        
        // Assert
        Assert.True(success);
    }
    
    // CalculateTotalPrice
    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 0)]
    public void CalculateTotalPrice_WhenParametersIsNegative_ThrowsArgumentException(
        decimal basePrice, int quantity)
    {
        // Arrange
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();

        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => orderProcessor.CalculateTotalPrice(basePrice, quantity));
    }
    
    [Fact]
    public void CalculateTotalPrice_WithDiscount_ReturnsTotalPrice()
    {
        // Arrange
        var basePrice = 1m;
        var quantity = 5;
        var discountRate = 0.2m;
        var expectedResult = 4m;
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        orderProcessor.DiscountRate = discountRate;
        
        // Act
        var totalPrice = orderProcessor.CalculateTotalPrice(basePrice, quantity);

        // Assert
        Assert.Equal(totalPrice, expectedResult);
    }
    
    // PlaceOrder
    [Fact]
    public void PlaceOrder_WhenItemIsNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        var quantity = 5;

        // Act
        
        // Assert
        Assert.Throws<InvalidOperationException>(() => orderProcessor.PlaceOrder("", "", quantity));
    }

    [Fact]
    public void PlaceOrder_WhenParametersIsValid_ReturnsOrderIdAndReducesStock()
    {
        // Arrange
        var itemId = "id1";
        var quantity = 5;
        
        var inventoryServiceMock = new Mock<IInventoryService>();
        var notificationServiceMock = new Mock<INotificationService>();
        
        inventoryServiceMock
            .Setup(x => x.IsItemAvailable(itemId, quantity))
            .Returns(true);
        
        var orderProcessor = new OrderProcessor.Models.OrderProcessor(inventoryServiceMock.Object, notificationServiceMock.Object);
        
        // Act
        var orderId = orderProcessor.PlaceOrder("", itemId, quantity);
        
        // Assert
        Assert.True(Guid.TryParse(orderId, out Guid _));
    }
    
    [Fact]
    public void PlaceOrder_CallsNotificationServiceWithCorrectParameters()
    {
        // Arrange
        var email = "test@test.test";
        var itemId = "id1";
        var quantity = 5;
        
        var inventoryServiceMock = new Mock<IInventoryService>();
        var notificationServiceMock = new Mock<INotificationService>();
        
        
        inventoryServiceMock
            .Setup(x => x.IsItemAvailable(itemId, quantity))
            .Returns(true);
        
        var orderProcessor = new OrderProcessor.Models.OrderProcessor(inventoryServiceMock.Object, notificationServiceMock.Object);
        
        // Act
        var orderId = orderProcessor.PlaceOrder(email, itemId, quantity);
        
        // Assert
        Assert.NotNull(orderId);
        notificationServiceMock.Verify(x => x.SendOrderConfirmation(email, It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public void PlaceOrder_CallsInventoryServiceWithCorrectParameters()
    {
        // Arrange
        var email = "test@test.test";
        var itemId = "id1";
        var quantity = 5;
        
        var inventoryServiceMock = new Mock<IInventoryService>();
        var notificationServiceMock = new Mock<INotificationService>();
        
        inventoryServiceMock
            .Setup(x => x.IsItemAvailable(itemId, quantity))
            .Returns(true);
        
        var orderProcessor = new OrderProcessor.Models.OrderProcessor(inventoryServiceMock.Object, notificationServiceMock.Object);
        
        // Act
        var orderId = orderProcessor.PlaceOrder(email, itemId, quantity);
        
        // Assert
        Assert.NotNull(orderId);
        inventoryServiceMock.Verify(x => x.ReduceStock(itemId, quantity), Times.Once);
    }
    
    // UpdateStock
    [Fact]
    public void UpdateStock_WhenNewStockIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var itemId = "id1";
        var newStock = -1;
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => orderProcessor.UpdateStock(itemId, newStock));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void UpdateStock_WhenNewStockIsValid_Successful(
        int newStock)
    {
        // Arrange
        var itemId = "id1";
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        
        // Act
        orderProcessor.UpdateStock(itemId, newStock);
        
        // Assert
        Assert.Equal(orderProcessor.AvailableStock[itemId], newStock);
    }
    
    // ApplyDiscount
    [Theory]
    [InlineData(-1)]
    [InlineData(0.6)]
    public void ApplyDiscount_WhenDiscountIsInvalid_ThrowsArgumentException(
        double discountRate)
    {
        // Arrange
        var rate = Convert.ToDecimal(discountRate);
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => orderProcessor.ApplyDiscount(rate));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    public void ApplyDiscount_WhenDiscountInValidRange_Successful(
        double discountRate)
    {
        // Arrange
        var rate = Convert.ToDecimal(discountRate);
        
        var orderProcessor = CreateOrderProcessorWithEmptyMocks();
        
        // Act
        orderProcessor.ApplyDiscount(rate);
        
        // Assert
        Assert.Equal(orderProcessor.DiscountRate, rate);
    }

    private static OrderProcessor.Models.OrderProcessor CreateOrderProcessorWithEmptyMocks()
    {
        return new OrderProcessor.Models.OrderProcessor(
            new Mock<IInventoryService>().Object, new Mock<INotificationService>().Object);
    }
}