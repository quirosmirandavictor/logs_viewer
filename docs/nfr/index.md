# Non-Functional Requirements (NFR) Index

This index tracks the non-functional requirements defined for the **Azure Log Processing Pipeline** (LogWorkerMaker → Azure Queue Storage → AppFunction → Azure Table Storage).

Each NFR is documented in its own file under `docs/nfr/`, following the template in [`nfr-template.md`](./nfr-template.md). Measurement results are tracked inside each NFR file's `Results` table, so history accumulates over time instead of being overwritten.

## How to add a new NFR

1. Copy `nfr-template.md` to `nfr-XXX-short-name.md` (next sequential number).
2. Fill in all sections.
3. Add a row to the table below.
4. Link the ADR(s) that motivated the NFR, if any.

## NFR Catalog

| ID | Name | Category | Statement (short) | Related ADR(s) | Status |
|----|------|----------|--------------------|-----------------|--------|
| [NFR-001](./nfr-001-latency.md) | End-to-End Processing Latency | Performance | p95 enqueue→persisted < 5s | 001, 002, 003 | Not Measured |
| [NFR-002](./nfr-002-throughput.md) | Queue Throughput | Performance / Capacity | Sustain N msg/s without unbounded queue growth | 001, 002 | Not Measured |
| [NFR-003](./nfr-003-idempotency.md) | Idempotent Processing | Reliability | 0% duplicate entities under forced retries | 002, 003 | Not Measured |
| [NFR-004](./nfr-004-poison-queue-rate.md) | Poison Queue Rate | Reliability | < 0.1% messages reach poison queue under normal load | 002 | Not Measured |
| [NFR-005](./nfr-005-scalability.md) | Horizontal Scalability | Scalability | Throughput scales predictably with instance/batch settings | 002 | Not Measured |
| [NFR-006](./nfr-006-resource-efficiency.md) | Resource Efficiency | Efficiency / Cost | CPU/memory bounds per container under steady and burst load | — | Not Measured |
| [NFR-007](./nfr-007-observability.md) | Failure Detection Time | Observability | Time to detect a failure from logs alone | — | Not Measured |
| [NFR-008](./nfr-008-ci-build-time.md) | CI Build Duration | Maintainability | Clean+restore+build completes in < 2 min per project | — | Not Measured |
| [NFR-009](./nfr-009-test-coverage.md) | Test Coverage | Maintainability | ≥ 70% coverage on core business logic | — | Not Measured |

## Status Legend

- **Not Measured** — defined, no test run yet.
- **Met** — last measurement satisfied the statement.
- **Not Met** — last measurement failed the statement.
- **Needs Improvement** — partially met, or trending in the wrong direction.

## References

- ADR index: `docs/adr/index.md`
- NFR template: `docs/nfr/nfr-template.md`
