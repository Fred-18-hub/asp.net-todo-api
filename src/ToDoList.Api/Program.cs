global using Microsoft.EntityFrameworkCore;
global using ToDoList.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using ToDoList.Api.Controllers;
using ToDoList.Api.Repositories.Db;
using ToDoList.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Adding Custom Interface Implementations
builder.Services.AddScoped<IUserDbRepository, UserDbRepository>();
builder.Services.AddScoped<IPhoneOtpDbRepository, PhoneOtpDbRepository>();
builder.Services.AddScoped<ITaskItemDbRepository, TaskItemDbRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IOtpCodeService, OtpCodeService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.ResolveConflictingActions(x => x.Last());  // Resolves action methods with conflicting HTTP verbs
	c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));	// Points to the xml file containing the api documentation
}
);

// Adding DbContext
builder.Services.AddDbContext<DataContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionPG"));
});

// Implementing JWT-Based Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")))
		};
	});

// Implementing API Versioning
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1, 0);
						   // = ApiVersion.Default
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ReportApiVersions = true;

	// Setting the versioning implementation method : Not Setting (= Default) = Query string params
	options.ApiVersionReader = ApiVersionReader.Combine(
		new MediaTypeApiVersionReader("version"),			// Configures to use either media type and header type for versioning
		new HeaderApiVersionReader("x-version")
	);

	// Using convention to set API versions | Uncomment to use | NOTE that you need to comment already defined versions in the controller
	//var conv = options.Conventions.Controller<TasksController>();
	//conv.HasDeprecatedApiVersion(new ApiVersion(1, 0));
	//conv.HasApiVersion(new ApiVersion(2, 0));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
