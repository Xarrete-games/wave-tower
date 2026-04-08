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
- Added C# hooks foundation: `core/Hooks.cs`
- Added C# runtime registry foundation:
  - `run_context/RunContextRuntime.cs`
  - `run_context/RelicsManagerRuntime.cs`
  - `run_context/TowersManagerRuntime.cs`

## Notes About Resources
- Resource references may need re-linking when script type changes (`.gd` -> `.cs`).
- This is expected and should be done per-slice to keep breakage contained.
