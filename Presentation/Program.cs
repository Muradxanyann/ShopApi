using Application;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.MappingProfiles;
using Application.Services;
using Infrastructure;
using Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ==Swagger==
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==Dependencies==
builder.Services.AddSingleton<IConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IOrderProductRepository, OrderProductRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderProductService,  OrderProductService>();



// To avoid repeatedly creating aliases
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();




app.Run();

