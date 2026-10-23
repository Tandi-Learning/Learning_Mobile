# PLAN.md

Work items to turn the mockup into a finished Yahtzee game. Tick boxes as work lands; this file is the
session-to-session memory, so an unticked box that is actually done is a bug.

Rules and invariants live in [AGENTS.md](AGENTS.md) — don't duplicate them here.

## Decisions

Settled — don't re-litigate these in a later session:

- **13 categories**, standard scorecard: Ones–Sixes (upper), Three of a Kind, Four of a Kind, Full House,
  Small Straight, Large Straight, Yahtzee, Chance (lower). The current mockup has only 11 (no Four of a
  Kind, no Chance).
- **Upper bonus**: 35 points at 63+ in the upper section. Needs a visible row.
- **Single player**, local only. No network, no accounts, no leaderboard.
- **Persistence** extends the existing single `GameState` row rather than introducing a new pattern.
- **Dice faces** stay Unicode glyphs (U+2680…) for now; drawn pips are a polish item, not a blocker.
- **Scoring is pure logic** — functions that take five values and return an int, independent of MAUI types.

Open — decide before the milestone that needs it:

- Yahtzee bonus / joker rules (extra 100 per extra Yahtzee, joker substitution) — in or out of v1?
- Whether a `Yahtzee.Tests` project gets added at M2 or scoring stays untested for now. (Recommend adding:
  scoring is the one part of this app that is cheap and genuinely valuable to test.)

## Milestones

### M1 — Bindings contract (finish the UI stage)

Done when every dynamic element on `Pages/GamePage.xaml` is bound to a real page-model member, even if
the values are fake.

- [x] Add a `Die` model: `Value` (1–6), `IsHeld`, display glyph.
- [x] Add a `ScoreCategory` model: `Name`, `Score` (nullable = not yet scored), `IsAvailable`, `IsUpper`.
- [x] Replace the five hard-coded `<Border>` dice with an `ItemsSource` over `Dice` + `DataTemplate`.
      Fixes the glyphs, which currently stop at ⚄, and makes a held die stylable.
- [x] Add tap → `ToggleHoldCommand` and a visual state for held dice.
- [x] Replace the 12 hard-coded scorecard rows with an `ItemsSource` over `Scorecard`; 13 rows plus the
      upper-bonus row.
- [x] Page model surface: `Dice`, `Scorecard`, `Round`, `RollsRemaining`, `TotalScore`, `RollCommand`,
      `EndTurnCommand`, `ToggleHoldCommand`, with `CanRoll` / `CanEndTurn` guards.
- [x] Show "Rolls left" in the header (not present today).
- [x] Delete or re-wire the dead `StartGame` command — the "Start Game" button no longer exists in the XAML.
- [x] Remove the `Pages/GamePage.xaml` `MSBuild:Compile` pin from `Yahtzee.csproj` and confirm the page
      still compiles under `SourceGen`.
- [ ] Runtime check by hand: tap a die and confirm the hold outline appears (no automated UI check exists).

### M2 — Scoring rules

- [ ] `Scoring` helper: upper section, three/four of a kind, full house, both straights, Yahtzee, chance.
- [ ] Upper bonus calculation.
- [ ] Test project covering every category, plus edge cases (five of a kind counting as a kind, straights
      across a held set, Yahtzee in every upper slot).

### M3 — Turn and roll state machine

- [ ] Roll up to 3 times per turn; held dice survive a re-roll.
- [ ] Scoring a category ends the turn, resets holds, advances the round.
- [ ] 13 rounds, then game over.
- [ ] Commands disabled correctly at every boundary (no rolls left, category already used, game over).

### M4 — Persistence and resume

- [ ] Extend `GameState` (dice, holds, rolls used, round, filled categories, total) — still one row, `ID = 1`.
- [ ] Save after every action that changes state; load on page appearing.
- [ ] Wire the dashboard's `HasResumableGame` / `NewGame` to the real game, not the stub flag.

### M5 — End of game

- [ ] Final scorecard with the upper bonus applied.
- [ ] "Play again" resets state cleanly.

### M6 — Delete the tracker

Only after the game runs end to end. Keep the dashboard page; remove its tracker content.

- [ ] New Yahtzee home page replaces the tracker dashboard (`MainPageModel` is shared ground — see AGENTS.md).
- [ ] Delete `Project`, `ProjectTask`, `Tag`, `Category`, `ProjectsTags`, their repositories, seed service,
      `SeedData.json`, tracker pages/page models, chart controls, and tracker Shell routes.
- [ ] Drop tracker tables from `DatabaseInitializationService` and raise the DB filename or clear the
      schema, so stale local databases don't linger.
- [ ] Remove now-unused packages (Syncfusion would go here if the chart controls were the last user).
- [ ] Final `dotnet build` clean on Windows and Android.

### M7 — Polish

- [ ] Decide on drawn pips vs Unicode glyphs with real font coverage on Android and Windows.
- [ ] Held-die styling, roll animation or feedback.
- [ ] Light/dark theme pass on the game screen.
- [ ] Accessibility: labels for dice, semantic descriptions for scorecard rows.

## Known debt, deliberately deferred

- Schema defined twice (models + raw SQL in repositories) — becomes moot at M6.
- `MainPageModel` is a singleton page model holding tracker state; revisit when the dashboard is reborn.
- `ProjectDetailPageModel` / `TaskDetailPageModel` violate several analyzers; they are being deleted, so
  leave them alone.
