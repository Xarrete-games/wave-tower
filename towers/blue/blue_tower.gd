class_name BlueTower extends Tower

const BLUE_PROJECTIL = preload("uid://csif0nju31dcs")

@onready var projectil_spawn_point: Marker2D = $ProjectilSpawnPoint

func _ready():
	super._ready()
		
func _fire() -> void:
	var projectil: BlueProjectil = BLUE_PROJECTIL.instantiate()
	add_child(projectil)
	projectil.position = projectil_spawn_point.position
