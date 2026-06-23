# ADR-0003: Use Azure Table Storage for Log Persistence

## Status

Accepted <!-- Proposed | Accepted | Rejected | Deprecated | Superseded -->

## Date

2026-04-09

## Context

ADR-0001 established Azure Queue Storage as the messaging layer for log ingestion, and ADR-0002 established the Azure Functions Queue Trigger as the mechanism that consumes and processes those messages.

Once a log message is dequeued and processed, it must be persisted so it can be queried, audited, and retained for compliance and troubleshooting purposes. Log messages originate from multiple applications built with different technologies (.NET using NLog, Python using standard logging), so the resulting log entries do not share a perfectly uniform schema (e.g., optional fields such as exception details, correlation IDs, or custom properties may or may not be present).

The persistence layer must:

- Integrate natively with Azure Functions with minimal additional code.
- Be cost-effective at high write volume, since logs are append-heavy and rarely updated.
- Tolerate schema variability across producers without requiring rigid migrations.
- Scale automatically without manual capacity planning.

## Decision

We will use **Azure Table Storage** to persist processed log entries.

Each log entry will be stored as a table entity with a partitioning strategy based on source application and date (e.g., `PartitionKey = {AppName}_{yyyyMMdd}`, `RowKey = {ReverseTicks}_{Guid}`) to support efficient, time-bounded queries per application while keeping writes well-distributed across partitions.

Azure Table Storage was selected because it provides:

- A native output binding for Azure Functions, requiring minimal custom code from the queue-triggered function established in ADR-0002.
- A schema-less entity model that accommodates the structural differences between .NET and Python log producers.
- Very low cost per GB and per transaction, suited to high-volume, append-only log data.
- Automatic horizontal scaling without capacity or throughput provisioning.
- Fast point reads and range queries when PartitionKey/RowKey are designed around access patterns.

Compared with the evaluated alternatives, Table Storage offers the best balance of cost, simplicity, and native integration for this stage of the pipeline, without introducing the operational and pricing complexity of a fully managed database.

## Alternatives Considered

### Option A: Azure Cosmos DB (Table API or NoSQL API)

**Pros**

- API-compatible with Table Storage (Table API), easing a future migration.
- Secondary indexes and richer querying (NoSQL API).
- Global distribution and low single-digit-millisecond latency at scale.
- Native TTL support for automatic data expiration.

**Cons**

- Significantly higher cost, especially under provisioned/auto-scale throughput (RU-based pricing).
- Requires throughput and partitioning capacity planning.
- Likely over-engineered for current log volume and query needs.

### Option B: Azure Blob Storage (line-delimited JSON files)

**Pros**

- Extremely low storage cost.
- Well suited for archival and downstream batch analytics (e.g., loading into Data Lake or Synapse later).
- Simple append/rotation pattern per time window.

**Cons**

- Not optimized for point lookups or queries on individual log entries.
- Requires custom logic for file rotation, partitioning, and indexing.
- Any "search" capability requires additional tooling or batch processing.

### Option C: Azure SQL Database

**Pros**

- Rich query capabilities (SQL, joins, aggregations).
- Strong consistency and mature tooling/reporting integration.
- Familiar relational model for ad-hoc analysis.

**Cons**

- Higher operational and licensing cost relative to expected write volume.
- Requires schema design/migrations to handle heterogeneous log shapes from different producers.
- Less natural horizontal scaling for sustained high-throughput writes.
- Adds connection pooling and scaling considerations on the Functions side.

### Option D: Azure Table Storage (Selected)

**Pros**

- Native Azure Functions output binding.
- Schema-less entities accommodate varying log structures.
- Very low cost per GB/transaction.
- Automatic scaling with no capacity planning.

**Cons**

- Limited query capabilities: efficient queries only on PartitionKey/RowKey; no secondary indexes or full-text search.
- No native TTL; retention/cleanup must be implemented separately.
- No cross-partition transactions; no aggregation or join support.
- Entity size limits (1 MB per entity, 64 KB per string property) may require truncating large messages or exception stack traces.

## Consequences

### Positive

- Persists all processed log entries at low cost, consistent with the serverless, pay-per-use model adopted in ADR-0001 and ADR-0002.
- Minimal additional code in the queue-triggered function due to native output binding support.
- Schema flexibility avoids rigid migrations as new log producers or fields are introduced.
- Scales automatically with ingestion volume without manual intervention.

### Negative / Trade-offs

- Advanced querying, full-text search, or analytics use cases will likely require exporting data to a complementary service (e.g., Azure Data Explorer, Synapse, or Log Analytics) in the future.
- Retention and cleanup of old entries must be implemented explicitly (e.g., a timer-triggered Function purging old partitions), since Table Storage has no native TTL.
- Partition key design must be monitored and may need adjustment if traffic distribution across source applications becomes uneven.
- Large log messages or exception payloads may need truncation or offloading to Blob Storage to respect entity size limits.

### Items to Monitor

- Partition key access patterns and potential hot partitions.
- Storage growth and compliance with data retention requirements.
- Entity size limits, especially for verbose exceptions or stack traces.
- Throttling responses (HTTP 503) under burst write load.

## References

- Azure Table Storage documentation
- Related ADR: 001-use-queue-storage.md
- Related ADR: 002-use-azure-functions-queue-trigger.md