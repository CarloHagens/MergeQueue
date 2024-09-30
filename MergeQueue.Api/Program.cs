using MergeQueue.Api;
using MergeQueue.Api.Filters;
using MergeQueue.Api.Repositories;
using MergeQueue.Api.Services;
using MergeQueue.Api.Settings;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var version = "v2.0.2";

// Add services to the container.

builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

builder.Host.UseSerilog();

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

builder.Services.AddHttpClient<ISlackService, SlackService>(client =>
{
    client.BaseAddress = new Uri("https://slack.com/api/");
    var settings = builder.Configuration.GetSection(nameof(SlackApiSettings)).Get<SlackApiSettings>();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.BotToken}");

});

builder.Services.AddMemoryCache();
builder.Services.AddScoped(typeof(Microsoft.Extensions.Logging.ILogger), typeof(Logger<Program>));
builder.Services.AddScoped<AuthenticationFilter>();
builder.Services.AddScoped<IQueueLookup, QueueLookup>();
builder.Services.AddScoped<IQueueRepository, MongoDbQueueRepository>();


builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var settings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(version, new OpenApiInfo { Title = "MergeQueue", Version = version });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"MergeQueue {version}"));
}
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});
//app.UseSerilogRequestLogging(options =>
//{
//    options.EnrichDiagnosticContext = async (diagnosticsContext, httpContext) =>
//    {
//        var request = httpContext.Request;

//        request.Headers.TryGetValue("X-Slack-Request-Timestamp", out var slackRequestTimestamp);
//        request.Headers.TryGetValue("X-Slack-Signature", out var slackSignature);

//        string body;
//        using (var reader = new StreamReader(request.Body, leaveOpen: true))
//        {
//            body = await reader.ReadToEndAsync();
//            request.Body.Position = 0;
//        }

//        diagnosticsContext.Set("SlackRequestTimestamp", slackRequestTimestamp);
//        diagnosticsContext.Set("SlackSignature", slackSignature);
//        diagnosticsContext.Set("RequestBody", body);
//    };
//    options.MessageTemplate = "Method: {RequestMethod} \r\n Path: {RequestPath} \r\n Timestamp: {SlackRequestTimestamp} \r\n Signature: {SlackSignature} \r\n Body: {RequestBody} \r\n Status Code: {StatusCode} \r\n Response Time: {Elapsed}";
//});
app.UseRouting();
app.MapControllers();
app.Run();
