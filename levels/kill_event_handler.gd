class_name KillEventHandler extends Node

@onready var enemy_generator: EnemyGenerator = %EnemyGenerator

func _ready() -> void:
	enemy_generator.enemy_killed.connect(_on_enemy_killed)

func _on_enemy_killed(enemy: Enemy, attack: Attack) -> void:
	pass
