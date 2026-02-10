class_name EnemyGeneratorProcedural extends Node

const ENEMY_SCENE: PackedScene = preload("uid://b4grxp1f6om7o")

@export var world_map: WorldMap = null

var spawn_points: Array[Vector2] = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
