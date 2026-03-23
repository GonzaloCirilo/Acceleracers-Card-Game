# Hot Wheels Acceleracers CCG

A digital implementation of the Hot Wheels Acceleracers Collectible Card Game built in Unity.

## Overview

Two players race three vehicles through four Racing Realms. The first player to advance three vehicles through all four Realms wins. Every action — drawing, playing cards, equipping, hazards — is driven by **Action Points (AP)**, and all state mutations go through a **Command** system that supports undo and replay.

---

## Project Structure

```
Assets/Scripts/
├── Cards/          # Card data models and ScriptableObject wrappers
│   └── Data/       # Unity SO classes (VehicleDataSO, ModDataSO, etc.)
├── Commands/       # Command pattern — all state mutations
│   ├── Player/     # Player-initiated actions
│   └── System/     # Engine-driven actions (advance, junk, win check)
├── Components/     # Game state containers (Deck, Hand, VehicleStack, etc.)
├── Core/           # GameState, PlayerState, SPP value type, enums
├── Effects/        # Card effect system (registry, triggers, implementations)
├── Infrastructure/ # EventBus, RNG abstraction
├── Rules/          # Validation logic (AP, modability, hazards, deck building)
├── Serialization/  # Game state and command serialization, replay
├── StateMachine/   # Turn phase machine
│   └── Phases/     # One class per phase
└── Tests/          # Unit tests (no Unity dependency)
```

---

## Architecture

### Card Data

Cards are plain C# objects with no Unity dependency. Each card type has its own class:

| Class | Card Type | Key Fields |
|---|---|---|
| `VehicleCardData` | Vehicle | `Team`, `ModabilityIcons`, `IsAdvancedVehicle` |
| `ModCardData` | Mod | `APCost`, `SPP bonus`, `ModabilityIcons` |
| `ShiftCardData` | Shift | `APCost`, `SPP bonus` (temporary) |
| `AcceleChargerCardData` | Accele-Charger | `APCost`, `SPP bonus` (temporary) |
| `HazardCardData` | Hazard | `APCost`, `SPP damage` |
| `RacingRealmCardData` | Racing Realm | `TerrainIcons`, `EscapeValue` |

All inherit from `CardData`. At runtime, `CardInstance` wraps `CardData` with a unique instance ID.

**In Unity**, each card type has a matching ScriptableObject (`VehicleDataSO`, `ModDataSO`, etc.) with a `ToCardData()` method. Place assets in `Assets/Resources/Cards/` and call `CardDatabase.LoadFromResources()` to populate the registry.

### Command Pattern

Every state mutation is a command implementing `ICommand`:

```
Execute() → mutates GameState
Undo()    → reverts the mutation
Validate() → returns error string or null
```

`CommandProcessor` runs commands through validation before executing them and stores snapshots for undo. `CommandHistory` records all commands for deterministic replay.

**Player commands:** `DrawCardCommand`, `PlayVehicleCommand`, `EquipModCommand`, `EquipShiftCommand`, `EquipAcceleChargerCommand`, `PlayHazardCommand`, `DiscardCardCommand`, `SpendAPToDrawCommand`, `EndPhaseCommand`

**System commands:** `AdvanceVehicleCommand`, `StripTemporariesCommand`, `JunkCardCommand`, `DestroyVehicleCommand`, `CheckWinConditionCommand`, `SetAPCommand`, `ApplyTerrainBonusCommand`, and more.

### Turn Phase Machine

`GamePhaseMachine` runs the turn as a sequence of phases defined in `GamePhaseId`:

```
Setup → Draw → Advance → PlayVehicle → TuneUp → Action → Discard → EndTurn → (repeat)
```

- **Automatic phases** (Setup, Draw, Advance, TuneUp, EndTurn) execute commands and advance immediately.
- **Interactive phases** (PlayVehicle, Action, Discard) wait for the player to call `EndPhase`.

### Game State

```
GameState
├── PlayerState[0]
│   ├── Deck, Hand, JunkPile
│   ├── VehiclesInPlay: List<VehicleStack>
│   │   └── VehicleStack: Vehicle + Mods + Shifts + AcceleCharger + TokenSet + RealmIndex
│   └── AP, VehiclesFinished
├── PlayerState[1]
│   └── (same structure)
└── RealmTrack: 4 RacingRealmCardData slots
```

`SPP` is an immutable value type (Speed, Power, Performance). The `SPPCalculator` sums a vehicle's base SPP plus all equipped card bonuses plus terrain bonuses.

### Rules Engine

Validation is separated from commands. Rule classes check conditions and return error messages:

- `PlayVehicleRules` — one vehicle per turn, played to Realm 1
- `EquipRules` → `ModabilityRules` — at least one matching modability icon required
- `HazardTargetRules` — targets Shifts/Mods; checks `HazardImmunity` effect; applies SPP damage and junks on zero
- `ActionPointRules` — AP costs, team bonus calculation (+1 per team with 2+ vehicles)
- `AdvanceRules` — vehicle stack SPP must meet or beat the Realm's escape value
- `DeckBuildingRules` — max 80 cards, 1 copy of vehicles/AcceleChargers/Realms, 3 copies of Shifts/Mods/Hazards

### Effects System

Cards reference an `effectId` string. `EffectRegistry` maps IDs to `ICardEffect` implementations. Effects are either:

- **Passive** — checked during validation (e.g. `HazardImmunityEffect`, `IgnoreModabilityEffect`)
- **Triggered** — fire on game events (`OnEquip`, `OnAdvance`, `OnJunked`, etc.) and return a list of commands to execute

Built-in effects: `NullEffect`, `HazardImmunityEffect`, `IgnoreModabilityEffect`, `PersistOnAdvanceEffect`, `TimedDestructionEffect`.

### Infrastructure

- **`EventBus`** — type-safe publish/subscribe for UI notification (`CardDrawnEvent`, `VehicleAdvancedEvent`, `GameOverEvent`, etc.). Game logic never depends on it.
- **`IRandomProvider` / `SeededRandomProvider`** — RNG abstraction for deterministic coin flips and shuffles. Pass a seed to reproduce any game.

---

## Adding Cards

1. In Unity, right-click in the Project window → **Create > AcceleracersCCG > [type]**
2. Fill in the Inspector fields (id, name, SPP values, team, icons, etc.)
3. Place the asset in `Assets/Resources/Cards/`
4. `CardDatabase.LoadFromResources()` picks it up automatically at runtime

---

## Disclaimer

Hot Wheels, Acceleracers, and all related names, characters, and imagery are trademarks of Mattel, Inc. This is an unofficial fan project, not affiliated with or endorsed by Mattel.

---

## Running Tests

Tests live in `Assets/Scripts/Tests/` and have no Unity dependency — they can run in any C# test runner. Test files cover: SPP arithmetic, command undo, deck building validation, hazard resolution, phase machine transitions, and full game simulations.