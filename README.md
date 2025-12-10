# DevOpsProject

Kurzbeschreibung

Dieses Projekt implementiert eine kleine Kontaktdatenverwaltung in C# (Contact model, Validator, Repository) und enthält Unit- sowie Integrationstests (xUnit). Eine GitHub Actions Pipeline führt die Tests automatisch aus und erzeugt MTP/TRX-Testberichte als Artefakte.

Wichtige Dateien

- `src/ContactManager.App` — Model, Validator, Repository
- `tests/ContactManager.Tests` — Unit- & Integrationstests (InMemoryRepository)
- `.github/workflows/test.yml` — CI-Workflow (build, test, upload TRX)
- `SPECIFICATION.md`, `TESTPLAN.md` — Spezifikation & Testplan

Schnellstart (lokal)

```powershell
git clone https://github.com/MichiHoertenhuber/DevOpsProject.git
cd DevOpsProject
dotnet restore
dotnet build
dotnet test
```

TRX-Report lokal erzeugen:

```powershell
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

CI / GitHub Actions

- Läuft bei `push` und `pull_request` gegen `main`.
- Führt `dotnet restore`, `dotnet build` und `dotnet test` aus.
- Lädt `.trx`-Dateien als Artefakte hoch (siehe Actions → Workflow → Artifacts).

Letzte Aktualisierung: 10.12.2025