using SQLite;
using BoostOrderApp.Models;

namespace BoostOrderApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public async Task InitAsync()
    {
        if (_database != null) return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "products.db");
        _database = new SQLiteAsyncConnection(databasePath);
        await _database.CreateTableAsync<ProductEntity>();
    }

    public async Task<List<ProductEntity>> GetProductsAsync()
    {
        await InitAsync();
        return await _database.Table<ProductEntity>().ToListAsync();
    }

    public async Task SaveProductsAsync(List<ProductEntity> products)
    {
        await InitAsync();
        await _database.DeleteAllAsync<ProductEntity>();
        await _database.InsertAllAsync(products);
    }
}