@abstract
@tool
class_name Tower extends Node2D

signal target_change(enemy: Enemy)


enum TowerType { RED, GREEN, BLUE }

@export var stats: TowerStats
@export var type: TowerType = TowerType.RED

var _targets_in_range: Array[Enemy] = [] 
var _current_target: Enemy
var _enabled: bool = false
var build_price:
	get:
		return stats.build_price

@onready var range_area: Area2D = $RangeArea
@onready var range_preview: RangePreview = $RangePreview
@onready var range_collision: CollisionShape2D = $RangeArea/RangeCollision
@onready var mouse_detector: Control = $MouseDetector
@onready var attack_timer: Timer = $AttackTimer
@onready var cristal_light: CristalLight = $CristalLight

func _ready():
	placement_mode()
	_set_stats()
	attack_timer.start()

# sets the tower's state while it is being placed
func placement_mode() -> void:
	_enabled = false
	range_area.monitoring = false
	range_preview.visible = true

# Enables the tower after its construction/placement.
# It is initially disabled to prevent actions while the player is placing it.
func enable() -> void:
	_enabled = true
	range_area.monitoring = true
	range_preview.visible = false
	mouse_detector.mouse_entered.connect(_on_mouse_entered)
	mouse_detector.mouse_exited.connect(_on_mouse_exited)
	
func _on_range_area_body_entered(body: Node2D) -> void:
	if not _enabled:
		return
	var enemy = body as Enemy
	enemy.die.connect(_on_enemy_die)
	
	_targets_in_range.append(enemy)
	if _current_target == null:
		_current_target = enemy

func _on_range_area_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	_remove_target_and_get_next(enemy)

func _on_enemy_die(enemy: Enemy) -> void:
	_remove_target_and_get_next(enemy)

func _remove_target_and_get_next(enemy: Enemy) -> void:
	# disconnect signal
	if enemy.die.is_connected(_on_enemy_die):
		enemy.die.disconnect(_on_enemy_die)
	# remove enemy
	_targets_in_range.erase(enemy)
	#exit when enemy is not the target
	if enemy != _current_target:
		return
	if _targets_in_range.is_empty():
		_current_target = null
		target_change.emit(_current_target)
	else:
		#logic to select next target
		_current_target = _targets_in_range[0]
		target_change.emit(_current_target)

func _on_attack_timer_timeout() -> void:
	if _current_target == null or not _enabled:
		return
	
	_fire()

# read stats from tower_stats and assigns the values ​​to the corresponding nodes
func _set_stats() -> void:
	if not stats:
		push_error("[Tower] stats not assigned in tower %s" % name)
	attack_timer.wait_time = stats.attack_speed
	range_preview.radius = stats.attack_range
	(range_collision.shape as CircleShape2D).radius = stats.attack_range

func _on_mouse_entered():
	range_preview.visible = true

func _on_mouse_exited():
	range_preview.visible = false
	
@abstract
func _fire() -> void
