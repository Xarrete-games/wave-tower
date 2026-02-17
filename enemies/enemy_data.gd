class_name EnemyData extends Resource

@export_group("General")
@export var type_legacy: Enemy.TypeLegacy
@export var name: String
@export_multiline var description: String
@export var icon: Texture2D
@export var scene: PackedScene

@export_group("Stats")
@export var max_health: int = 50
@export var base_speed: float = 80
@export var damage: int = 1
@export var base_gold_value: int = 1
