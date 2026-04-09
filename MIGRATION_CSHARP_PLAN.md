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
  - `run_context/CompositeTileMapRuntime.cs`
- Refined core inheritance order:
  - `core/models/RelicModel.cs` is now the C# base listener model for relic implementations (inherits `AbstractModel`).
  - `run_context/RelicsManagerRuntime.cs` now stores `RelicModel` and includes core parity API (`AddRelic`, `AddRelicById`, `RemoveRelic`, `HasRelic`, `GetRelic`, `GetRelicCount`, `GetAllRelics`).
  - `core/models/RelicModel.cs` now exposes base helper methods for relic dependency checks (`HasRelic`, `GetRelicCount`).
  - `run_context/TowersManagerRuntime.cs` includes parity aliases (`GetTowerListeners`, `AddTowerPlaced`, `TowerRemoved`).
  - `run_context/ConsumablesManagerRuntime.cs` includes base parity API (`IsFull`, `AddConsumable`, `RemoveConsumable`, `GetConsumables`, `Reset`).
- Added core relic model factory for deterministic ID-based creation:
  - `relics/RelicModelFactory.cs`.
- Runtime wiring from GDScript to C# remains pending; no new runtime/autoload bridge classes should be introduced.
- Reverted resource fallback layer to keep migration clean:
  - `relics/relic_data.gd` restored to direct `runtime_script.new(self)` behavior.
  - Relic `.tres` resources restored to original `.gd` runtime scripts.
- Current substitution strategy (one-by-one, no dual resource wiring):
  - Keep existing relic resource entrypoints in `.gd`.
  - Move relic behavior logic into paired C# runtime classes and call them from each `.gd` relic implementation.
- Core dependency update for direct substitution path:
  - `relics/relic_data.gd:create_item()` now instantiates relic scripts with `new()` and assigns `item.data` explicitly.
  - `relics/relic.gd` no longer relies on `_init(p_data)` constructor injection.
  - `run_context/relics_manager.gd` and relic creation call sites now accept non-`Relic` typed instances, reducing coupling for direct C# runtime script substitution.
- First direct relic substitution in resource:
  - `relics/data/tuna_nigiri_data.tres` now points `runtime_script` to `relics/implementations/TunaNigiriRelic.cs`.
  - `TunaNigiriRelic.cs` implements the relic contract directly (signals/properties/hooks) without `.gd` wrapper delegation.
- Continued direct substitutions in resources (no wrappers):
  - `relics/data/salmon_nigiri_data.tres` -> `SalmonNigiriRelic.cs`
  - `relics/data/butterfish_nigiri.tres` -> `ButterfishNigiriRelic.cs`
  - `relics/data/soya_sauce_data.tres` -> `SoyaSauceRelic.cs`
  - `relics/data/strategy_tome_low_health_priority_data.tres` -> `StrategyTomeLowHealthPriorityRelic.cs`
  - `relics/data/strategy_tome_high_health_priority_data.tres` -> `StrategyTomeHighHealthPriorityRelic.cs`
  - `relics/data/captain_cap_data.tres` -> `CaptainCapRelic.cs`
  - `relics/data/ice_cream_data.tres` -> `IceCreamRelic.cs`
  - `relics/data/lemon_data.tres` -> `LemonRelic.cs`
  - `relics/data/pirate_blunderbuss_data.tres` -> `PirateBlunderbussRelic.cs`

- Decoupled relic manager and key callsites from strict `Relic` typing to support direct C# relic instances.
- Restored `core/base_data.gd` temporarily to keep non-migrated BaseData inheritors stable (`TowerData`, `ConsumableData`, `BuffData`, `EnemyDebuffData`) while relic data runs on `relics/RelicData.cs`.

- Refactor direction aligned with clean substitution:
  - Added `relics/RelicRuntimeAdapter.cs` as a thin Godot adapter layer.
  - Migrated substituted relic scripts to delegate behavior into pure `RelicModel` implementations (`TunaNigiri`, `SalmonNigiri`, `ButterfishNigiri`, `SoyaSauce`, `StrategyTome*`, `CaptainCap`, `IceCream`, `Lemon`).
  - Added `PirateBlunderbussRelic.cs` adapter and switched `pirate_blunderbuss` resource to direct C#.
  - Removed replaced GDScript relic implementation files for migrated resources (no dual path for those relics).
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
  - `relics/implementations/Metronome.cs` (inherits `RelicModel`, mirrors `metronome.gd`).
  - `relics/implementations/ValveAmplifier.cs` (inherits `RelicModel`, mirrors `valve_amplifier.gd`).
  - `relics/implementations/PaganiniBow.cs` (inherits `RelicModel`, mirrors `paganini_bow.gd`).
  - `relics/implementations/PowerGloves.cs` (inherits `RelicModel`, mirrors `power_gloves.gd`).
  - `relics/implementations/CrownOfTheForgottenKing.cs` (inherits `RelicModel`, mirrors `crown_of_the_forgotten_king.gd`).
  - `relics/implementations/HotChiliPepper.cs` (inherits `RelicModel`, mirrors `hot_chili_pepper.gd`).
  - `relics/implementations/HeadPhones.cs` (inherits `RelicModel`, mirrors `headphones.gd`).
  - `relics/implementations/EchoOfVoid.cs` (inherits `RelicModel`, mirrors `echo_of_void.gd`).
  - `relics/implementations/PerseusFury.cs` (inherits `RelicModel`, mirrors `perseus's_fury.gd`).
  - `relics/implementations/IgnitionVoltage.cs` (inherits `RelicModel`, mirrors `ignition_voltage.gd`).
  - `relics/implementations/VicMicrophone.cs` (inherits `RelicModel`, mirrors `vic_microphone.gd`).

- Expanded relic core parity:
  - `RelicModel` now includes `IsCursed` metadata for relic-to-relic interactions.

- Expanded status/enemy model parity:
  - `StatusModel` and `StatusRuntime` include `ChangeMaxHealth` for property-style max-health modifications.
  - `EnemyModel` includes `GoldValue` and `HasAnyDebuff`.

- Expanded tower/consumable model parity:
  - `TowerModel` now includes buff/state fields used by relic logic.
  - Added `TowerBuffModel`, `TowerBuffFactoryModel`, and `ConsumableTargeteableModel` for 1:1 relic behavior mapping.
  - Added `relics/TowerBuffRelicBase.cs` as C# core base to mirror `tower_buff_relic.gd` behavior.

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
