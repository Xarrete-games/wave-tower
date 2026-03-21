class_name WaveConfig extends Resource
## Defines the scaling parameters that control how waves grow in difficulty.
## Tweak these values in the inspector to adjust the pacing curve.

# ---------------------------------------------------------
# BUDGET
# ---------------------------------------------------------
@export_group("Budget Scaling")
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
@export_group("Spawn Intervals")
## Spawn interval range for SWARM pressure groups.
## SWARM should usually feel dense, so defaults are tighter.
@export_range(0.1, 10.0, 0.01) var spawn_interval_swarm_min: float = 0.5
@export_range(0.1, 10.0, 0.01) var spawn_interval_swarm_max: float = 0.8

## Spawn interval range for SPEED pressure groups.
## SPEED enemies should feel aggressive and chained.
@export_range(0.1, 10.0, 0.01) var spawn_interval_speed_min: float = 0.7
@export_range(0.1, 10.0, 0.01) var spawn_interval_speed_max: float = 1.0

## Spawn interval range for NORMAL groups.
## Used as fallback when a group is not SWARM/SPEED/TANK.
@export_range(0.1, 10.0, 0.01) var spawn_interval_normal_min: float = 2.0
@export_range(0.1, 10.0, 0.01) var spawn_interval_normal_max: float = 3.0

## Spawn interval range for TANK pressure groups.
## TANK tends to be more deliberate by default.
@export_range(0.1, 10.0, 0.01) var spawn_interval_tank_min: float = 2.0
@export_range(0.1, 10.0, 0.01) var spawn_interval_tank_max: float = 3.0

## Every N waves, reduce both min and max values of all spawn interval ranges.
## This makes enemies spawn closer together as the run advances.
@export var spawn_interval_max_decay_every_waves: int = 5

## Amount subtracted from each pressure interval bound per decay step.
## Example: if every_waves=5 and amount=0.02, at wave 11 both min and max are reduced by 0.04.
@export_range(0.0, 1.0, 0.01) var spawn_interval_max_decay_amount: float = 0.02

## Hard lower bound applied after all interval calculations (including decay).
## Useful to keep chaos under control without touching each pressure range.
@export_range(0.01, 2.0, 0.01) var spawn_interval_min_cap: float = 0.1

## Seconds of pause between consecutive groups within the same wave.
## Gives the player a moment to notice the pressure shift.
@export var group_delay: float = 2.0

# ---------------------------------------------------------
# TYPE UNLOCK THRESHOLDS
# ---------------------------------------------------------
@export_group("Type Unlocks")
## A boss is guaranteed every N waves
@export var boss_wave_every: int = 10

# ---------------------------------------------------------
# PRESSURE PROFILE
# ---------------------------------------------------------
@export_group("Pressure Profile")   
## Ratio [0.0–1.0] of the budget reserved for the primary pressure type.
## The remainder is filled with a random mix of unlocked enemy types.
@export_range(0.0, 1.0) var primary_pressure_ratio: float = 0.7

@export var wave_chances: Array[WaveTypeChance] = []
