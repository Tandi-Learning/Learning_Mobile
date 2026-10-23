# AGENTS.md

Project rules for anyone (human or AI) changing this file's project. Keep this file short and factual.
Anything readable from the code does not belong here.

This file sits at the repository root, where agent tools look for it, so a session should pick it up on
its own. All paths and commands in it are relative to that root.

## What this project is

A .NET MAUI Yahtzee game, currently **half-converted** from Microsoft's "Project Tracker" MAUI sample.
The tracker (Projects / Tasks / Tags / Categories / Dashboard) still works and still ships, but it is
**temporary scaffolding and will be deleted**. The Yahtzee game itself is a UI mockup with no rules yet.

Work items and current state: see [PLAN.md](PLAN.md).

## Build and run

```bash
dotnet build -f net10.0-windows10.0.19041.0   # primary dev target, ~35s, 0 errors
dotnet run   -f net10.0-windows10.0.19041.0   # launch the app
```

`dotnet build` with no `-f` builds Android `net10.0-android`. Android needs a running emulator/device;
Windows is the fast loop, so use Windows unless the change is platform-specific.

Baseline is **0 errors and ~77 warnings** (CA1822 on page-model commands, CA1305, CsWinRT1030 from
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
- `Pages/GamePage.xaml` is pinned to `<Generator>MSBuild:Compile</Generator>` in `Yahtzee.csproj`,
  overriding the project-wide `MauiXamlInflator=SourceGen`. That means binding typos on the game page
  fail at runtime, not at compile time. Remove the pin when the page is real.

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

## Definition of done

1. `dotnet build -f net10.0-windows10.0.19041.0` succeeds with 0 errors.
2. UI changes are exercised by running the app and touching the screen you changed — a green build
   does not prove a binding resolved.
3. `PLAN.md` checkboxes updated for whatever was finished.

## Working style

Small diffs, one concern at a time. State assumptions in the reply instead of silently choosing. Do not
refactor tracker code on the way past it — it is scheduled for deletion, so effort spent there is wasted.
