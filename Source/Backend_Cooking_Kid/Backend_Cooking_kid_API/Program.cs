using Backend_Cooking_Kid_BusinessLogic.Services;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Backend_Cooking_Kid_DataAccess.ValidateConverts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnect");
// Add services to the container.
//kết nối DB
builder.Services.AddDbContext<CookingKidContext>(options =>
{
	options.UseSqlServer(connectionString)
	.LogTo(Console.WriteLine , LogLevel.Information);
});
// Cấu hình CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll" ,
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});
// Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1" , new OpenApiInfo { Title = "Sinco API" , Version = "v1" });
	c.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"" ,
		Name = "Authorization" ,
		In = ParameterLocation.Header ,
		Type = SecuritySchemeType.ApiKey ,
		Scheme = "Bearer"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

//Scope
builder.Services.AddScoped(typeof(IBaseRepository<>) , typeof(BaseRepository<>));
builder.Services.AddScoped<IFileService , FileService>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
