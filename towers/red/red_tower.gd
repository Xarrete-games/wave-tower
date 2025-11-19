@tool
class_name RedTower extends Tower
# this tower hit 5 times
const TOTAL_HITS = 5
const EXECUTE_DAMAGE: float = 9999

var execute_threshold: float = 0.0
var local_execute_threshold: float = 0.0
var _hits_count = 0

@onready var red_projectil: RedProjectil = $RedProjectil
@onready var attack_tick_timer: Timer = $AttackTickTimer

func _ready():
	super._ready()
	target_change.connect(_on_target_change)
	
func _process(_delta: float) -> void:
	if not _current_target:	
		return

func _set_buffs(tower_buffs: TowerBuff) -> void:
	super._set_buffs(tower_buffs)
	#green tower stats
	var red_tower_buffs = tower_buffs as RedTowerBuff
	execute_threshold = local_execute_threshold + red_tower_buffs.extra_execute_threshold
	
func _fire() -> void:
	red_projectil.set_attack(_get_attack_per_hit())
	red_projectil.set_target(_current_target)
	attack_tick_timer.start()
	cristal_light.turn_on()
	_hits_count = 0

func _on_target_change(target: Enemy) -> void:
	if not attack_tick_timer.is_stopped():
		if target == null:
			_stop_attack()
		else:
			cristal_light.turn_on()
			red_projectil.set_attack(_get_attack_per_hit())
			red_projectil.set_target(target)

func _on_attack_tick_timer_timeout() -> void:
	var next_attack = _get_attack_per_hit() if _current_target.get_remaining_heal() > execute_threshold else _get_letal_attack()
	red_projectil.set_attack(next_attack)
	red_projectil.hit_target()
	_hits_count += 1
	
	if _hits_count == TOTAL_HITS:
		_stop_attack()

func _get_attack_per_hit() -> Attack:
	var attack = _get_attack()
	attack.damage = attack.damage / TOTAL_HITS
	return attack
	
func _get_letal_attack() -> Attack:
	var attack = _get_attack()
	attack = EXECUTE_DAMAGE
	attack.is_critic = true
	return attack


func _stop_attack() -> void:
	attack_tick_timer.stop()
	red_projectil.stop()
	cristal_light.turn_off()
	_hits_count = 0
