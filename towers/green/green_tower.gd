@tool
class_name GreenTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition

func _ready():
	super._ready()
	
func _process(_delta: float) -> void:
	pass
	
func _fire() -> void:
	var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
	
	add_child(projectil)
	projectil.global_position = projectil_spawn_position.global_position
	
	# get enemy direction
	var dir = (_current_target.global_position - projectil.global_position).normalized()
	projectil.set_direction(dir, stats.damage)
	
	
