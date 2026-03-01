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

## Minimum wave number at which FAST enemies may appear.
@export var fast_unlock_wave: int = 2

## Minimum wave number at which TANK enemies may appear.
@export var tank_unlock_wave: int = 3

## Minimum wave number at which BOSS enemies may appear.
@export var boss_unlock_wave: int = 10

## A boss is guaranteed every N waves (starting from boss_unlock_wave).
@export var boss_wave_every: int = 10

# ---------------------------------------------------------
# PRESSURE PROFILE
# ---------------------------------------------------------

## Ratio [0.0–1.0] of the budget reserved for the primary pressure type.
## The remainder is filled with a random mix of unlocked enemy types.
@export_range(0.0, 1.0) var primary_pressure_ratio: float = 0.7
