using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Web3Service.API.Application.Interfaces;
using Web3Service.API.Application.Services;
using Web3Service.API.Common.Errors;
using Web3Service.API.Domain.Repositories;
using Web3Service.API.Domain.Services;
using Web3Service.API.Infrastructure.Repositories;
using Web3Service.API.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();

// 配置API版本控制
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")
    );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// 配置Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Web3 Service API", Version = "v1" });
    
    // 包含XML注释
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置Problem Details


// 注册仓储
builder.Services.AddScoped<IBlockchainAccountRepository, InMemoryBlockchainAccountRepository>();
builder.Services.AddScoped<ITransactionRepository, InMemoryTransactionRepository>();
builder.Services.AddScoped<IContractDeploymentRepository, InMemoryContractDeploymentRepository>();

// 注册领域服务
builder.Services.AddScoped<IWeb3BlockchainService, NetheriumWeb3Service>();
builder.Services.AddScoped<ISmartContractService, NetheriumSmartContractService>();

// 注册应用服务
builder.Services.AddScoped<IWeb3ApplicationService, Web3ApplicationService>();

// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                $"Web3 Service API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

// 使用Problem Details中间件


app.UseHttpsRedirection();

// 使用CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

// 健康检查端点
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithTags("Health");

// 服务信息端点
app.MapGet("/info", () => new 
{ 
    Service = "Web3 Service API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
})
.WithName("ServiceInfo")
.WithTags("Info");

app.Run();

// 辅助方法
static string GetTitleForStatusCode(int statusCode)
{
    return statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized", 
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        500 => "Internal Server Error",
        503 => "Service Unavailable",
        _ => "Error"
    };
}

