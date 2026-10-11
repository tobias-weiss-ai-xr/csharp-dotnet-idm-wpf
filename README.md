# C# .NET + Keycloak + Coordinate Optimization Demo

**Dockerized .NET demo that combines:**
- Authentication via **Keycloak OIDC** (OpenID Connect)
- **Coordinate measurement path optimization** (greedy near-neighbor, like MiCAT Planner)
- Domain: measurement point sorting → minimal travel distance for CMM (Coordinate Measuring Machine)

## Start

```bash
cd csharp-dotnet-idm-wpf
docker compose up
```

- Keycloak Admin UI:  http://localhost:8081   (admin / admin)
- Demo API:          http://localhost:8080   (Swagger UI at /swagger)

## Test

```bash
# Optimize measurement path (public for demo)
curl -s http://localhost:8080/coords/optimize \
  -X POST -H "Content-Type: application/json" \
  -d '{"points":[[0,0],[1,3],[4,2],[3,5]]}' | jq

# -> returns ordered points + totalDistance

# Secured endpoint (needs Keycloak token)
curl -s http://localhost:8080/sec/hello -H "Authorization: Bearer <TOKEN>"
```

## Setup Keycloak Realm (one-time)

1. Open: http://localhost:8081 → Admin Console → login (admin/admin)
2. Hover "Master" → Add realm → **Name: `measure`** → Create
3. Clients → Create → Client ID: `coord-service` → Client Protocol: `openid-connect`
4. Settings: Access Type = `confidential`, Valid Redirect URIs = `http://localhost:8080/*`
5. Save → Credentials → copy **Client Secret** → set in app if needed (dev uses standard JWKS)
6. Users → Add user → Credentials → set password, disable "Temporary"
7. Client Scopes: add `profile`, `email` → Assign to client

Then `docker compose restart coordsvc` picks up the realm.

## Build locally

```bash
cd src/CoordService
dotnet run  # port 8080 or 5001
```

With Keycloak at http://localhost:8081/realms/measure, the JwtBearer auth validates.
