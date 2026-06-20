workspace {
    model {
        logWorker = softwareSystem "Log Worker Maker" "Generates and publishes logs to Azure Queue" {
            workerService = container "Worker Service" "BackgroundService" "C#/.NET 10" {
                # Composition Root
                program = component "Program.cs" "DI setup and startup" "Composition Root"
                
                # Main Service
                worker = component "Worker" "Periodic background task executor" "BackgroundService"
                
                # Business Logic
                nlogGenerator = component "NlogFormatGenerator" "Generates and processes logs" "Business Logic"
                
                # Infrastructure
                logFileReader = component "LogFileReader" "Reads local log files" "Data Access"
                queuePublisher = component "QueuePublisher" "Publishes to Azure Queue" "Data Access"
                
                # DI Setup
                program -> worker "creates"
                program -> nlogGenerator "registers"
                program -> logFileReader "registers"
                program -> queuePublisher "registers"
                
                # Execution Flow
                worker -> nlogGenerator "executes"
                nlogGenerator -> logFileReader "reads logs"
                nlogGenerator -> queuePublisher "publishes"
            }
        }
    }
    
    views {
        component workerService {
            title "Log Worker Maker - Internal Architecture"
            description "Core components and dependencies"
            include *
            autolayout lr
        }
        
        styles {
            element "Component" {
                background #0099ff
                color #ffffff
                fontSize 12
                border solid
            }
            element "Composition Root" {
                background #ff9933
                color #ffffff
                fontSize 12
            }
            element "BackgroundService" {
                background #00cc99
                color #ffffff
                fontSize 12
            }
            element "Business Logic" {
                background #99ccff
                color #000000
                fontSize 12
            }
            element "Data Access" {
                background #ffcc99
                color #000000
                fontSize 12
            }
        }
    }
}
