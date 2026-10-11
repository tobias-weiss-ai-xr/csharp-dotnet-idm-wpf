using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Authority = Environment.GetEnvironmentVariable("KEYCLOAK_URL") ?? "http://localhost:8081/realms/measure";
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true, ValidAudience = Environment.GetEnvironmentVariable("KEYCLOAK_AUDIENCE") ?? "coord-service",
            ValidateIssuer = true, ValidIssuer = Environment.GetEnvironmentVariable("KEYCLOAK_ISSUER") ?? "http://localhost:8081/realms/measure"
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Coord Demo", Version = "v1" }));

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo"));
app.UseHttpsRedirection(); app.UseAuthentication(); app.UseAuthorization();

app.MapGet("/", () => "POST /coords/optimize OR /coords/optimize/aco");
app.MapPost("/coords/optimize", (CoordRequest req) =>
{
    var path = PathOptimizer.GreedyNn(req.Points);
    double total = 0;
    for (int i = 0; i < path.Length - 1; i++) total += PathOptimizer.Dist(req.Points[path[i]], req.Points[path[i+1]]);
    return Results.Ok(new { algorithm = "greedy-nn", path = path.Select(idx => req.Points[idx]).ToArray(), totalDistance = Math.Round(total, 3) });
});
app.MapPost("/coords/optimize/aco", (CoordRequest req) =>
{
    var (order, dist) = AntColonyOptimizer.Optimize(req.Points, iterations: 50);
    return Results.Ok(new { algorithm = "ant-colony", path = order.Select(idx => req.Points[idx]).ToArray(), totalDistance = Math.Round(dist, 3) });
});
app.MapGet("/sec/hello", () => "SECURED — Keycloak OK!").RequireAuthorization();
app.Run();
