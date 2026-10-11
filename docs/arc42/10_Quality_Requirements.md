# 10. Quality Requirements

## 10.1 Quality Tree

```
Quality Goals
├── Security (MUST)
│   ├── Secure authentication
│   ├── Validated JWT tokens
│   └── HTTPS recommended
│
├── Functional Suitability (MUST)
│   ├── Correct path optimization
│   ├── RESTful API interface
│   └── Working Keycloak integration
│
├── Maintainability (SHOULD)
│   ├── Clean code
│   ├── Unit tests
│   └── arc42 documentation
│
├── Portability (MUST)
│   ├── Docker containers
│   └── Cross-platform support
│
├── Performance (MEDIUM)
│   ├── Greedy: < 10ms for n ≤ 50
│   ├── ACO: < 200ms for n ≤ 20, iter ≤ 50
│   └── Memory: < 50MB per request
│
└── Usability (LOW)
    ├── Swagger UI
    └── Simple API endpoints
```

## 10.2 Quality Scenarios

### QS-001: Secure Token Validation

**Scenario**: Attacker sends tampered JWT token to secured endpoint

**Expected**: Request is rejected with 401 Unauthorized

**Implementation**: `JwtBearer` middleware validates signature, issuer, audience, expiration

**Verification**: 
```bash
curl -H "Authorization: Bearer INVALID.TOKEN" http://localhost:8080/sec/hello
# Returns: 401 Unauthorized
```

### QS-002: Algorithm Correctness (Greedy NN)

**Scenario**: 4 points forming a square, start at (0,0)

**Expected**: Path stays within 10% of optimal distance

**Input**: `[[0,0], [0,1], [1,1], [1,0]]`
**Optimal**: ~4.0 (perimeter)
**Actual**: Calculated by `PathOptimizer.GreedyNn()`
**Test**: `PathTests.GreedyNn_Returns_Valid_Path()` ✅

### QS-003: Algorithm Correctness (ACO)

**Scenario**: Same 4-point square

**Expected**: After sufficient iterations, path distance ≤ optimal + 5%

**Test**: `PathTests.ACO_Returns_Valid_Path()` ✅
**Test**: `PathTests.ACO_Two_Points_Distance()` ✅ (exact match for 5.0)

### QS-004: Docker Build Reproducibility

**Scenario**: Clean environment build

**Expected**: `docker build .` completes successfully on any platform

**Verification**:
```bash
docker system prune -a
docker build -t test-coordsvc .
# Returns: Successfully built
```

### QS-005: Docker Compose Up

**Scenario**: First-time user runs demo

**Expected**: `docker compose up` starts both services without errors

**Verification**: All containers show "healthy" status

### QS-006: API Response Time

**Scenario**: 20-point optimization request

**Expected**: Response time < 100ms for Greedy, < 300ms for ACO (50 iterations)

**Test**: Manual timing with cURL + `time` command

## 10.3 Test Coverage

| Component | Tests | Coverage | Status |
|---|---|---|---|
| PathOptimizer | 2 | Method level | ✅ |
| AntColonyOptimizer | 3 | Method level | ✅ |
| Distance calculation | 1 | Full | ✅ |
| **Total** | **6** | **All algorithms** | ✅ |

```bash
# Run all tests
dotnet test tests/CoordService.Tests/
# Passed! - Failed: 0, Passed: 6, Skipped: 0
```

## 10.4 Non-Functional Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| NFR-001 | JWT token validation | MUST | ✅ |
| NFR-002 | Containerized deployment | MUST | ✅ |
| NFR-003 | REST API with JSON | MUST | ✅ |
| NFR-004 | Algorithm correctness | MUST | ✅ |
| NFR-005 | API documentation (Swagger) | SHOULD | ✅ |
| NFR-006 | Unit test coverage | SHOULD | ✅ |
| NFR-007 | Architecture documentation | SHOULD | ✅ |

## 10.5 Quality Gates

| Gate | Criteria | Status |
|---|---|---|
| Required Tests | All tests pass | ✅ Pass |
| Docker Build | Build succeeds | ✅ Pass |
| Docker Compose | Services start | ✅ Pass |
| API Health | `/` returns 200 | ✅ Pass |
| Swagger UI | `/swagger` accessible | ✅ Pass |

---

**Screenshot: Test Results**

![Test Results](screenshots/test-results.png)
*Figure: dotnet test output showing 6 passed tests*

**Screenshot: Quality Dashboard**

![Quality Dashboard](screenshots/quality-dashboard.png)
*Figure: Quality gates status overview*
