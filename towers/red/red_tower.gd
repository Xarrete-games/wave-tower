@tool
class_name RedTower extends Tower
# this tower hit 5 times
const TOTAL_HITS = 5

var _hits_count = 0
var _damage_per_hit = 0

@onready var red_projectil: RedProjectil = $RedProjectil
@onready var attack_tick_timer: Timer = $AttackTickTimer

func _ready():
	super._ready()
	target_change.connect(_on_target_change)
	_damage_per_hit = stats.damage / TOTAL_HITS

func _process(_delta: float) -> void:
	if not _current_target:	
		return
	
func _fire() -> void:
	red_projectil.set_target(_current_target, _damage_per_hit)
	attack_tick_timer.start()
	cristal_light.turn_on()
	_hits_count = 0

func _on_target_change(target: Enemy) -> void:
	if not attack_tick_timer.is_stopped():
		if target == null:
			_stop_attack()
		else:
			red_projectil.set_target(target, _damage_per_hit)

func _on_attack_tick_timer_timeout() -> void:
	red_projectil.hit_target()
	_hits_count += 1
	
	if _hits_count == TOTAL_HITS:
		_stop_attack()

func _stop_attack() -> void:
	attack_tick_timer.stop()
	red_projectil.stop()
	cristal_light.turn_off()
	_hits_count = 0
