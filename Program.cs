using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApi;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("extract", async () =>
{
    var options = new LaunchOptions()
    {
        Headless = true,
        ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
    };
    using var browser = await Puppeteer.LaunchAsync(options);
    using var page = await browser.NewPageAsync();
    
        await page.GoToAsync("https://www.gmanetwork.com/news/archives/just_in/");
        await page.ClickAsync("#section");
        await page.WaitForNavigationAsync();
        await page.ClickAsync("#news_archives");
        await page.WaitForNavigationAsync();
        await page.ClickAsync("#news_ulatfilipino_archives");
        await page.WaitForNavigationAsync();
        var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
        var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
        return urls;
    
});

app.Run();