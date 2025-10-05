using System.Data;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public sealed class NpgsqlConnectionFactory : IConnectionFactory {
    private readonly string? _connectionString;
    
    public NpgsqlConnectionFactory(IConfiguration cfg) {
        _connectionString = cfg.GetConnectionString("DefaultConnection");
    }
    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}