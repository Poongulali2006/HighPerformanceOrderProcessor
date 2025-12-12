using System.Data.SQLite;
using System.Text.Json;

namespace OrderProcessorService.Services
{
    public class DatabaseService
    {
        private readonly string _connection = "Data Source=orders.db";

        public DatabaseService()
        {
            using var con = new SQLiteConnection(_connection);
            con.Open();

            string valid = @"CREATE TABLE IF NOT EXISTS ValidOrders(
                OrderId TEXT PRIMARY KEY,
                CustomerName TEXT,
                OrderDate TEXT,
                Items TEXT,
                TotalAmount REAL,
                IsHighValue INTEGER
            );";

            string invalid = @"CREATE TABLE IF NOT EXISTS InvalidOrders(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                RawJson TEXT,
                Reason TEXT
            );";

            new SQLiteCommand(valid, con).ExecuteNonQuery();
            new SQLiteCommand(invalid, con).ExecuteNonQuery();
        }

        public void SaveValid(Order order)
        {
            using var con = new SQLiteConnection(_connection);
            con.Open();

            string sql = @"INSERT OR IGNORE INTO ValidOrders 
                           VALUES(@id,@name,@date,@items,@amt,@hv)";

            SQLiteCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", order.OrderId);
            cmd.Parameters.AddWithValue("@name", order.CustomerName);
            cmd.Parameters.AddWithValue("@date", order.OrderDate);
            cmd.Parameters.AddWithValue("@items", JsonSerializer.Serialize(order.Items));
            cmd.Parameters.AddWithValue("@amt", order.TotalAmount);
            cmd.Parameters.AddWithValue("@hv", order.IsHighValue ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void SaveInvalid(string raw, string reason)
        {
            using var con = new SQLiteConnection(_connection);
            con.Open();

            string sql = @"INSERT INTO InvalidOrders(RawJson, Reason) VALUES(@raw, @reason)";

            SQLiteCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@raw", raw);
            cmd.Parameters.AddWithValue("@reason", reason);
            cmd.ExecuteNonQuery();
        }
    }
}
