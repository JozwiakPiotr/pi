# Konspekt pracy inżynierskiej

## Cel pracy inżynierskiej
Głównym celem pracy jest realizacja aplikacji opartej na technologii Blazor, która będzie hostowana na platformie OpenShift. Cel ten obejmuje również integrację systemu uwierzytelniania użytkowników z wykorzystaniem domeny Active Directory oraz kompleksowe zadbanie o konfigurację środowiska, jego automatyzację, bezpieczeństwo i monitoring.

## Wyzwanie inżynierskie
Wyzwanie inżynierskie polega na zaprojektowaniu i wdrożeniu systemu działającego w środowisku kontenerowym OpenShift. Wymaga to nie tylko stworzenia samej aplikacji w technologii Blazor, ale przede wszystkim poprawnej konfiguracji klastra, zdefiniowania odpowiednich zasobów i polityk bezpieczeństwa. Dodatkowym wyzwaniem jest integracja aplikacji z zewnętrzną usługą Active Directory oraz skonfigurowanie procesów CI/CD i systemu monitorującego to środowisko.

## Sposób osiągnięcia celu

- Analiza i projekt: Zebranie i opracowanie wymagań funkcjonalnych dla tworzonej aplikacji oraz zaprojektowanie docelowej architektury dla platformy OpenShift.
- Spike'i technologiczne: Seria eksperymentów (proof-of-concept) mających na celu wybór optymalnego rozwiązania Identity Provider do integracji AD z OpenShift. Kandydaci: Keycloak, Duende IdentityServer, AD FS. Kryteria oceny: łatwość integracji z Blazor, wsparcie dla OIDC/OAuth2, możliwość mapowania grup AD na role, licencjonowanie, dokumentacja.
- Implementacja i środowisko: Zaprogramowanie aplikacji oraz przygotowanie klastra OpenShift, w tym skonfigurowanie definicji zasobów, wdrożenie polityk bezpieczeństwa i uruchomienie monitoringu.
- Integracja i automatyzacja: Połączenie stworzonego rozwiązania z usługą Active Directory w celu uwierzytelniania oraz skonfigurowanie zautomatyzowanych procesów ciągłej integracji i dostarczania (CI/CD).
- Weryfikacja i testy: testy funkcjonalne oraz praktyczne sprawdzenie działania wdrożonego monitoringu.

## Spodziewane rezultaty

### Scenariusze użycia systemu
1. **Administrator platformy** - zarządzanie klastrem OpenShift przez panel webowy z logowaniem AD, kontrola zasobów i uprawnień użytkowników
2. **Programista** - uproszczenie konfiguracji autoryzacji, tak aby integracja z AD nie zwiększała kosztów developmentu ani utrzymania aplikacji
3. **Użytkownik końcowy** - logowanie do aplikacji Blazor przez AD (min: formularz, max: SSO), dostęp zgodny z przypisanymi rolami
4. **Onboarding pracownika** - dodanie do grupy AD automatycznie nadaje odpowiednie scope'y i dostęp do zasobów (RBAC mapowany z AD)

### wariant minimum
Działająca aplikacja Blazor (monolit) z uwierzytelnianiem użytkowników przez Active Directory (LDAP), wdrażana manualnie na klastrze OpenShift. Podstawowa konfiguracja środowiska z wykorzystaniem standardowych zasobów Kubernetes (Deployment, Service). Przeprowadzone spike'i technologiczne z udokumentowanym porównaniem rozwiązań Identity Provider. Dokumentacja procesu instalacji i konfiguracji.

### wariant min-max
Działająca aplikacja Blazor (monolit) z uwierzytelnianiem AD przez wybrany Identity Provider (OIDC), hostowana na OpenShift z wdrożonymi politykami bezpieczeństwa (RBAC, Network Policies, SecurityContextConstraints). Mapowanie grup AD na role aplikacji. Zautomatyzowany proces CI/CD. Podstawowy monitoring z wykorzystaniem wbudowanych mechanizmów OpenShift (Prometheus, health checks). Szablon projektu umożliwiający programistom tworzenie nowych serwisów z gotową integracją auth. Testy funkcjonalne potwierdzające poprawność działania.

### wariant maksimum
Architektura mikrousług: aplikacja Blazor jako frontend + wydzielone serwisy backendowe, komunikacja przez API Gateway z centralnym uwierzytelnianiem. Pełne SSO (Single Sign-On) - użytkownik zalogowany do domeny Windows uzyskuje automatyczny dostęp bez ponownego wpisywania hasła. Platforma self-service: programista tworzy nowy mikroserwis z szablonu i ma auth "za darmo" (zero konfiguracji OIDC). W pełni zautomatyzowane środowisko OpenShift z zaawansowaną orkiestracją. Kompleksowy pipeline CI/CD z automatycznymi testami, skanowaniem bezpieczeństwa obrazów (np. Trivy, Clair) i promocją między środowiskami. Rozbudowany system monitoringu (Grafana dashboardy, alerty) i centralne logowanie. Infrastruktura jako kod (IaC) z wykorzystaniem Helm/Kustomize. Dokumentacja architektury, runbooki operacyjne oraz przeprowadzone testy penetracyjne/bezpieczeństwa.
