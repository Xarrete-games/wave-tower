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

func _fire() -> void:
	var next_attack = _get_attack() if _current_target.get_percentage_remaining_health() > execute_threshold else _get_letal_attack()

	var debuff = EnemyDebuff.create_burn(damage_source) if apply_burn else null
	red_projectile.set_target(_current_target, next_attack, debuff)
	cristal_light.turn_on()

	await get_tree().create_timer(0.1).timeout
	red_projectile.hit_target()
	red_projectile.stop()
	cristal_light.turn_off()

func _get_letal_attack() -> Attack:
	var attack = _get_attack()
	attack.damage = EXECUTE_DAMAGE
	attack.is_execute = true
	return attack
