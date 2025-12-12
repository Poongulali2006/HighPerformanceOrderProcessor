using System.Text.Json;

namespace OrderProcessorService.Services
{
    public class OrderProcessor
    {
        private readonly DatabaseService _db;
        private readonly ILogger<OrderProcessor> _logger;

        public OrderProcessor(DatabaseService db, ILogger<OrderProcessor> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task ProcessFile(string path)
        {
            try
            {
                string json = await File.ReadAllTextAsync(path);
                Order? order = JsonSerializer.Deserialize<Order>(json);

                if (order == null)
                {
                    _db.SaveInvalid(json, "Could not parse JSON");
                    return;
                }

                // Validation
                if (order.TotalAmount < 0)
                {
                    _db.SaveInvalid(json, "TotalAmount < 0");
                    return;
                }

                if (string.IsNullOrWhiteSpace(order.CustomerName))
                {
                    _db.SaveInvalid(json, "CustomerName is missing");
                    return;
                }

                // Business rule
                order.IsHighValue = order.TotalAmount > 1000;

                _db.SaveValid(order);

                _logger.LogInformation("Order saved: {id}", order.OrderId);
            }
            catch (Exception ex)
            {
                _db.SaveInvalid(File.ReadAllText(path), ex.Message);
                _logger.LogError(ex, "Error processing file");
            }
        }
    }
}
