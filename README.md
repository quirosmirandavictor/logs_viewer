# logs_viewer

# 🚀 Azure Log Processing Pipeline

[![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Azure Functions](https://img.shields.io/badge/Azure-Functions-0062AD?logo=azure-functions\&logoColor=white)](https://azure.microsoft.com/products/functions)
[![Azure Storage](https://img.shields.io/badge/Azure-Storage-0078D4?logo=microsoftazure\&logoColor=white)](https://azure.microsoft.com/products/storage)
[![Azurite](https://img.shields.io/badge/Azurite-Local%20Storage-blue)](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

# 📖 Overview

This project demonstrates a complete **event-driven log processing pipeline** built with Azure technologies.

The solution simulates a production environment where application logs are generated, queued, processed asynchronously, and stored for later analysis.

Although it runs locally using **Azurite**, the architecture is designed to be easily migrated to Azure with minimal configuration changes.

This repository was created as part of my journey toward becoming an **Azure Solutions Architect**, showcasing cloud-native development practices and serverless architecture.

---

# 🎯 Objectives

* Demonstrate event-driven architecture.
* Practice Azure Functions development.
* Simulate Azure Storage locally using Azurite.
* Process logs asynchronously.
* Store structured log data.
* Build a production-ready portfolio project.

---

# 🏗 Architecture

```mermaid
graph TB

    %% ==========================================
    %% SYSTEM BOUNDARY
    %% ==========================================
    subgraph PLATFORM["LOGS VIEWER PLATFORM (LOCAL ENVIRONMENT)"]
        direction TB

        %% =====================
        %% 1. PRODUCER LAYER
        %% =====================
        subgraph INPUTS ["Log Generators (Internal)"]
            direction LR
            LN["#128196; NLog Source"]
            LP["#128196; Python Log Source"]
        end

        WORKER(["fa:fa-gears (WORKER SERVICE)
        ---
        [Docker Container]
        Logs Aggregator"])

        %% =====================
        %% 2. MESSAGING LAYER
        %% =====================
        QUEUE(["fa:fa-envelope (AZURE QUEUE STORAGE)
        ---
        Queue: logsqueue
        [Azurite Emulator]"])

        %% =====================
        %% 3. PROCESSING LAYER
        %% =====================
        FUNCTION(["fa:fa-bolt (AZURE FUNCTION APP)
        ---
        Queue Trigger
        Event Processor"])

        %% =====================
        %% 4. STORAGE LAYER
        %% =====================
        TABLE_STORAGE[("(AZURE TABLE STORAGE)
        ---
        Entity Store
        NoSQL Structured Logs")]

    end

    %% ==========================================
    %% FLOWS (STRAIGHT LINES)
    %% ==========================================
    LN --> WORKER
    LP --> WORKER
    
    WORKER -->|1. Pushes LogEvent JSON| QUEUE
    QUEUE -->|2. Event Polled| FUNCTION
    FUNCTION -->|3. Persists Entities| TABLE_STORAGE

    %% ==========================================
    %% STYLING DEFINITIONS
    %% ==========================================
    classDef compute fill:#ffecd2,stroke:#ff9248,stroke-width:1.5px,color:#000;
    classDef infra fill:#d0f0ff,stroke:#00a3ff,stroke-width:1.5px,color:#000;
    classDef storage fill:#d3ffd8,stroke:#00ba2d,stroke-width:1.5px,color:#000;
    classDef logInput fill:#f8f9fa,stroke:#b8b8b8,stroke-width:1px,stroke-dasharray: 4,color:#6c757d;

    class WORKER compute;
    class QUEUE,FUNCTION infra;
    class TABLE_STORAGE storage;
    class LN,LP logInput;

```
## C1 Context Diagram

<p align="center">
    <img src="docs/diagrams/images/c1-context.png" width="70%">
</p>

## C2 Container Diagram

<p align="center">
    <img src="docs/diagrams/images/c2-container.png" width="100%">
</p>

## C3 LogWorkerMaker Diagram

<p align="center">
    <img src="docs/diagrams/images/c3-worker.png" width="100%">
</p>

## C3 App Function Diagram

<p align="center">
    <img src="docs/diagrams/images/c3-appfunction.png" width="100%">
</p>

---

# ⚙️ Technologies

| Technology             | Purpose                      |
| ---------------------- | ---------------------------- |
| .NET 8                 | Application Platform         |
| Azure Functions        | Event Processing             |
| Azure Queue Storage    | Message Queue                |
| Azure Table Storage    | Structured Storage           |
| Azurite                | Local Azure Storage Emulator |
| NLog                   | Logging Framework            |
| Docker                 | Local Infrastructure         |
| Visual Studio 2026     | Development                  |
| Visual Studio Code     | Development                  |
| Azure Storage Explorer | Storage Inspection           |

---

# 📂 Solution Structure



---

## 🔄 Processing Flow

```mermaid
flowchart LR

subgraph Client
    A[🖥️ Application]
    B[NLog]
    A --> B
end

subgraph Messaging
    C[📨 Azure Queue Storage]
end

subgraph Processing
    D[⚡ Azure Function]
    E[Deserialize]
    F[Parse]
    G[Validate]
    D --> E --> F --> G
end

subgraph Storage
    H[(Azure Table Storage)]
end

B --> C
C --> D
G --> H
```

---

# ✨ Features

* Event-driven processing
* Asynchronous architecture
* Local Azure Storage simulation
* Queue Trigger Azure Functions
* Structured log persistence
* Dependency Injection
* Configuration via appsettings.json
* Ready for Azure deployment
* Easily extensible

---

# 🧠 Architecture Decisions

## Why Queue Storage?

Using queues decouples the producer from the consumer.

Benefits:

* Higher scalability
* Improved resilience
* Retry capabilities
* Better fault tolerance

---

## Why Azure Functions?

Azure Functions provide:

* Serverless execution
* Automatic scaling
* Pay-per-execution model
* Minimal infrastructure management

---

## Why Table Storage?

Structured logs do not require relational joins.

Table Storage offers:

* Low cost
* High performance
* Excellent scalability
* Simple querying
