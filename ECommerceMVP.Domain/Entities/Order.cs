using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceMVP.Domain.Entities;

[BsonIgnoreExtraElements]
public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();

    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; set; }

    [BsonElement("status")]
    public OrderStatus Status { get; set; }

    [BsonElement("shippingAddress")]
    public ShippingAddress ShippingAddress { get; set; } = null!;

    [BsonElement("paymentMethod")]
    public string PaymentMethod { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

[BsonIgnoreExtraElements]
public class OrderItem
{
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }
}

[BsonIgnoreExtraElements]
public class ShippingAddress
{
    [BsonElement("street")]
    public string Street { get; set; } = null!;

    [BsonElement("city")]
    public string City { get; set; } = null!;

    [BsonElement("postalCode")]
    public string PostalCode { get; set; } = null!;

    [BsonElement("country")]
    public string Country { get; set; } = null!;
}

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}