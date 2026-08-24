using Azure.Identity;
using CommunityToolkit.VectorData.InMemory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using MongoDB.Driver;
using OpenAI.VectorStores;
using SchurkoPortfolio.Core.Data;
using SchurkoPortfolio.Core.Identity;
using SchurkoPortfolio.Core.Interfaces;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;
using SchurkoPortfolio.Core.Services;
using SchurkoPortfolio.Data;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseSqlServer(connectionString));

AzureOpenAiModel openAiSettings = builder.Configuration.GetSection("AzureOpenAI").Get<AzureOpenAiModel>() ?? new AzureOpenAiModel();

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(openAiSettings.DeploymentName,
                                  openAiSettings.Endpoint,
                                  openAiSettings.ApiKey)
    .AddAzureOpenAITextEmbeddingGeneration(openAiSettings.EmbeddingDeploymentName,
                                           openAiSettings.Endpoint,
                                           openAiSettings.ApiKey);

MongoDbSettings mongoDbSettings = builder.Configuration.GetSection("CosmoDb").Get<MongoDbSettings>() ?? new MongoDbSettings();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<MongoDbSettings>(mongoDbSettings);

builder.Services.AddTransient<IMongoClient, MongoClient>(cc => new MongoClient(mongoDbSettings.ConnectionString));

builder.Services.AddScoped<IImageUploadService, ImageUploadService>();


builder.Services.AddSingleton<Microsoft.Extensions.VectorData.VectorStore, InMemoryVectorStore>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SchurkoPortfolioAuthCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddTransient<IEmailAiService, EmailAiService>(sp => new EmailAiService(sp.GetRequiredService<Kernel>()));
SmtpSetting smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSetting>() ?? new SmtpSetting();
builder.Services.AddSingleton<ISmtpEmailService, EmailService>(provider => new EmailService(smtpSettings));
builder.Services.AddTransient<IEmailRepository, EmailRepository>(s
    => new EmailRepository(s.GetRequiredService<EmailDbContext>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
