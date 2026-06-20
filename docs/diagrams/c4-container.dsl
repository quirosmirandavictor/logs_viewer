workspace {

    model {

        developer = person "Developer"

        softwareSystem = softwareSystem "Log Viewer Platform" {

            worker = container "Worker Service" ".NET 8 Worker"
            queue = container "Azure Queue Storage"
            function = container "Azure Function"
            table = container "Azure Table Storage"

            worker -> queue "Publishes LogEvent JSON"
            queue -> function "Triggers"
            function -> table "Persists Log Entities"
        }

        nlog = softwareSystem "NLog Source"
        python = softwareSystem "Python Log Source"

        nlog -> worker "Writes logs"
        python -> worker "Writes logs"

        developer -> worker "Runs locally"

    }

    views {

        container softwareSystem {
            include *
            autolayout lr
        }

        theme default

    }

}