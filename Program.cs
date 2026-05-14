using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI 3.1 by default in .NET 10
builder.Services.AddOpenApi();

// Built-in validation - NEW in .NET 10 for Minimal APIs
builder.Services.AddValidation();

var app = builder.Build();

// OpenAPI available in all environments (needed for Railway + N8N)
app.MapOpenApi();

var orders = new List<Order>();

app.MapGet("/orders", () => orders)
    .WithName("GetOrders")
    .WithSummary("Get all orders");

app.MapPost("/orders", (Order order) =>
{
    order.Id = orders.Count + 1;
    orders.Add(order);
    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("CreateOrder")
.WithSummary("Create a new order");

app.MapGet("/orders/{id}", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    return order is null ? Results.NotFound() : Results.Ok(order);
})
.WithName("GetOrderById")
.WithSummary("Get order by ID");

app.Run();

public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000")]
    public decimal Price { get; set; }

    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}
