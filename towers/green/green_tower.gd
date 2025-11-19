@tool
class_name GreenTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

var num_waves = 3
var local_num_waves = 3

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition

func _ready():
	super._ready()
	
func _process(_delta: float) -> void:
	pass

func _set_buffs(tower_buffs: TowerBuff) -> void:
	super._set_buffs(tower_buffs)
	#green tower stats
	var green_tower_buffs = tower_buffs as GreenTowerBuff
	num_waves = local_num_waves + green_tower_buffs.extra_waves
	
func _fire() -> void:
	for i in range(num_waves):
		if not _current_target:
			await get_tree().create_timer(0.1).timeout
			continue
		var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
		call_deferred("_fire_projectil", projectil)
		await get_tree().create_timer(0.1).timeout

func _fire_projectil(projectil: GreenProjectile) -> void:
	cristal_light.play()
			
	add_child(projectil)
	projectil.global_position = projectil_spawn_position.global_position
	
	# get enemy direction
	var dir = (_current_target.global_position - projectil.global_position).normalized()
	var attack = _get_attack()
	projectil.set_direction(dir, attack)
	
	
