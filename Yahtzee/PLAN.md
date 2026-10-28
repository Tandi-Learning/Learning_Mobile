# PLAN.md

Work items to turn the mockup into a finished Yahtzee game. Tick boxes as work lands; this file is the
session-to-session memory, so an unticked box that is actually done is a bug.

Rules and invariants live in [AGENTS.md](AGENTS.md) — don't duplicate them here.

## Decisions

Settled — don't re-litigate these in a later session:

- **13 categories**, standard scorecard: Ones–Sixes (upper), Three of a Kind, Four of a Kind, Full House,
  Small Straight, Large Straight, Yahtzee, Chance (lower). All 13 are listed on the page.
- **Upper bonus**: 35 points at 63+ in the upper section. Needs a visible row.
- **Single player**, local only. No network, no accounts, no leaderboard.
- **The grand total is derived, not stored**: it is the upper total plus the lower total, computed from
  the scorecard rather than held in an accumulator member. Same for the section totals — nothing derived
  is written by the rules, and nothing derived is persisted.
- **Persistence** extends the existing single `GameState` row rather than introducing a new pattern.
- **Dice faces** stay Unicode glyphs (U+2680…) for now; drawn pips are a polish item, not a blocker.
- **Scoring is pure logic** — functions that take five values (plus a joker flag where the rule depends on
  game state) and return an int, independent of MAUI types. They live in `Scoring/` and are tested by
  `Yahtzee.Tests`, which compiles those files directly instead of referencing the app: a MAUI project
  reference drags WinUI packaging into the test build and its app icon collides there. Anything added to
  `Scoring/` has to stay free of MAUI types, or `dotnet test` stops working.
- **Yahtzee bonus and joker rules: the full official rules.**
  - Yahtzee scores 50. Each *further* Yahtzee rolled while the Yahtzee box holds 50 adds 100, counted
    separately and shown on the Yahtzee Bonus row. The first Yahtzee earns no bonus.
  - Joker: on five of a kind with the Yahtzee box at 50, the roll scores 5 × face in the matching upper
    category, the sum of all five dice in Three/Four of a Kind and Chance, 25 in Full House, 30 in Small
    Straight and 40 in Large Straight — where it would otherwise score 0.
  - A Yahtzee box at 0 earns no bonus and no joker; the same roll scores normally (0 in Full House and
    both straights).
  - Placement is the player's choice: v1 does not force the matching upper box first. Revisit only if it
    feels wrong in play.

## Milestones

### M1 — Bindings contract (finish the UI stage)

Done when every dynamic element on `Pages/GamePage.xaml` is bound to a real page-model member, even if
the values are fake.

- [x] Add a `Die` model: `Value` (1–6), `IsHeld`, display glyph.
- [x] Add a `ScoreCategory` model: `Name`, `Score` (nullable = not yet scored), `IsAvailable`, `IsUpper`.
- [x] Replace the five hard-coded `<Border>` dice with an `ItemsSource` over `Dice` + `DataTemplate`.
      Fixes the glyphs, which currently stop at ⚄, and makes a held die stylable.
- [x] Add tap → `ToggleHoldCommand` and a visual state for held dice.
- [x] Replace the 12 hard-coded scorecard rows with an `ItemsSource` over `Scorecard`; 13 categories plus
      the derived rows (upper bonus, upper total, Yahtzee bonus, lower total) and the grand total below.
- [x] Page model surface: `Dice`, `Scorecard`, `UpperSection`, `LowerSection`, `GrandTotal`, `Round`,
      `RollsRemaining`, `RollCommand`, `ToggleHoldCommand`, with a `CanRoll` guard.
- [x] Show remaining rolls: three lights at the right of the row, one emptying per roll as it is used.
      No End Turn button — ending a turn is choosing a score, and a new game comes from the main menu.
- [x] Delete or re-wire the dead `StartGame` command — the "Start Game" button no longer exists in the XAML.
- [x] Remove the `Pages/GamePage.xaml` `MSBuild:Compile` pin from `Yahtzee.csproj` and confirm the page
      still compiles under `SourceGen`.
- [x] Runtime check by hand: tap a die and confirm the hold outline appears (no automated UI check exists).
      Verified on Windows by hand.

### M2 — Scoring rules

Done when every category, both bonuses and the joker score correctly from five die values alone, with no
UI involved. Wiring a scorecard tap to these rules is M3.

- [x] `Scoring` helper: upper section, three/four of a kind, full house, both straights, Yahtzee, chance.
      `ScoringRules.Score(category, dice, yahtzeeBoxHoldsFifty)`, with `ScoringCategory` as the enum of
      the 13 scored rows. The scorecard rows still carry display names, so M3 maps one to the other.
- [x] Upper bonus: 35 points at 63+ in the upper section — `ScoringRules.UpperBonus(upperPoints)`.
- [x] Yahtzee bonus: 100 per further Yahtzee, plus the joker values in other categories, both gated on
      whether the Yahtzee box holds 50 — the `yahtzeeBoxHoldsFifty` flag on `Score` and
      `YahtzeeBonus(yahtzeeBoxHoldsFifty)`.
- [x] Test project covering every category, plus edge cases (five of a kind counting as a kind, duplicates
      in a straight, a joker in every category, an unrolled die being rejected). 58 cases in
      `Yahtzee.Tests`, run with `dotnet test Yahtzee.Tests/Yahtzee.Tests.csproj`.

### M3 — Turn and roll state machine

- [ ] Roll up to 3 times per turn; held dice survive a re-roll.
- [ ] Scoring a category ends the turn, resets holds, advances the round.
- [ ] 13 rounds, then game over.
- [ ] Commands disabled correctly at every boundary (no rolls left, category already used, game over).

### M4 — Persistence and resume

- [ ] Extend `GameState` (dice, holds, rolls used, round, filled categories) — still one row, `ID = 1`.
      No total: the grand total is derived (see Decisions).
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
