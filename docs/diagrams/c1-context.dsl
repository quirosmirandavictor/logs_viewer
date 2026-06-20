workspace {

    model {

        user = person "Developer"

        system = softwareSystem "Azure Log Processing Pipeline" {

            description "Processes application logs and stores them for visualization."

        }

        user -> system "Generates logs and reviews processed data"

    }

    views {

        systemContext system {

            include *

            autolayout lr

        }

        theme default

    }

}