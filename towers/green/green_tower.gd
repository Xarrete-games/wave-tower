@tool
class_name GreenTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

var num_waves = 3
var poison_damage = 0
var local_num_waves = 3

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition
@onready var attack_player: AudioStreamPlayer2D = $AttackPlayer

func _ready():
	super._ready()
	
func _process(_delta: float) -> void:
	pass

func _set_buffs(tower_buffs: TowerBuff) -> void:
	super._set_buffs(tower_buffs)
	#green tower stats
	var green_tower_buffs = tower_buffs as GreenTowerBuff
	num_waves = local_num_waves + green_tower_buffs.extra_waves
	poison_damage = green_tower_buffs.poison_damage
	
func _fire() -> void:
	#attack_player.play()
	for i in range(num_waves):
		if not _current_target:
			await get_tree().create_timer(0.1).timeout
			continue
		var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
		call_deferred("_fire_projectil", projectil)
		await get_tree().create_timer(0.1).timeout

func _fire_projectil(projectil: GreenProjectile) -> void:
	if not _current_target:
		return
	
	cristal_light.play()
	
	const START_OFFSET_PERCENTAGE: float = 0.4
	
	var start_pos: Vector2 = projectil_spawn_position.global_position
	var target_pos: Vector2 = _current_target.global_position
	
	# position beetween projectil spawn and enemy
	var accelerated_start_pos: Vector2 = start_pos.lerp(target_pos, START_OFFSET_PERCENTAGE)
	
	add_child(projectil)
	projectil.global_position = accelerated_start_pos
	
	# configure attack
	var dir: Vector2 = (target_pos - projectil.global_position).normalized()
	var poison_debuff: EnemyDebuff = EnemyDebuff.new(EnemyDebuff.DebuffType.POISON, poison_damage, 5)
	var attack = _get_attack([poison_debuff])
	
	projectil.set_direction(dir, attack)
