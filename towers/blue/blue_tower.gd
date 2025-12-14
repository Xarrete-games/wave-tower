@tool
class_name BlueTower extends Tower

const BLUE_PROJECTIL = preload("uid://csif0nju31dcs")

var freezing_power = 0.5
var local_freezing_power = 0
var double_hit_chance = 0
var freezing_duration = 0

@onready var projectil_spawn_point: Marker2D = $ProjectilSpawnPoint

func _set_buffs(tower_buff: TowerBuff) -> void:
	super._set_buffs(tower_buff)
	var blue_tower_buff = tower_buff as BlueTowerBuff
	#freezing_power = local_freezing_power + blue_tower_buff.freezing_power
	freezing_duration = blue_tower_buff.freezing_duration
	double_hit_chance = blue_tower_buff.double_hit_chance

func _fire() -> void:
	cristal_light.play()
	var projectil: BlueProjectil = BLUE_PROJECTIL.instantiate()
	
	var is_double_hit = _is_doble_hit()
	
	var attack = _get_attack()
	projectil.set_stats(attack, attack_range, RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.FROST))
	call_deferred("_add_projectil", projectil)
	
	if is_double_hit:
		await get_tree().create_timer(0.5).timeout
		cristal_light.play()
		projectil = BLUE_PROJECTIL.instantiate()
		attack = _get_attack()
		projectil.set_stats(attack, attack_range, RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.FROST))
		call_deferred("_add_projectil", projectil)

func _add_projectil(projectil: BlueProjectil) -> void:
	add_child(projectil)
	projectil.position = projectil_spawn_point.position

func _is_doble_hit() -> bool:
	var random = randf()
	return (double_hit_chance / 100) >= random
