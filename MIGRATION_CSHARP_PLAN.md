# C# Migration Plan

Goal: migrate runtime to 100% C# (no mixed runtime long-term).

## Rules
- Use official C# naming conventions (PascalCase for members/types).
- Private fields must use `_camelCase` and remain `private`.
- Always use `this.` when accessing instance members.
- Entity/listener classes should use the `*Model` suffix (e.g. `AbstractModel`).
- Do not keep `using Godot;` when a file does not use Godot types.
- Migrate domain models/contexts first, then systems (hooks/managers).
- Migrate by vertical slices; remove old `.gd` of that slice immediately after wiring.
- Keep scenes/resources working after each slice.
- Keep migration 1:1 with original behavior; avoid new abstraction layers/adapters unless strictly required for Godot interop.
- Prefer small, isolated changes per step (one feature path at a time) to simplify diff and debugging.

## Order
1. Core base types (`AbstractModel`, shared contexts, hooks facade)
2. Autoloads (`RunContext`, `GameState`, `DataLoader`, `ActionManager`)
3. Data/resources (`BaseData` + concrete data resources)
4. Combat runtime (Tower/Enemy/Buffs/Attacks)
5. UI/HUD/screens
6. Remove remaining GDScript runtime

## Current Status
- Added initial C# base: `core/AbstractModel.cs`
- Added GDScript base rename bridge: `core/abstract_model.gd` (`AbstractItem` kept as alias)
- Added initial tower logic class: `towers/TowerLogic.cs`
- Added typed C# contexts/models (no `Variant`):
  - `core/context/*.cs`, `core/models/*.cs`, `core/PriceContext.cs`
  - Added/expanded: `core/models/SourceModel.cs`, `AttackModel` source fields, `TowerModel` type fields.
- Added C# hooks foundation: `core/Hooks.cs`
- Added C# runtime registry foundation:
  - `run_context/RunContextRuntime.cs`
  - `run_context/RelicsManagerRuntime.cs`
  - `run_context/TowersManagerRuntime.cs`
  - `run_context/ConsumablesManagerRuntime.cs`
  - `run_context/EconomyRuntime.cs`
  - `run_context/StatusRuntime.cs`
- Refined core inheritance order:
  - `core/models/RelicModel.cs` is now the C# base listener model for relic implementations (inherits `AbstractModel`).
  - `run_context/RelicsManagerRuntime.cs` now stores `RelicModel` and includes core parity API (`AddRelic`, `AddRelicById`, `RemoveRelic`, `HasRelic`, `GetRelic`, `GetRelicCount`, `GetAllRelics`).
  - `core/models/RelicModel.cs` now exposes base helper methods for relic dependency checks (`HasRelic`, `GetRelicCount`).
  - `run_context/TowersManagerRuntime.cs` includes parity aliases (`GetTowerListeners`, `AddTowerPlaced`, `TowerRemoved`).
  - `run_context/ConsumablesManagerRuntime.cs` includes base parity API (`IsFull`, `AddConsumable`, `RemoveConsumable`, `GetConsumables`, `Reset`).
- Added core relic model factory for deterministic ID-based creation:
  - `relics/RelicModelFactory.cs`.
- Added first C# relic implementation candidate (not wired yet):
  - `relics/implementations/TunaNigiri.cs` (inherits `RelicModel`, mirrors `tuna_nigiri.gd`).
  - `relics/implementations/SalmonNigiri.cs` (inherits `RelicModel`, mirrors `salmon_nigiri.gd`).
  - `relics/implementations/ButterfishNigiri.cs` (inherits `RelicModel`, mirrors `butterfish_nigiri.gd`).
  - `relics/implementations/SoyaSauce.cs` (inherits `RelicModel`, mirrors `soya_sauce.gd`).
  - `relics/implementations/Lemon.cs` (inherits `RelicModel`, mirrors `lemon.gd`).
  - `relics/implementations/StrategyTomeLowHealthPriority.cs` (inherits `RelicModel`, mirrors `strategy_tome_low_health_priority.gd`).
  - `relics/implementations/StrategyTomeHighHealthPriority.cs` (inherits `RelicModel`, mirrors `strategy_tome_high_health_priority.gd`).
  - `relics/implementations/StrategyTomeEconomy.cs` (inherits `RelicModel`, mirrors `strategy_tome_economy.gd`).
  - `relics/implementations/SafetyHelmet.cs` (inherits `RelicModel`, mirrors `safety_helmet.gd`).
  - `relics/implementations/IceCream.cs` (inherits `RelicModel`, mirrors `ice_cream.gd`).
  - `relics/implementations/PhoenixFeather.cs` (inherits `RelicModel`, mirrors `phoenix_feather.gd`).
  - `relics/implementations/Salt.cs` (inherits `RelicModel`, mirrors `salt.gd`).
  - `relics/implementations/PiratePatch.cs` (inherits `RelicModel`, mirrors `pirate_patch.gd`).
  - `relics/implementations/FlowerPot.cs` (inherits `RelicModel`, mirrors `flower_pot.gd`).
  - `relics/implementations/IceVeins.cs` (inherits `RelicModel`, mirrors `ice_veins.gd`).
  - `relics/implementations/ExtraVirginOliveOil.cs` (inherits `RelicModel`, mirrors `extra_virgin_olive_oil.gd`).
  - `relics/implementations/CaptainCap.cs` (inherits `RelicModel`, mirrors `captain_cap.gd`).
  - `relics/implementations/BigBowlOfMilkAndBiscuits.cs` (inherits `RelicModel`, mirrors `big_bowl_of_milk_and_biscuits.gd`).
  - `relics/implementations/PirateHat.cs` (inherits `RelicModel`, mirrors `pirate_hat.gd`).
  - `relics/implementations/RunicLighter.cs` (inherits `RelicModel`, mirrors `runic_lighter.gd`).
  - `relics/implementations/TinfoilHat.cs` (inherits `RelicModel`, mirrors `tinfoil_hat.gd`).
  - `relics/implementations/Buda.cs` (inherits `RelicModel`, mirrors `buda.gd`).
  - `relics/implementations/CursedBuda.cs` (inherits `RelicModel`, mirrors `cursed_buda.gd`).
  - `relics/implementations/Boniato.cs` (inherits `RelicModel`, mirrors `boniato.gd`).
  - `relics/implementations/PirateBlunderbuss.cs` (inherits `RelicModel`, mirrors `pirate_blunderbuss.gd`).
  - `relics/implementations/BlackWitchHat.cs` (inherits `RelicModel`, mirrors `black_witch_hat.gd`).
  - `relics/implementations/ArticCube.cs` (inherits `RelicModel`, mirrors `artic_cube.gd`).
  - `relics/implementations/DonRafaelPipe.cs` (inherits `RelicModel`, mirrors `don_rafael_pipe.gd`).
  - `relics/implementations/TunelVision.cs` (inherits `RelicModel`, mirrors `tunel_vision.gd`).

- Expanded relic core parity:
  - `RelicModel` now includes `IsCursed` metadata for relic-to-relic interactions.

- Expanded status/enemy model parity:
  - `StatusModel` and `StatusRuntime` include `ChangeMaxHealth` for property-style max-health modifications.
  - `EnemyModel` includes `GoldValue` and `HasAnyDebuff`.

- Expanded core debuff model parity:
  - `core/models/EnemyDebuffModel.cs` now includes `DebuffType`, `Duration`, and related debuff fields.

## Next Small Slices (Low Risk)
1. Core parity first (before relic implementations):
   - Add `ConsumableModel`/`TowerModel` base parity where needed (inheritance/layout review).
   - Ensure manager APIs required by hooks are present in C# runtime (`HasRelic`, listener retrieval parity).
2. Hook parity scaffolding (no behavior changes):
   - Keep `core/hooks.gd` as source of truth.
   - Prepare C# hooks invocation points with 1:1 method signatures only.
3. Then migrate one relic implementation at a time.

These slices are prioritized to preserve deterministic diffs and reduce breakage while building core dependencies first.

## Notes About Resources
- Resource references may need re-linking when script type changes (`.gd` -> `.cs`).
- This is expected and should be done per-slice to keep breakage contained.
