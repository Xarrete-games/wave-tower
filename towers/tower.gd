@abstract
class_name Tower extends Node2D

signal stats_change(tower: Tower)
signal attack_fired()
signal on_target_change(enemy: Enemy)

enum Type {FIRE, LIGHTNING, FROST}
enum TargetingMode {FIRST_IN_PROGRESS, HIGH_HP, LOW_HP}

const uuid_util = preload('res://addons/uuid/uuid.gd')
const PHANTOM_COLOR: Color = Color(1.0, 1.0, 1.0, 0.5)

@export var type: Type = Type.FIRE

var data: TowerData
var build_price: int = 0
var _current_target: Enemy
var _enabled: bool = false
var current_tower_selected: Tower
var range_tween: Tween
# when true, the tower fires instantly upon detecting an enemy
var _first_shot = true
# global stats
var stats: TowerStats:
	set(value):
		stats = value
		stats_change.emit(self)

# tile_pos
var tile_pos: Vector2i
## Key used by CompositeTileMap to identify the occupied tile.
var composite_tile_key: String = ""
var targeting_mode: TargetingMode = TargetingMode.FIRST_IN_PROGRESS:
	set(value):
		targeting_mode = value
		area_detector.targeting_type = value

# level
var exp_data: TowerExpData:
	set(value):
		exp_data = value
		stats_change.emit(self)

var id: String
var type_id: String = get_script().get_global_name()
var damage_source: Source:
	get:
		return Source.new(Source.SourceType.TOWER, type_id, self)

@onready var area_detector: AreaDetector = $AreaDetector
@onready var range_preview: RangePreview = $RangePreview
@onready var range_collision: CollisionShape2D = $AreaDetector/RangeCollision
@onready var mouse_detector: Control = $MouseDetector
@onready var attack_timer: Timer = $AttackTimer
@onready var cristal_light: CristalLight = $CristalLight
@onready var sprite_2d: Sprite2D = $Sprite2D
@onready var tower_stats_handler: TowerStatsHandler = $TowerStatsHandler
@onready var tower_area: Area2D = $TowerArea

func _ready():
	data.build()
	range_collision.shape = CircleShape2D.new()
	# stats_handlers
	tower_stats_handler.stats_change.connect(_on_stats_change)
	tower_stats_handler.set_data(data, type)
	area_detector.target_change.connect(_on_target_change)

# --------------------
# --- HELPER ---
# --------------------

static func targeting_mode_to_string(mode: Tower.TargetingMode) -> String:
	match mode:
		Tower.TargetingMode.FIRST_IN_PROGRESS:
			return "Progress"
		Tower.TargetingMode.HIGH_HP:
			return "High Health"
		Tower.TargetingMode.LOW_HP:
			return "Low Health"
		_:
			return "Unknown"


# --------------------
# --- MODES ---
# --------------------

func phantom_mode() -> void:
	sprite_2d.modulate = PHANTOM_COLOR
	range_preview.visible = false

func normal_color() -> void:
	sprite_2d.modulate = Color.WHITE
	_show_range()

# sets the tower's state while it is being placed
func placement_mode() -> void:
	_enabled = false
	area_detector.monitoring = false
	tower_area.monitorable = false
	
# Enables the tower after its construction/placement.
# It is initially disabled to prevent actions while the player is placing it.
func enable() -> void:
	sprite_2d.modulate = Color.WHITE
	_enabled = true
	area_detector.monitoring = true
	tower_area.monitorable = true

	await get_tree().create_timer(0.1).timeout
	
	ClickEvents.tower_selected.connect(_on_tower_selected)
	mouse_detector.gui_input.connect(_on_gui_input)
	mouse_detector.mouse_entered.connect(_on_mouse_entered)
	mouse_detector.mouse_exited.connect(_on_mouse_exit)

# --------------------
# --- BUFFS ---
# --------------------
func add_local_buff(tower_buff: TowerBuff) -> void:
	tower_stats_handler.add_local_buff(tower_buff)

func remove_local_buff(source_id: String) -> void:
	tower_stats_handler.remove_local_buff(source_id)

# --------------------
# --- COPY TOWER DATA---
# --------------------
func copy_tower_data(from_tower: Tower) -> void:
	# only copy relevant data
	area_detector.targets_in_range = from_tower.area_detector.targets_in_range.duplicate()
	area_detector.current_target = from_tower.area_detector.current_target
	targeting_mode = from_tower.targeting_mode

# --------------------
# --- ATTACK ---
# --------------------
func _get_attack() -> Attack:
	var is_critic = _is_critical_hit()
	var attack_damage = stats.damage * (1 + (stats.critic_damage / 100)) if is_critic else stats.damage
	var attack = Attack.new(attack_damage,damage_source, is_critic)

	var ctx = AttackContext.new(_current_target, attack, self)
	
	Hooks.on_before_attack(ctx)
	# apply final mult
	ctx.attack.damage = ctx.rebuild_attack()
	return ctx.attack
	
func _is_critical_hit() -> bool:
	var random_value: float = randf()
	return random_value < (stats.critic_chance / 100.0)
	
# --------------------
# --- TARGETING ---
# --------------------
func _on_target_change(enemy: Enemy) -> void:
		_current_target = enemy
		on_target_change.emit(enemy)
		if _first_shot and _current_target != null:
			_fire()
			attack_fired.emit()
			attack_timer.start()
			_first_shot = false

func _on_attack_timer_timeout() -> void:
	if _current_target == null or not _enabled:
		_first_shot = true
		return
	
	_fire()
	attack_fired.emit()

@abstract
func _fire() -> void

# --------------------
# --- STATS ---
# --------------------

func _on_stats_change(new_stats: TowerStats) -> void:
	stats = new_stats
	_apply_stats_changes()

func _apply_stats_changes() -> void:
	attack_timer.wait_time = stats.attack_speed
	range_preview.radius = stats.attack_range
	(range_collision.shape as CircleShape2D).radius = stats.attack_range
		
func _on_exp_data_change(new_exp_data: TowerExpData) -> void:
	exp_data = new_exp_data
# --------------------
# --- MOUSE INTERACTION ---
# --------------------
func _on_tower_selected(tower: Tower) -> void:
	current_tower_selected = tower
	if tower != self:
		range_preview.visible = false
	else:
		range_preview.visible = true

func _on_gui_input(event: InputEvent) -> void:
	if not _enabled:
		return
	if UIUtils.is_left_click_event(event):
		ClickEvents.tower_selected.emit(self)

func _on_mouse_entered():
	ClickEvents.tower_hovered.emit(self)
	_show_range()

func _on_mouse_exit():
	ClickEvents.tower_unhovered.emit(self)
	if current_tower_selected != self:
		_hide_range()
# --------------------
# --- RANGE PREVIEW ---
# --------------------
func _show_range():
	if range_tween: range_tween.kill()
	range_tween = create_tween()
	range_preview.visible = true
	range_tween.tween_property(range_preview, "self_modulate:a", 1.0, 0.1).set_trans(Tween.TRANS_SINE)

func _hide_range():
	if range_tween: range_tween.kill()
	range_tween = create_tween()
	range_tween.tween_property(range_preview, "self_modulate:a", 0.0, 0.5).set_trans(Tween.TRANS_SINE)
	range_tween.tween_callback(func(): range_preview.visible = false)
