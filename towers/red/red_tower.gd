@tool
class_name RedTower extends Tower
# this tower hit 5 times
const TOTAL_HITS = 5

var _hits_count = 0
var _damage_per_hit = 0

@onready var red_projectil: RedProjectil = $RedProjectil
@onready var attack_tick_timer: Timer = $AttackTickTimer
@onready var cristal_light: PointLight2D = $CristalLight

func _ready():
	super._ready()
	_damage_per_hit = stats.damage / TOTAL_HITS

func _process(_delta: float) -> void:
	if not _current_target:	
		return
	
func _fire() -> void:
	red_projectil.set_target(_current_target, _damage_per_hit)
	attack_tick_timer.start()
	_hits_count = 0
	
func _on_attack_tick_timer_timeout() -> void:
	red_projectil.hit_target()
	cristal_light.enabled = true
	_hits_count += 1
	
	if _hits_count == TOTAL_HITS:
		attack_tick_timer.stop()
		red_projectil.stop()
		cristal_light.enabled = false
		_hits_count = 0
