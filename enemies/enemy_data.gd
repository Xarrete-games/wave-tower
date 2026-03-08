class_name EnemyData extends Resource

## Categorizes the enemy's role in wave composition.
## Determines how the wave composer uses this enemy to create pressure.
enum Type { SWARM, FAST, NORMAL, TANK, BOSS }

@export_group("General")
@export var type_legacy: Enemy.TypeLegacy
## The pressure role this enemy plays during wave composition.
@export var type: Type = Type.NORMAL
@export var name: String
@export_multiline var description: String
@export var icon: Texture2D
@export var scene: PackedScene

@export_group("Stats")
@export var max_health: int = 50
@export var base_speed: float = 80
@export var damage: int = 1
@export var base_gold_value: int = 1

@export_group("Wave")
## Budget cost for wave composition. Higher weight = stronger/more expensive enemy.
@export var weight: int = 1
@export var available_waves: Array[EnemyWaveRange] = []
