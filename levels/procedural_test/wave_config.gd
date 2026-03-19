class_name WaveConfig extends Resource
## Defines the scaling parameters that control how waves grow in difficulty.
## Tweak these values in the inspector to adjust the pacing curve.

# ---------------------------------------------------------
# BUDGET
# ---------------------------------------------------------

## Starting budget for wave 1. Each enemy costs its 'weight' from this budget.
@export var base_budget: int = 10

## Additional budget added for every wave after the first.
@export var budget_per_wave: int = 5

## Wave where exponential scaling starts.
## Up to this wave, budget uses only linear growth.
@export var exponential_start_wave: int = 8

## Exponential growth factor applied after exponential_start_wave.
## Example: 0.08 means +8% multiplicative growth per extra wave.
@export_range(0.0, 1.0, 0.01) var exponential_growth: float = 0.08

# ---------------------------------------------------------
# SPAWN TIMING
# ---------------------------------------------------------

## Seconds between individual enemy spawns within a group.
@export var spawn_interval: float = 0.8

## Seconds of pause between consecutive groups within the same wave.
## Gives the player a moment to notice the pressure shift.
@export var group_delay: float = 2.0

# ---------------------------------------------------------
# TYPE UNLOCK THRESHOLDS
# ---------------------------------------------------------

## A boss is guaranteed every N waves
@export var boss_wave_every: int = 10

# ---------------------------------------------------------
# PRESSURE PROFILE
# ---------------------------------------------------------

## Ratio [0.0–1.0] of the budget reserved for the primary pressure type.
## The remainder is filled with a random mix of unlocked enemy types.
@export_range(0.0, 1.0) var primary_pressure_ratio: float = 0.7

@export var wave_chances: Array[WaveTypeChance] = []
