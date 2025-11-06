class_name GreenTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition

func _ready():
	pass
	
func _process(delta: float) -> void:
	pass
	
func _fire() -> void:
	var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
	
	add_child(projectil)
	projectil.global_position = projectil_spawn_position.global_position
	
	# calcular dirección correcta antes de lanzarlo
	var dir = (_current_target.global_position - projectil.global_position).normalized()
	projectil.set_direction(dir)
	
	
