workspace {
    model {
        appFunction = softwareSystem "App Function" "Processes log messages" {
            functionApp = container "Azure Functions App" "Function runtime" "Azure Functions v4 Isolated" {
                # Composition Root
                program = component "Program.cs" "DI setup and startup" "Composition Root"
                
                # Function Trigger
                logsQueueFunction = component "LogsQueueFunction" "Queue-triggered handler" "Function Handler"
                
                # Supporting Services
                logger = component "ILogger" "Logging service" "Logging"
                
                # DI Setup
                program -> logsQueueFunction "creates"
                program -> logger "registers"
                
                # Execution Flow
                logsQueueFunction -> logger "logs message"
            }
        }
    }
    
    views {
        component functionApp {
            title "App Function - Internal Architecture"
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
            element "Function Handler" {
                background #00cc99
                color #ffffff
                fontSize 12
            }
            element "Logging" {
                background #ffcc99
                color #000000
                fontSize 12
            }
        }
    }
}
