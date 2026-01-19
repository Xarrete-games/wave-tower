@abstract
class_name Tower extends Node2D

signal stats_change(tower: Tower)
signal attack_fired()
signal attack_speed_change(value: float)
signal on_target_change(enemy: Enemy)

enum Type {FIRE, LIGHTNING, FROST}
enum TargetingMode {FIRST_IN_PROGRESS, HIGH_HP, LOW_HP}

const uuid_util = preload('res://addons/uuid/uuid.gd')
const PHANTOM_COLOR: Color = Color(1.0, 1.0, 1.0, 0.5)

@export var type: Type = Type.FIRE

var configuration: TowerConfiguration
var build_price: int = 0
var _current_target: Enemy
var _enabled: bool = false
# when true, the tower fires instantly upon detecting an enemy
var _first_shot = true
# global stats
var stats: TowerStats:
	set(value):
		stats = value
		stats_change.emit(self)

# tile_pos
var tile_pos: Vector2i
var targeting_mode: TargetingMode = TargetingMode.FIRST_IN_PROGRESS:
	set(value):
		targeting_mode = value
		area_detector.targeting_type = value

# level
var exp_data: TowerExpData:
	set(value):
		exp_data = value
		stats_change.emit(self)

var modifiers: Array[AttackModifier] = []
var id: String	
var damage_source: DamageSource:
	get:
		return DamageSource.new(DamageSource.Type.TOWER, id, get_script().get_global_name())

@onready var area_detector: AreaDetector = $AreaDetector
@onready var range_preview: RangePreview = $RangePreview
@onready var range_collision: CollisionShape2D = $AreaDetector/RangeCollision
@onready var mouse_detector: Control = $MouseDetector
@onready var attack_timer: Timer = $AttackTimer
@onready var cristal_light: CristalLight = $CristalLight
@onready var experience_handler: ExprienceHandler = $ExperienceHandler
@onready var sprite_2d: Sprite2D = $Sprite2D
@onready var tower_stats_handler: TowerStatsHandler = $TowerStatsHandler
@onready var tower_area: Area2D = $TowerArea

func _ready():
	configuration.build()
	range_collision.shape = CircleShape2D.new()
	# modifiers
	modifiers = RunContext.towers_buffs.get_modifiers()
	RunContext.towers_buffs.targeting_modes_change.connect(func(new_modes: Array[Tower.TargetingMode]) -> void:
		if targeting_mode not in new_modes:
			targeting_mode = TargetingMode.FIRST_IN_PROGRESS
	)
	RunContext.towers_buffs.attack_modifiers_added.connect(func(new_modifier: AttackModifier) -> void:
		modifiers.append(new_modifier)
	)
	RunContext.towers_buffs.attack_modifiers_removed.connect(func(source_id: String) -> void:
		modifiers = modifiers.filter(func(mod: AttackModifier) -> bool:
			return mod.source_id != source_id
		)
	)
	# handlers
	experience_handler.exp_data_change.connect(_on_exp_data_change)
	tower_stats_handler.stats_change.connect(_on_stats_change)
	tower_stats_handler.extra_stats_change.connect(_on_extra_stats_change)
	tower_stats_handler.set_data(configuration, type, experience_handler)
	area_detector.target_change.connect(_on_target_change)
	
# --------------------
# --- MODES ---
# --------------------

func phantom_mode() -> void:
	sprite_2d.modulate = PHANTOM_COLOR
	range_preview.visible = false

func normal_color() -> void:
	sprite_2d.modulate = Color.WHITE
	range_preview.visible = true

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
	range_preview.visible = false
	tower_area.monitorable = true

	await get_tree().create_timer(0.1).timeout
	
	ClickEvents.tower_selected.connect(_on_tower_selected)
	mouse_detector.gui_input.connect(_on_gui_input)
	mouse_detector.mouse_entered.connect(func () -> void:
		ClickEvents.tower_hovered.emit(self)
	)
	mouse_detector.mouse_exited.connect(func () -> void:
		ClickEvents.tower_unhovered.emit(self)
	)

# --------------------
# --- BUFFS ---
# --------------------
func add_local_buff(tower_buff: TowerBuff) -> void:
	tower_stats_handler.add_local_buff(tower_buff)

func remove_local_buff(tower_buff: TowerBuff) -> void:
	tower_stats_handler.remove_local_buff(tower_buff)
# --------------------
# --- ATTACK ---
# --------------------
func _get_attack() -> Attack:
	var is_critic = _is_critical_hit()
	var attack_damage = stats.damage * (1 + (stats.critic_damage / 100)) if is_critic else stats.damage
	var damage_type = DamageNumbers.Type.CRITICAL if is_critic else DamageNumbers.Type.NORMAL
	var attack = Attack.new(attack_damage, damage_type, damage_source)

	var attack_context = AttackContext.new(_current_target, attack, is_critic)
	for modifier in modifiers:
		modifier.on_before_hit(attack_context)
	 
	# apply final mult
	attack_context.attack.damage = attack_context.attack.damage * attack_context.mult
	return attack_context.attack
	
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

@abstract
func _on_extra_stats_change(tower_extra_stats: TowerExtraStats) -> void

# --------------------
# --- STATS ---
# --------------------

func _on_stats_change(new_stats: TowerStats) -> void:
	stats = new_stats
	attack_speed_change.emit(stats.attack_speed)
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
	if tower != self:
		range_preview.visible = false
	else:
		range_preview.visible = true

func _on_gui_input(event: InputEvent) -> void:
	if not _enabled:
		return
	if Utils.is_left_click_event(event):
		ClickEvents.tower_selected.emit(self)
