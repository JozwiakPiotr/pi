workspace "Hot-Desk System" "Architektura aplikacji do rezerwacji biurek realizowanej w ramach pracy inżynierskiej (C4)." {

    model {
        // Aktorzy
        employee = person "Employee" "Pracownik firmy przeglądający i rezerwujący biurka oraz zgłaszający awarie sprzętu." "User"
        maintainer = person "Maintainer" "Technik konserwator obsługujący zgłoszenia awarii biurek i wyposażenia." "User"
        admin = person "Platform Admin" "Administrator zarządzający uprawnieniami, klastrem i systemem w domenie." "Admin"

        // Systemy Zewnętrzne
        ad = softwareSystem "Active Directory / Identity Provider" "System uwierzytelniania i autoryzacji (np. Keycloak, Duende IdentityServer)." "External System"

        // Główny System
        hotdesk = softwareSystem "System Hot-Desk" "Aplikacja do rezerwacji biurek w biurze." "Target System" {
            
            // Frontend
            spa = container "Blazor App (HotDeskApp)" "Główny interfejs użytkownika (SPA)." "Blazor WebAssembly / Server" "Web Browser"
            
            // Mikrousługi
            api_booking = container "Booking Service" "Zarządza logiką rezerwacji biurek na dany czas." ".NET / C#" "Microservice"
            api_inventory = container "Inventory Service" "Zarządza strukturą biura (pokoje, biurka, przypisany sprzęt i statusy)." ".NET / C#" "Microservice"
            api_maintenance = container "Maintenance Service" "Obsługuje cykl życia zgłoszeń technicznych i awarii." ".NET / C#" "Microservice"
            api_analytics = container "Analytics Service" "Gromadzi dane i dostarcza statystyki użycia biura." ".NET / C#" "Microservice"
            
            // Szyna danych (Message Broker)
            msg_broker = container "Message Broker" "Zapewnia asynchroniczną komunikację opartą na zdarzeniach." "RabbitMQ" "Message Broker"
            
            // Bazy Danych
            db_booking = container "Booking DB" "Przechowuje dane rezerwacji." "Relacyjna Baza Danych" "Database"
            db_inventory = container "Inventory DB" "Przechowuje schemat ERD: Pokoje, Biurka, Wyposażenie." "Relacyjna Baza Danych" "Database"
            db_maintenance = container "Maintenance DB" "Przechowuje tickety serwisowe i ich statusy." "Relacyjna Baza Danych" "Database"
        }

        // Relacje: Użytkownicy -> Aplikacja / Zewnętrzne systemy
        employee -> spa "Przegląda i rezerwuje biurka, zgłasza awarie" "HTTPS"
        maintainer -> spa "Rozwiązuje tickety serwisowe" "HTTPS"
        admin -> ad "Zarządza użytkownikami i grupami (RBAC)" "HTTPS"

        // Relacje: Integracja z AD
        spa -> ad "Uwierzytelnia użytkownika i pobiera tokeny (SSO/OIDC)" "HTTPS"

        // Relacje: Frontend -> Mikrousługi (API)
        spa -> api_booking "Odpytuje o dostępne biurka, dokonuje rezerwacji" "REST/HTTPS"
        spa -> api_inventory "Pobiera szczegóły i atrybuty sprzętowe biurek" "REST/HTTPS"
        spa -> api_maintenance "Zgłasza awarie, pobiera listę zgłoszeń (Ticketów)" "REST/HTTPS"
        spa -> api_analytics "Pobiera logi i statystyki wykorzystania biura" "REST/HTTPS"

        // Relacje: Mikrousługi -> Bazy danych
        api_booking -> db_booking "Odczytuje i zapisuje rezerwacje" "TCP"
        api_inventory -> db_inventory "Odczytuje i zapisuje strukturę biura (ERD)" "TCP"
        api_maintenance -> db_maintenance "Odczytuje i zapisuje cykl życia zgłoszeń" "TCP"

        // Relacje: Event-Driven (Publikowanie do RabbitMQ)
        api_inventory -> msg_broker "Publikuje zdarzenia (np. Desk added, Desk state changed)" "AMQP"
        api_maintenance -> msg_broker "Publikuje zdarzenia (np. Issue resolved)" "AMQP"
        
        // Relacje: Event-Driven (Subskrypcja z RabbitMQ)
        api_booking -> msg_broker "Subskrybuje zdarzenia o biurkach (aktualizuje dostępność)" "AMQP"
        api_maintenance -> msg_broker "Subskrybuje zdarzenia o dodaniu nowego sprzętu" "AMQP"
        api_inventory -> msg_broker "Subskrybuje zdarzenia o rozwiązaniu awarii" "AMQP"
    }

    views {
        // Widok Kontekstu (Poziom 1)
        systemContext hotdesk "SystemContext" {
            include *
            autoLayout
        }

        // Widok Kontenerów (Poziom 2)
        container hotdesk "Containers" {
            include *
            autoLayout
        }

        // Style i Kolory
        styles {
            element "Person" {
                shape Person
                background #08427b
                color #ffffff
            }
            element "External System" {
                background #999999
                color #ffffff
            }
            element "Target System" {
                background #1168bd
                color #ffffff
            }
            element "Microservice" {
                shape Hexagon
            }
            element "Web Browser" {
                shape WebBrowser
            }
            element "Database" {
                shape Cylinder
            }
            element "Message Broker" {
                shape Pipe
            }
        }
    }
}