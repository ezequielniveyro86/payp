using Microsoft.EntityFrameworkCore;
using WebAppTOC.Data;
using WebAppTOC.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar contexto de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebAppTOC API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppTOC API v1");
    });
}

app.UseHttpsRedirection();

// Endpoint para obtener todas las billeteras
app.MapGet("/wallets", async (ApplicationDbContext db) =>
    await db.Wallets.ToListAsync())
.WithName("GetWallets")
.WithOpenApi();

// Endpoint para obtener una billetera por su ID
app.MapGet("/wallets/{id}", async (ApplicationDbContext db, int id) =>
{
    var wallet = await db.Wallets.FindAsync(id);
    
    if (wallet == null)
        return Results.NotFound($"No se encontró la billetera con ID {id}");
    
    return Results.Ok(wallet);
})
.WithName("GetWalletById")
.WithOpenApi();

// Endpoint para crear una nueva billetera
app.MapPost("/wallets", async (ApplicationDbContext db, WalletCreateDto walletDto) =>
{
    // Crear una nueva billetera con nombre y documentId
    var wallet = new Wallet
    {
        Name = walletDto.Name,
        DocumentId = walletDto.DocumentId,
        Balance = 0, // Balance inicial en cero
        CreatedAt = DateTime.Now
    };
    
    db.Wallets.Add(wallet);
    await db.SaveChangesAsync();
    return Results.Created($"/wallets/{wallet.Id}", wallet);
})
.WithName("CreateWallet")
.WithOpenApi();

// Endpoint para obtener todas las transacciones de una billetera
app.MapGet("/wallets/{walletId}/transactions", async (ApplicationDbContext db, int walletId) =>
{
    var transactions = await db.Transactions
        .Where(t => t.WalletId == walletId)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();
    
    if (transactions == null || !transactions.Any())
        return Results.NotFound($"No se encontraron transacciones para la billetera con ID {walletId}");
    
    return Results.Ok(transactions);
})
.WithName("GetWalletTransactions")
.WithOpenApi();

// Endpoint para crear una nueva transacción
app.MapPost("/wallets/{walletId}/transactions", async (ApplicationDbContext db, int walletId, Transaction transaction) =>
{
    var wallet = await db.Wallets.FindAsync(walletId);
    if (wallet == null)
        return Results.NotFound($"No se encontró la billetera con ID {walletId}");
    
    // Asignar billetera a la transacción
    transaction.WalletId = walletId;
    transaction.CreatedAt = DateTime.Now;
    
    // Actualizar balance según el tipo de transacción
    if (transaction.Type == "DEPOSIT")
    {
        wallet.Balance += transaction.Amount;
    }
    else if (transaction.Type == "WITHDRAW")
    {
        if (wallet.Balance < transaction.Amount)
            return Results.BadRequest("Saldo insuficiente para realizar esta transacción");
        
        wallet.Balance -= transaction.Amount;
    }
    
    wallet.UpdatedAt = DateTime.Now;
    
    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();
    
    return Results.Created($"/wallets/{walletId}/transactions/{transaction.Id}", transaction);
})
.WithName("CreateTransaction")
.WithOpenApi();

// Endpoint para obtener el historial de todas las transacciones
app.MapGet("/transactions", async (ApplicationDbContext db) =>
    await db.Transactions
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync())
.WithName("GetAllTransactions")
.WithOpenApi();

// Endpoint para actualizar una billetera existente
app.MapPut("/wallets/{id}", async (ApplicationDbContext db, int id, WalletUpdateDto walletDto) =>
{
    var wallet = await db.Wallets.FindAsync(id);
    
    if (wallet == null)
        return Results.NotFound($"No se encontró la billetera con ID {id}");
    
    // Actualizar solo los campos permitidos
    wallet.Name = walletDto.Name;
    wallet.UpdatedAt = DateTime.Now;
    
    await db.SaveChangesAsync();
    
    return Results.Ok(wallet);
})
.WithName("UpdateWallet")
.WithOpenApi();

app.Run();

// Crear un DTO para recibir solo los datos necesarios de la billetera
record WalletCreateDto(string Name, int DocumentId);
// DTO para actualizar una billetera
record WalletUpdateDto(string Name);
