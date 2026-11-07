# KNOWLEDGE.md

Lessons learned while building this Yahtzee MAUI app, kept so a later session — human or agent — does not
have to rediscover them. This file is a log, not a spec: current work items live in [PLAN.md](PLAN.md),
binding project rules and invariants in [AGENTS.md](AGENTS.md).

Each entry is written as **symptom → cause → fix**, because the symptom is what gets searched for later.
Append new entries at the end of the relevant section; do not rewrite history here, and do not promote an
entry into AGENTS.md unless it is a rule that must never be broken.

---

## Environment and toolchain

### "Waiting for emulator to be ready…" never finishes when pressing F5

**Symptom.** Visual Studio shows that dialog indefinitely. The emulator window may be open, responsive, and
fully booted — it still never returns.

**Cause.** No ADB server process was running. VS gates Android deployment on `emulator-5554` changing from
`offline` to `device`. With no server holding the emulator's console connection, that change never arrives
and the wait never expires on its own.

**Confirm it** (solution folder, any terminal):

```bash
"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe" devices
```

- `* daemon not running; starting now at tcp:5037` is the tell: the bridge had died.
- `emulator-5554   offline` is the stuck state VS is staring at.
- Proof the guest itself is fine: `adb -s emulator-5554 shell getprop sys.boot_completed` → `1`.

**Fix.** Start a server and reconnect, then Stop Debugging and press F5 again — the open dialog never
retries itself:

```bash
adb reconnect offline        # any adb command also starts the server, e.g. `adb devices`
```

**Steps to stop it recurring**

1. Start the emulator from the Android Device Manager *before* pressing F5. VS then attaches to a running
   device instead of launching one and gating on its own startup wait.
2. Put `C:\Program Files (x86)\Android\android-sdk\platform-tools` on the user `PATH` and set `ANDROID_HOME`
   to `C:\Program Files (x86)\Android\android-sdk`. Without them nothing outside VS can inspect or restart
   the bridge, so the hang is invisible until it is fatal.
3. When it hangs, restart the bridge instead of restarting Visual Studio:
   `adb kill-server && adb start-server && adb reconnect offline`. The emulator itself rarely needs a
   restart, and a rebuild costs far more than 5 seconds.
4. Check Tools → Options → Xamarin → Android Settings: SDK path and JDK path must both point at that one
   SDK install. Two `adb.exe` copies of different versions shut each other's server down in a loop, which
   looks exactly like this bug. (This machine currently has only one, at the path above.)
5. Keep exactly one AVD that boots. Extra AVDs are a menu of new ways to wait forever — see the next entry.

**Related red herring.** `VirtualizationFirmwareEnabled = False` alongside `HypervisorPresent = True` is
normal when Hyper-V owns VT-x. Do not send anyone into BIOS for it; check `HypervisorPresent` and whether
the emulator actually booted instead.

### An ARM system image cannot boot on an x86_64 host

**Symptom.** Same "waiting for emulator" hang, but no emulator window ever appears.

**Cause.** The AVD's `config.ini` has `abi.type=armeabi-v7a` / `hw.cpu.arch=arm`. The QEMU2 emulator has no
ARM-on-x86_64 path, so the launch dies before the device exists. This project still carries such an AVD,
`VisualStudio_android-25_arm_phone`, next to the working `nexus_7_2012_-_api_36_0` (x86_64, API 36).

**Fix.** Delete AVDs whose `abi.type` is not `x86_64`:

```bash
grep -i "abi.type" "$USERPROFILE"/.android/avd/*.avd/config.ini
```

### Build commands that waste time

- `dotnet build -f <tfm>` at the solution folder pushes the framework onto `Yahtzee.Tests` and fails with
  `NETSDK1005`. Name the app project: `dotnet build Yahtzee/Yahtzee.csproj -f net10.0-windows10.0.19041.0`.
- Android needs a live emulator; the Windows target is the fast loop. Use Android only for
  platform-specific work.
- **`adb install` of a Debug APK installs code the app will not run.** Debug builds use Fast Deployment:
  the APK is a stub and the managed assemblies live in
  `/data/user/0/<pkg>/files/.__override__/<abi>/`. A plain `adb install -r` leaves the *previous* build's
  assemblies in place (so you test stale code and see stale colours), and after `adb uninstall` the
  directory is empty and the app aborts with `monodroid: No assemblies found ... Exiting`. Deploy with
  `dotnet build Yahtzee.csproj -f net10.0-android -t:Install` instead.

---

## XAML and MVVM

### A command whose name matches a model type stops compiling

Naming the score-row command `ScoreCategoryCommand`/`ScoreCategory` collided with the *model* type
`ScoreCategory` — the method's return type became ambiguous. Renamed to `ScoreRow`. Rule of thumb: command
names describe the action (`ScoreRow`, `ToggleHold`), never the type they operate on.

### Inside a `DataTemplate`, the binding context is the item, not the page model

`{Binding ScoreRowCommand}` inside the row template resolves against the `ScoreCategory` row and silently
finds nothing. The page model has to be reached through the visual tree:

```xml
<TapGestureRecognizer
    Command="{Binding Source={RelativeSource AncestorType={x:Type pageModels:GamePageModel}}, Path=ScoreRowCommand}"
    CommandParameter="{Binding .}" />
```

`CommandParameter="{Binding .}"` is how the tapped row reaches the handler — the same shape as
`ToggleHoldCommand` on a die. No separate "selected row" state is needed; the tap is the selection.

### A `Grid` needs a non-null background to receive taps

`Grid` with no `BackgroundColor` is not reliably hit-testable on every platform. The row template sets
`BackgroundColor="Transparent"` deliberately — that is not decoration, and removing it kills scoring.

### Triggers are last-one-wins, and `HasScore` is the wrong question for a total

The spent-row grey keyed off `HasScore`, but the recompute writes a score onto Upper Bonus, Upper Total,
Yahtzee Bonus and Lower Total too. They went grey — and grey text on a bold label reads as "not bold", so
the bug surfaced as "the bold rows stopped being bold". The fix was a semantic flag,
`ScoreCategory.IsSpent => HasScore && !IsDerived`, with `[NotifyPropertyChangedFor(nameof(IsSpent))]` on
`_score`. Lesson: a trigger should bind to the meaning you want ("spent"), not to a proxy that happens to
be true ("has a number").

### A binding to something that never populates still compiles

The third roll light was bound to `RollFlags[2]`, a leftover collection nothing wrote, so it stayed grey
with three rolls left. XAML source-gen catches *typos* in names, not bindings that resolve to an empty
value forever. When a control never updates, check the source property exists and is actually set.

### Die faces come from a fallback font, so do not "improve" them

U+2680–U+2685 and U+2610 are in neither Segoe UI nor Roboto — the default UI font on each platform. They
render because the platform falls back: Segoe UI Symbol (`C:\Windows\Fonts\seguisym.ttf`) on Windows, and
`NotoSansSymbols-Regular-Subsetted.ttf` on Android (the `NotoSansSymbols-Regular-Subsetted2.ttf` family is
the one that *lacks* them, so do not repoint the label at it). Verified rendered, not just present in the
cmap: Windows by eye, Android on an API 36 emulator against the score previews. A drawn pip grid would buy
nothing over this.

### Semantics have to go on the label, and you cannot hide the second label

- In a `DataTemplate` the node a screen reader reaches is the `Label`, not the `Border` or `Grid` around it
  that carries the tap gesture. Put `SemanticProperties.Description` on the label and the string arrives as
  `content-desc` on Android.
- Nothing in MAUI 10 removes a sibling label from the Android accessibility tree:
  `AutomationProperties.IsInAccessibleTree="False"` is not mapped (a test button stayed in the tree with
  `uiautomator dump`), and neither is `SemanticProperties.ExcludedWithChildren`. A scorecard row therefore
  announces its sentence and then its bare number. `AutomationProperties.ExcludedWithChildren` compiles and
  mirrors a real WinUI property, so it is worth keeping for Windows — but never assume it works elsewhere.
- **Verify semantics without a screen reader:** `adb shell uiautomator dump` exposes `content-desc`, so the
  announcement can be read off the device. That is how the dice ("Showing 5, held") and the row states were
  confirmed.

### Derived properties need explicit change notifications

`Die.Glyph` only repaints because `[NotifyPropertyChangedFor(nameof(Glyph))]` sits on `_value`; without it
the face glyph freezes. Any computed property exposed to XAML must be named in a
`[NotifyPropertyChangedFor]` on each input, or it silently goes stale.

---

## Scoring decisions (this project's rules, not "standard" Yahtzee)

- **Upper bonus:** 35 points once the six upper rows total 63 or more. It counts toward **Upper Total**;
  **Grand Total = Upper Total + Lower Total**. There is no separate overall total.
- **Yahtzee bonus:** 100 points for a five of a kind committed while the Yahtzee row already holds 50.
  Awarded once per turn, at commit — rerolling a Yahtzee away mid-turn cannot farm it. It counts toward
  **Lower Total**.
- **Joker rule (chosen deliberately):** applies only to Full House, Small Straight and Large Straight —
  those pay their fixed value on a joker five-of-a-kind instead of 0. Upper rows, three/four of a kind and
  Chance need no special case, because a five of a kind already scores there.
- **Derived rows are never tappable:** `ScoreCategory.Category` is null for the bonus and the totals, and
  `CanScore` requires it, so no display-name → enum dictionary is needed and no string can drift.
- **Rows preview their value:** each open row shows what it would pay for the dice on the table
  (`Preview`, folded into `DisplayScore`) so the player is not choosing blind; the preview clears at the
  start of each turn. The tap commits, including committing a 0 when nothing scores.

---

## Testing and warnings

- `Scoring/` must stay free of MAUI types. `Yahtzee.Tests` glob-compiles `..\Yahtzee\Scoring\*.cs` instead
  of referencing the app project, because that reference drags WinUI packaging into a plain `net10.0` test
  build. One MAUI `using` there breaks `dotnet test`.
- `dotnet test Yahtzee.Tests/Yahtzee.Tests.csproj` is the whole suite (~58 rules cases) and takes seconds.
  A scoring change without a new case is unfinished.
- Baseline is **0 errors and ~80 warnings** (CA1822 on page-model commands, CA1305, CsWinRT1030 from XAML
  source-gen). The warning count drifting by a few is noise; new *errors* are not. Do not "fix" the
  pre-existing set — it makes every future diff look noisy.

---

## Working with the plan and the tracker code

- **Editing `PLAN.md` checkboxes is where boxes get lost.** A `str_replace` around a `- [ ]` line can eat
  adjacent lines; re-read the section after editing rather than trusting the diff summary. Restore any box
  that vanished.
- **The tracker is scheduled for deletion.** Projects / Tasks / Tags / Categories / Dashboard still compile
  and ship, but do not refactor them, style them, or reuse their repository layer — effort there is thrown
  away. Copy only their MVVM *shape*.
- **Verify UI changes by running the app.** Every page compiles under `MauiXamlInflator=SourceGen`, so
  binding typos fail the build — but spacing, sizing, colours and gesture behaviour do not. A green build
  says nothing about whether a tap lands or a grey dot reads as "off".
