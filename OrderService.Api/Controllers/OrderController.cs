using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderService.Api.Data;
using StackExchange.Redis;

namespace OrderService.Api.Controllers;

[Authorize]// siparişler güvende tutmak için
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderController> _logger;

    public OrderController(AppDbContext context, ILogger<OrderController> logger)
    {
        _context = context;
        _logger = logger;
    }

    //Post
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
    {
        if (request.Quantity <= 0 || string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest("Invalid input.");

        var order = new Models.Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            PaymentMethod = request.PaymentMethod,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        //detaylı log bilgisi
        _logger.LogInformation("Kullanıcı {UserId} tarafından yeni bir sipariş verildi. Sipariş ID: {OrderId}, Ürün ID: {ProductId}, Adet: {Quantity}, Ödeme Yöntemi: {PaymentMethod}",
      order.UserId, order.Id, order.ProductId, order.Quantity, order.PaymentMethod);


        return Ok(new { message = "Order placed successfully." });
    }

    //Get
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOrders(string userId, [FromServices] IConnectionMultiplexer redis)
    {
        var cache = redis.GetDatabase();
        var cacheKey = $"orders:{userId}";

        // 1. Cache'den oku
        var cached = await cache.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            Console.WriteLine("[CACHE HIT] Redis'ten döndü");
            var ordersFromCache = JsonConvert.DeserializeObject<List<Models.Order>>(cached!);
            return Ok(ordersFromCache);
        }

        // 2. DB'den getir
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();

        // 3. Cache'e yaz (2 dakika TTL)
        await cache.StringSetAsync(
            cacheKey,
            JsonConvert.SerializeObject(orders),
            TimeSpan.FromMinutes(2)
        );

        Console.WriteLine("[CACHE MISS] DB'den döndü ve cache'lendi");

        return Ok(orders);
    }

    //GetAll
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders);
    }


}
