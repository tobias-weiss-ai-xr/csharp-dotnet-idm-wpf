# 9. Design Decisions

## 9.1 ADR List

### ADR-001: Minimal API vs Full Controllers

**Status**: ✅ Accepted

**Context**: Starting a new C# web service with simple CRUD-like endpoints.

**Decision**: Use **ASP.NET Core Minimal APIs**

**Consequences**:
- ✅ Less boilerplate code
- ✅ Faster development
- ✅ Perfect for microservices
- ⚠️ Less suitable for complex MVC applications
- ⚠️ No built-in routing conventions

**Alternatives Considered**:
- Full MVC Controllers - More structure, more files
- Razor Pages - UI-focused, not API-focused

---

### ADR-002: Keycloak vs Custom JWT

**Status**: ✅ Accepted

**Context**: Need identity management for demo/auth purposes.

**Decision**: Use **Keycloak** as external identity provider

**Consequences**:
- ✅ Industry standard OIDC/OAuth2 implementation
- ✅ Full-featured: users, roles, clients, realms
- ✅ Open source, well-maintained
- ✅ Docker image available
- ⚠️ Additional container/service to manage
- ⚠️ Slightly more complex setup

**Alternatives Considered**:
- Custom JWT generation - Less secure, more code
- IdentityServer4 - .NET focused, more complex
- Azure AD - Cloud only, vendor lock-in

---

### ADR-003: Docker for Deployment

**Status**: ✅ Accepted

**Context**: Demo should be easily runnable across different environments.

**Decision**: **Dockerize** the application

**Consequences**:
- ✅ Reproducible builds
- ✅ Cross-platform compatibility (Windows, Linux, macOS)
- ✅ Isolated dependencies
- ✅ Easy cleanup
- ⚠️ Slightly larger memory footprint
- ⚠️ Slightly slower startup

**Alternatives Considered**:
- Native .NET install - Requires SDK on host
- Virtual Machine - Too heavyweight

---

### ADR-004: ACO Algorithm Implementation

**Status**: ✅ Accepted

**Context**: Need path optimization for coordinate measurement (similar to MiCAT Planner).

**Decision**: Use **Ant Colony Optimization** alongside simpler Greedy NN

**Consequences**:
- ✅ Demonstrates metaheuristic approach
- ✅ Better solution quality for complex problems
- ✅ Related to real CMM software (MiCAT Planner)
- ⚠️ Iterative = slower than greedy
- ⚠️ Stochastic = non-deterministic results

**Algorithm Details**:
- Pheromone evaporation rate: 0.1 (10% per iteration)
- Alpha (pheromone weight): 1.0
- Beta (heuristic weight): 2.0 (prioritize closer nodes)
- Iterations: 50 (default, configurable)

**Alternatives Considered**:
- Pure Greedy NN - Faster but suboptimal
- Genetic Algorithm - Similar quality, different approach
- Simulated Annealing - Another metaheuristic
- Exact TSP solvers - Too slow for n>20

---

### ADR-005: File Naming for Top-Level Statements

**Status**: ✅ Accepted

**Context**: .NET with Top-Level Statements requires source files without type declarations to come first alphabetically.

**Decision**: Prefix non-top-level-statement files with **z_**

**Consequences**:
- ✅ Works with Top-Level Statements
- ✅ No need for explicit namespace declarations
- ✅ Simple naming convention
- ⚠️ Slightly unusual file names

**Files Affected**:
- `z_AntColonyOptimizer.cs` (contains `public static class AntColonyOptimizer`)
- `PathOptimizer.cs` (contains `public static class PathOptimizer`)
- `Types.cs` (contains `public record CoordPoint`, `public record CoordRequest`)

**Note**: `Program.cs` (Top-Level Statements) comes before all z_-prefixed files alphabetically.

---

### ADR-006: .NET 10 vs .NET 8

**Status**: ✅ Accepted

**Context**: Local SDK is .NET 10.0 (v10.0.401).

**Decision**: Target **.NET 10.0**

**Consequences**:
- ✅ Latest features and improvements
- ✅ Current SDK version
- ⚠️ Not LTS (yet)
- ⚠️ Some libraries may not be updated

**Alternatives**:
- .NET 8.0 LTS - Would require SDK downgrade

---

### ADR-007: Project Structure (No Solution File)

**Status**: ✅ Accepted

**Context**: Simple project with one service and tests.

**Decision**: **No .sln file**, use project references directly

**Consequences**:
- ✅ Simpler structure
- ✅ Faster CI/CD
- ✅ Less overhead
- ⚠️ Slightly harder IDE navigation

**Folder Structure**:
```
csharp-dotnet-idm-wpf/
├── src/CoordService/
├── tests/CoordService.Tests/
├── docs/arc42/
├── Dockerfile
└── docker-compose.yml
```

---

**Screenshot: Decision Record Template**

![ADR Template](screenshots/adr-template.png)
*Figure: Standard MADR template used for all decisions*

**Screenshot: ADR Index**

![ADR Index](screenshots/adr-index.png)
*Figure: All ADRs in architecture decisions log*
