using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Repository;
using Business_Logic_Layer.Services;
using System.IO;
using System.Net.Http.Headers;
using webapi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  HTTP Client config for every service that sends requests to RIOT API. 
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ISummonerInfoService, SummonerInfoService>((ServiceProvider, httpClient) =>
{
    var configuration = ServiceProvider.GetRequiredService<IConfiguration>();

    var path = configuration.GetValue<string>("SummonerInfoURL");
    var apiKey = configuration.GetValue<string>("ApiKey");

    httpClient.BaseAddress = new Uri(path);
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
});
builder.Services.AddHttpClient<IMatchesService, MatchesService>((ServiceProvider, httpClient) => 
{
    var configuration = ServiceProvider.GetRequiredService<IConfiguration>();

    var path = configuration.GetValue<string>("SummonerMatchesListURL");
    var apiKey = configuration.GetValue<string>("ApiKey");

    httpClient.BaseAddress = new Uri(path);
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
});
builder.Services.AddHttpClient<IMatchDetailsService, MatchDetailsService>((Serviceprovider, httpClient) => 
{
    var configuration = Serviceprovider.GetRequiredService<IConfiguration>();

    var path = configuration.GetValue<string>("SingleMatchDetailsURL");
    var apiKey = configuration.GetValue<string>("ApiKey");

    httpClient.BaseAddress = new Uri(path);
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
});


builder.Services.AddTransient<ISummonerRepository, SummonerRepository>();

builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalHostPolicy",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173")
                  .SetIsOriginAllowedToAllowWildcardSubdomains()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });

});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.UseCors();

app.MapControllers();

app.Run();
