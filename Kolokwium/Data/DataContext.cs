namespace Kolokwium.Data;

using Kolokwium.DTO;
using Kolokwium.Models;
using Kolokwium.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

public class DataContext : IDataContext
{
    private readonly ILogger<DataContext> _logger;
    private readonly IOptions<DataOptions> _options;
    
    public DataContext(ILogger<DataContext> logger, IOptions<DataOptions> options)
    {
        _logger = logger;
        _options = options;
        _logger.LogInformation("DataContext created");
    }

    /// <summary>
    /// Inicjalizuje bazę danych
    /// </summary>
    /// <param name="options"></param>
    public static async Task InitializeDb(IOptions<DataOptions> options)
    {
        await using var connection = new SqlConnection(options.Value.DbConnection);
        await connection.OpenAsync();
        
        await TryCreateDatabaseAsync(connection);
        await TryCreateProcedureAsync(connection);
    }

   

    private static async Task TryCreateDatabaseAsync(SqlConnection connection)
    {
        await using var databasesCommand = connection.CreateCommand();
        databasesCommand.CommandText = "select table_name from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE'";
        var databaseExists = false;
        
        await using (var reader = await databasesCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync() && !databaseExists)
            {
                if(reader.GetString(0) == "Book" || reader.GetString(0) == "Author" || reader.GetString(0) == "Order" || reader.GetString(0) == "Product_Warehouse")
                    databaseExists = true;
            }
        }
        if(databaseExists)
            return;
        
        var createScript = await File.ReadAllTextAsync("Data/Scripts/create.sql");
        createScript = createScript.Replace("GO", string.Empty);
        
        
        await using var command = connection.CreateCommand();
        command.CommandText = createScript;
        await command.ExecuteNonQueryAsync();
    }
    
    private static async Task TryCreateProcedureAsync(SqlConnection connection)
    {
        var procedureScript = await File.ReadAllTextAsync("Data/Scripts/proc.sql");
        
        await using var checkProcedureCommand = connection.CreateCommand();
        checkProcedureCommand.CommandText = "SELECT * FROM sysobjects WHERE id = object_id('dbo.AddProductToWarehouse')";
        var procedureExists = false;
        
        await using (var reader = await checkProcedureCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync() && !procedureExists)
                procedureExists = true;
        }

        if (procedureExists)
        {
            Console.WriteLine("Procedure already exists");
            return;
        }
        
        await using var procedureCommand = connection.CreateCommand();
        procedureCommand.CommandText = procedureScript;
        await procedureCommand.ExecuteNonQueryAsync();
    }

    /// <inheritdoc />
    public async Task<List<Book>> GetBookAsync()
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM books";
        
        await using var reader = await command.ExecuteReaderAsync();
        var books = new List<Book>();
        
        while (await reader.ReadAsync())
        {
            books.Add(new Book()
            {
                IdBook = reader.GetInt32(0),
                Title = reader.GetString(1),
            });
        }
        
        return books;
    }
    
    /// <inheritdoc />
    public async Task<Book?> FindProductAsync(int idProduct)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM books WHERE IdProduct = @IdBook";
        command.Parameters.AddWithValue("@IdBook", IdBook);
        
        await using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;
        
        return new Book()
        {
            IdBook = reader.GetInt32(0),
            Title = reader.GetString(1),
        };
    }

    /// <inheritdoc />
    public async Task<Authors?> FindAuthorsAsync(int productWarehouseIdWarehouse)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Warehouse WHERE IdWarehouse = @idWarehouse";
        command.Parameters.AddWithValue("@idWarehouse", productWarehouseIdWarehouse);
        
        await using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;
        
        return new Warehouse
        {
            IdWarehouse = reader.GetInt32(0),
            Name = reader.GetString(1),
            Address = reader.GetString(2)
        };
    }

    /// <inheritdoc />
    public async Task<Order?> FindOrderWhereAsync(Func<Order, bool> predicate)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM [Order];";
        
        await using var reader = await command.ExecuteReaderAsync();
        
        var orders = new List<Order>();

        while (await reader.ReadAsync())
        {
            orders.Add(new Order
            {
                IdOrder = reader.GetInt32(0),
                IdProduct = reader.GetInt32(1),
                Amount = reader.GetInt32(2),
                CreatedAt = reader.GetDateTime(3),
                FullfilledAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
            });
        }
        
        return orders.FirstOrDefault(predicate);
    }

    /// <inheritdoc />
    public async Task<ProductWarehouse?> FindProductWarehouseWhere(Func<ProductWarehouse, bool> predicate)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Product_Warehouse;";
        
        
        await using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;
        
        var productWarehouses = new List<ProductWarehouse>();
        
        while (await reader.ReadAsync())
        {
            productWarehouses.Add(new ProductWarehouse
            {
                IdProductWarehouse = reader.GetInt32(0),
                IdProduct = reader.GetInt32(1),
                IdWarehouse = reader.GetInt32(2),
                IdOrder = reader.GetInt32(3),
                Amount = reader.GetInt32(4),
                Price = reader.GetDecimal(5),
                CreatedAt = reader.GetDateTime(6)
            });
        }

        return productWarehouses.FirstOrDefault(predicate);
    }

    public async Task<Order> UpdateAsync(Order entity)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE [Order] SET [Order].FulfilledAt = @fullfilledAt WHERE [Order].IdOrder = @idOrder";
        command.Parameters.AddWithValue("@fullfilledAt", entity.FullfilledAt);
        command.Parameters.AddWithValue("@idOrder", entity.IdOrder);
        await command.ExecuteNonQueryAsync();
        return entity;
    }

    public async Task<ProductWarehouse> InsertAsync(ProductWarehouse newProductWarehouse)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt) VALUES (@idProduct, @idWarehouse, @idOrder, @amount, @price, @createdAt)";
        command.Parameters.AddWithValue("@idProduct", newProductWarehouse.IdProduct);
        command.Parameters.AddWithValue("@idWarehouse", newProductWarehouse.IdWarehouse);
        command.Parameters.AddWithValue("@idOrder", newProductWarehouse.IdOrder);
        command.Parameters.AddWithValue("@amount", newProductWarehouse.Amount);
        command.Parameters.AddWithValue("@price", newProductWarehouse.Price);
        command.Parameters.AddWithValue("@createdAt", newProductWarehouse.CreatedAt);
        await command.ExecuteNonQueryAsync();
        
        await using var getIdCommand = connection.CreateCommand();
        getIdCommand.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdProduct = @idProduct AND IdWarehouse = @idWarehouse AND IdOrder = @idOrder";
        getIdCommand.Parameters.AddWithValue("@idProduct", newProductWarehouse.IdProduct);
        getIdCommand.Parameters.AddWithValue("@idWarehouse", newProductWarehouse.IdWarehouse);
        getIdCommand.Parameters.AddWithValue("@idOrder", newProductWarehouse.IdOrder);
        
        newProductWarehouse.IdProductWarehouse = (int) (await getIdCommand.ExecuteScalarAsync() ?? -1);
        
        return newProductWarehouse;
    }

    /// <inheritdoc />
    public async Task<int> CreateWithProcAsync(AddProductWarehouseDTO productWarehouseDto)
    {
        await using var connection = new SqlConnection(_options.Value.DbConnection);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC AddProductToWarehouse @idProduct, @idWarehouse, @amount, @createdAt";
        command.Parameters.AddWithValue("@idProduct", productWarehouseDto.IdProduct);
        command.Parameters.AddWithValue("@idWarehouse", productWarehouseDto.IdWarehouse);
        command.Parameters.AddWithValue("@amount", productWarehouseDto.Amount);
        command.Parameters.AddWithValue("@createdAt", productWarehouseDto.CreatedAt);
        
        return (int) (await command.ExecuteScalarAsync() ?? -1);
    }
}