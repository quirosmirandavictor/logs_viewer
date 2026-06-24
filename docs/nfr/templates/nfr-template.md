# NFR-XXX: <Short Title>

## Category

<!-- Performance | Reliability | Scalability | Efficiency / Cost | Observability | Maintainability | Security -->

## Statement

<!-- One clear, testable sentence. Should include a number/threshold whenever possible.
Example: "95% of log messages must be persisted to Table Storage within 5 seconds of being enqueued." -->

## Rationale

<!-- Why this matters for the system. What breaks or degrades if this NFR is not met? -->

## Related ADRs

<!-- List ADRs that motivated or constrain this NFR -->
- 00X-adr-name.md

## Measurement Method

<!-- Exact procedure: what you instrument, what you run, what you sample.
Be specific enough that you (or someone else) could repeat the test in 6 months. -->

## Tooling

<!-- Scripts, SDKs, commands, dashboards used to collect the measurement.
Example: "Custom timestamp instrumentation (EnqueuedAtUtc / ProcessedAtUtc) + Table Storage query script (scripts/measure-latency.ps1)." -->

## Test Conditions

<!-- Environment details that affect comparability across runs -->
- Hosting plan: <!-- e.g., local Docker / Consumption / Premium -->
- Load profile: <!-- e.g., steady 10 msg/min, burst 500 msg in 1 min -->
- Duration: <!-- e.g., 10 minutes -->
- Host/container resources: <!-- e.g., 2 vCPU / 4GB allocated to Docker -->

## Results

| Date | Metric Value(s) | Conditions / Notes | Status |
|------|------------------|---------------------|--------|
| YYYY-MM-DD | p50 / p95 / p99 or relevant value(s) | brief context | Met / Not Met / Needs Improvement |

## Status

<!-- Current overall status, should match the latest row in Results -->
Not Measured <!-- Met | Not Met | Needs Improvement | Not Measured -->

## Follow-up Actions

<!-- Only if Status is Not Met or Needs Improvement: what will be changed before the next measurement -->
