@tool
class_name GreenTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

var num_waves = 0

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition

func _ready():
	super._ready()
	
func _process(_delta: float) -> void:
	pass

func _set_stats(tower_stats: TowerStats) -> void:
	super._set_stats(tower_stats)
	#green tower stats
	var green_tower_stats = tower_stats as GreenTowerStats
	num_waves = green_tower_stats.num_waves
	
func _fire() -> void:
	for i in range(num_waves):
		if not _current_target:
			await get_tree().create_timer(0.1).timeout
			continue
		cristal_light.play()
		var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
		
		add_child(projectil)
		projectil.global_position = projectil_spawn_position.global_position
		
		# get enemy direction
		var dir = (_current_target.global_position - projectil.global_position).normalized()
		projectil.set_direction(dir, stats.damage)
		await get_tree().create_timer(0.1).timeout
	
	
