@tool
class_name BlueTower extends Tower

const BLUE_PROJECTIL = preload("uid://csif0nju31dcs")

@onready var projectil_spawn_point: Marker2D = $ProjectilSpawnPoint

func _ready():
	super._ready()

func _fire() -> void:
	cristal_light.play()
	var projectil: BlueProjectil = BLUE_PROJECTIL.instantiate()
	projectil.set_stats(stats.damage, stats.attack_range)
	add_child(projectil)
	projectil.position = projectil_spawn_point.position
