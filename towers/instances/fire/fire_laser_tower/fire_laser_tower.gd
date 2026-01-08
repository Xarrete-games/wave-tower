class_name FireLaserTower extends Tower

const EXECUTE_DAMAGE: float = 9999

var execute_threshold: float = 0.0
var base_execute_threshold: float = 10.0
var apply_burn: bool = false

@onready var red_projectile: FireLaserProjectile = $FireLaserProjectile

func _ready():
	super._ready()
	execute_threshold = base_execute_threshold 
	
func _process(_delta: float) -> void:
	if not _current_target:
		return

func _on_extra_stats_change(extra_stats: TowerExtraStats) -> void:
	execute_threshold = base_execute_threshold + extra_stats.execute_threshold
	apply_burn = extra_stats.all_fire_apply_burn

func _fire() -> void:
	var next_attack = _get_attack() if _current_target.get_percentage_remaining_health() > execute_threshold else _get_letal_attack()

	var debuff = RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.BURN) if apply_burn else null
	red_projectile.set_target(_current_target, next_attack, debuff)
	red_projectile.hit_target()
	cristal_light.turn_on()

	await get_tree().create_timer(0.1).timeout
	red_projectile.stop()
	cristal_light.turn_off()

func _get_letal_attack() -> Attack:
	var attack = _get_attack()
	attack.damage = EXECUTE_DAMAGE
	attack.damage_type = DamageNumbers.Type.EXECUTE
	return attack
