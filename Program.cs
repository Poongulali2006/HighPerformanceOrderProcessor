using System.Text.Json;

string folder = "IncomingOrders";
Directory.CreateDirectory(folder);

int count = args.Length > 0 ? int.Parse(args[0]) : 10;
var random = new Random();

for (int i = 0; i < count; i++)
{
    string path = Path.Combine(folder, $"order_{Guid.NewGuid()}.json");

    if (i % 7 == 0)
    {
        File.WriteAllText(path, "{bad json}");
        continue;
    }

    var order = new
    {
        OrderId = Guid.NewGuid().ToString(),
        CustomerName = (i % 5 == 0) ? "" : $"Customer{i}",
        OrderDate = DateTime.UtcNow.ToString("o"),
        Items = new[] { "Item1", "Item2" },
        TotalAmount = (i % 3 == 0) ? -100 : random.Next(10, 2000)
    };

    File.WriteAllText(path, JsonSerializer.Serialize(order));
}

Console.WriteLine($"{count} order files generated.");
