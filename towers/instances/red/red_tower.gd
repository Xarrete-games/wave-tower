@tool
class_name RedTower extends Tower
# this tower hit 5 times
const TOTAL_HITS = 5
const EXECUTE_DAMAGE: float = 9999

var execute_threshold: float = 0.0
var local_execute_threshold: float = 0.0
var burn_damage: float = 0
var _hits_count = 0
var _target_in_progress: Enemy = null

@onready var red_projectil: RedProjectil = $RedProjectil
@onready var attack_tick_timer: Timer = $AttackTickTimer

func _ready():
	super._ready()
	attack_tick_timer.wait_time = 0.1
	area_detector.enemy_die.connect(_on_enemy_die)
	
func _process(_delta: float) -> void:
	if not _current_target:	
		return

func _on_extra_stats_change(extra_stats: TowerExtraStats) -> void:
	execute_threshold = extra_stats.execute_threshold

func _fire() -> void:
	_target_in_progress = _current_target
	red_projectil.set_attack(_get_attack_per_hit())
	red_projectil.set_target(_target_in_progress)
	attack_tick_timer.start()
	cristal_light.turn_on()
	_hits_count = 0

func _on_target_change(_target: Enemy) -> void:
	super._on_target_change(_target)
	if _target == null:
		_stop_attack()
	#if not attack_tick_timer.is_stopped():
		#if target == null:
			#_stop_attack()
		#else:
			#cristal_light.turn_on()
			#red_projectil.set_attack(_get_attack_per_hit())
			#red_projectil.set_target(target)

func _on_attack_tick_timer_timeout() -> void:
	var next_attack = _get_attack_per_hit() if _current_target.get_percentage_remaining_health() > execute_threshold else _get_letal_attack()
	red_projectil.set_attack(next_attack)
	red_projectil.hit_target()
	_hits_count += 1
	
	if _hits_count == TOTAL_HITS:
		if _current_target:
			var debuff = RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.BURN)
			_current_target.apply_debuff(debuff)
		_stop_attack()

func _get_attack_per_hit() -> Attack:
	var attack = _get_attack()
	attack.damage = attack.damage / TOTAL_HITS
	return attack
	
func _get_letal_attack() -> Attack:
	var attack = _get_attack()
	attack.damage = EXECUTE_DAMAGE
	attack.is_critic = true
	return attack


func _stop_attack() -> void:
	attack_tick_timer.stop()
	red_projectil.stop()
	cristal_light.turn_off()
	_hits_count = 0

func _on_enemy_die(enemy: Enemy) -> void:
	if enemy == _target_in_progress:
		_stop_attack()
