class_name KillEventHandler extends Node

@export var burn_area_scene: PackedScene

@onready var enemy_generator: EnemyGenerator = %EnemyGenerator

func _ready() -> void:
	enemy_generator.enemy_killed.connect(_on_enemy_killed)

func _on_enemy_killed(enemy: Enemy, attack: Attack) -> void:
	print("Enemy killed: %s by %s" % [enemy.name, attack.source.type_id])
	if attack.source.type_id == "WildFireTower":
		_spawn_burn_area(enemy.global_position, attack.source)

func _spawn_burn_area(position: Vector2, source: Object) -> void:
	var burn_area: BurnArea = burn_area_scene.instantiate()
	add_child(burn_area)
	burn_area.global_position = position
	burn_area.setup(source)