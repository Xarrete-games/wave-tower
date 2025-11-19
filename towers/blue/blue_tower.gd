@tool
class_name BlueTower extends Tower

const BLUE_PROJECTIL = preload("uid://csif0nju31dcs")

var freezing_power = 0
var local_freezing_power = 0

@onready var projectil_spawn_point: Marker2D = $ProjectilSpawnPoint

func _set_buffs(tower_buff: TowerBuff) -> void:
	super._set_buffs(tower_buff)
	var blue_tower_buff = tower_buff as BlueTowerBuff
	freezing_power = local_freezing_power + blue_tower_buff.freezing_power

func _fire() -> void:
	cristal_light.play()
	var projectil: BlueProjectil = BLUE_PROJECTIL.instantiate()
	
	var frost_debuff = EnemyDebuff.new(EnemyDebuff.DebuffType.FROST, freezing_power, 5)
	var attack = _get_attack([frost_debuff])
	projectil.set_stats(attack, attack_range)
	call_deferred("_add_projectil", projectil)   

func _add_projectil(projectil: BlueProjectil) -> void:
	add_child(projectil)
	projectil.position = projectil_spawn_point.position
