# AGENTS.md

Project rules for anyone (human or AI) changing this file's project. Keep this file short and factual.
Anything readable from the code does not belong here.

This file sits at the root of the `Yahtzee/` solution folder. A session opened on that folder picks it up on
its own; a session opened on the git root (`Learning_Mobile/`, which holds sibling projects) does not read
it. All paths and commands in it are relative to `Yahtzee/`, which holds `Yahtzee.slnx`, the app in the
nested `Yahtzee/` folder, and `Yahtzee.Tests/` beside it.

## What this project is

A .NET MAUI Yahtzee game, currently **half-converted** from Microsoft's "Project Tracker" MAUI sample.
The tracker (Projects / Tasks / Tags / Categories / Dashboard) still works and still ships, but it is
**temporary scaffolding and will be deleted**. The Yahtzee game is still a UI mockup: the 13 categories
score correctly in isolation (`Scoring/`), but nothing on the page is wired to them yet.

Work items and current state: see [PLAN.md](PLAN.md).

## Build and run

```bash
dotnet build Yahtzee/Yahtzee.csproj -f net10.0-windows10.0.19041.0   # primary dev target, ~35s, 0 errors
dotnet run   --project Yahtzee/Yahtzee.csproj -f net10.0-windows10.0.19041.0

dotnet test  Yahtzee.Tests/Yahtzee.Tests.csproj                     # scoring rules, plain net10.0, no -f
```

`dotnet build` with no `-f` builds Android `net10.0-android`. Android needs a running emulator/device;
Windows is the fast loop, so use Windows unless the change is platform-specific.

**Name the app project when passing `-f`.** Both projects are in `Yahtzee.slnx`, so a root-level
`dotnet build -f net10.0-windows10.0.19041.0` pushes that framework onto `Yahtzee.Tests` too and dies with
`NETSDK1005` — the test project only targets `net10.0`. Building the `.csproj` keeps it single-project.

Baseline is **0 errors and ~80 warnings** (CA1822 on page-model commands, CA1305, CsWinRT1030 from
XAML source-gen). Do not chase pre-existing warnings, and do not introduce new errors.

## Invariants — break these and the app breaks quietly

- **The SQLite schema is defined twice**: `sqlite-net` attributes on the models in `Models/`, and raw
  `CREATE TABLE IF NOT EXISTS` SQL inside each repository in `Data/Repositories/`. Changing a model
  means changing both. The two can drift without any compile error.
- **`GameState` is a single row, `ID = 1`.** It is a singleton, not a collection. Do not add rows to it.
- `DatabaseInitializationService.InitializeAsync()` is called **synchronously** in `MauiProgram.CreateMauiApp()`.
  Startup blocks on it by design.
- `MainPageModel` implements `IProjectTaskPageModel` **and** owns `HasResumableGame` / `NewGame`.
  The dashboard is shared ground: when tracker code is deleted, the page must survive with its tracker
  content removed, not deleted wholesale.
- **`Scoring/` stays free of MAUI types.** `Yahtzee.Tests` compiles those files directly rather than
  referencing the app, because a MAUI project reference drags WinUI packaging into the test build and
  its app icon collides there. A MAUI type in `Scoring/` breaks `dotnet test`.
- The test project reaches the rules through one relative glob,
  `..\Yahtzee\Scoring\*.cs` in `Yahtzee.Tests.csproj`. Moving `Scoring/` breaks it: the test build then
  fails on missing types, not on the glob.

## Conventions to follow

The tracker code is the **style reference for MVVM shape**, not for the data layer. Copy from it:
page-model-per-page, constructor DI, `[ObservableProperty]` / `[RelayCommand]` (never hand-written
`INotifyPropertyChanged`), `x:DataType` on every page and template, `ModalErrorHandler` for errors in
commands.

Do **not** copy its data layer. The duplicated schema and ordinal column mapping above are existing
debt, not a pattern to repeat. The game needs one row of state; it does not need a repository class
hierarchy.

- New files: **file-scoped namespaces** (`namespace Yahtzee.X;`). The newer Yahtzee files already do
  this; older tracker files use block namespaces. Use file-scoped in anything new. Don't reformat
  existing files to match.
- Registrations go in `MauiProgram.cs`; Shell routes use `AddTransientWithShellRoute<TPage, TModel>("route")`.
- Prefer editing existing files over creating new ones.
- No new NuGet packages or abstraction layers without asking first.
- Style rules live in the repo-root `.editorconfig`, one level above this folder — not here. It sets
  `dotnet_analyzer_diagnostic.severity = warning`, so any style rule without an explicit option shows up
  in the IDE. Expression bodies (`=>`) are allowed for single-line members; anything longer keeps a block.

## Definition of done

1. `dotnet build Yahtzee/Yahtzee.csproj -f net10.0-windows10.0.19041.0` succeeds with 0 errors.
2. UI changes are exercised by running the app and touching the screen you changed — a green build
   does not prove the layout or the behaviour is right. Every page compiles under
   `MauiXamlInflator=SourceGen`, so binding typos fail the build; spacing, sizing and gestures do not.
3. A change to a scoring rule comes with a case in `Yahtzee.Tests`, and
   `dotnet test Yahtzee.Tests/Yahtzee.Tests.csproj` passes.
4. `PLAN.md` checkboxes updated for whatever was finished.

## Working style

Small diffs, one concern at a time. State assumptions in the reply instead of silently choosing. Keep
replies concise: answer first, short paragraphs, no restating the request and no narration of tool
calls. Do not refactor tracker code on the way past it — it is scheduled for deletion, so effort spent
there is wasted.
