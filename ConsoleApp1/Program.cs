using Npgsql;

var connString = "Host=localhost;Port=5432;Username=postgres;Password=123456ball;Database=quanly";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();
Console.WriteLine("Connected successfully!");
