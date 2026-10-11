using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Keycloak OIDC setup (from environment variables)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Authority = Environment.GetEnvironmentVariable("KEYCLOAK_URL") ?? "http://localhost:8081/realms/measure";
        opt.RequireHttpsMetadata = false;  // for local dev; prod: true
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("KEYCLOAK_AUDIENCE") ?? "coord-service",
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("KEYCLOAK_ISSUER") ?? "http://localhost:8081/realms/measure"
        };
        opt.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                // Extract claims for demo
                var user = ctx.Principal?.Identity?.Name ?? "unknown";
                Console.WriteLine($"[Auth] OK user: {user}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "MiCAT-like Coord Demo", Version = "v1" }));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coord Demo"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Simple coordinate point
record CoordPoint(double X, double Y);
record CoordRequest(CoordPoint[] Points);
record OptimizedPath(CoordPoint[] Order, double TotalDistance);

// Greedy Nearest-Neighbor for measurement path optimization (like MiCAT)
static OptimizedPath Optimize(CoordPoint[] pts)
{
    if (pts.Length <= 1) return new OptimizedPath(pts, 0);
    var dist = new double[pts.Length, pts.Length];
    for (int i = 0; i < pts.Length; i++)
        for (int j = 0; j < pts.Length; j++)
            dist[i, j] = Math.Sqrt(Math.Pow(pts[i].X - pts[j].X, 2) + Math.Pow(pts[i].Y - pts[j].Y, 2));

    var used = new bool[pts.Length];
    var path = new List<int>();
    path.Add(0); used[0] = true;
    for (int i = 1; i < pts.Length; i++)
    {
        var last = path[^1];
        var nearest = -1; var min = double.MaxValue;
        for (int j = 0; j < pts.Length; j++)
        { if (!used[j] && dist[last, j] < min) { min = dist[last, j]; nearest = j; } }
        path.Add(nearest); used[nearest] = true;
    }
    var ordered = path.Select(idx => pts[idx]).ToArray();
    var total = 0.0;
    for (int i = 1; i < path.Count; i++) total += dist[path[i-1], path[i]];
    return new OptimizedPath(ordered, total);
}

app.MapGet("/", () => "MiCAT-like Coordinate Measurement Demo — POST /coords/optimize");

app.MapPost("/coords/optimize", (CoordRequest req) =>
{
    var opt = Optimize(req.Points);
    return Results.Ok(new { path = opt.Order, totalDistance = Math.Round(opt.TotalDistance, 3), count = req.Points.Length });
})
// Allow unauth for demo simplicity; uncomment below to require auth
//.RequireAuthorization();

app.MapGet("/sec/hello", () => "SECURED — Keycloak auth OK!")
   .RequireAuthorization();

app.Run();
"Auth providers: JwtBearer");
