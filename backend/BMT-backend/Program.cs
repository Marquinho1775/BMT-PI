using BMT_backend.Infrastructure;
using BMT_backend.Application.Interfaces;
using BMT_backend.Application.Services;
using BMT_backend.Application.Queries;
using BMT_backend.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = "wwwroot"
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddScoped<IUserRepository>(provider =>
    new UserRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<ITagRepository>(provider =>
    new TagRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IImageFileRepository>(provider =>
    new ImageFileRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IDirectionRepository>(provider =>
    new DirectionRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IEntrepeneurRepository>(provider =>
    new EntrepeneurRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IDirectionRepository>(provider =>
    new DirectionRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<ICreditCardRepository>(provider =>
    new CreditCardRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IProductRepository>(provider =>
    new ProductRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<ICodeRepository>(provider =>
    new CodeRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IEnterpriseRepository>(provider =>
    new EnterpriseRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IShoppingCartRepository>(provider =>
    new ShoppingCartRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<IOrderRepository>(provider =>
    new OrderRepository(builder.Configuration.GetConnectionString("BMTContext")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<ImageFileService>();
builder.Services.AddScoped<DirectionService>();
builder.Services.AddScoped<EntrepeneurService>();
builder.Services.AddScoped<IImageFileService, ImageFileService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CreditCardService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<EnterpriseService>();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<SearchProductsAndEnterprisesQuery>();
builder.Services.AddScoped<GetEnterpriseEarningsQuery>();
builder.Services.AddScoped<GetAllEnterprisesEarningsQuery>();
builder.Services.AddScoped<GetSystemTotalDeliveryFeeQuery>();
builder.Services.AddScoped<GetEnterpriseWeeklyEarningsQuery>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
