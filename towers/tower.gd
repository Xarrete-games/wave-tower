@abstract
@tool
class_name Tower extends Node2D

signal target_change(enemy: Enemy)
signal stats_change(tower: Tower)
signal selected(tower: Tower)
signal attack_fired()
signal sold(tower: Tower)
signal attack_speed_change(value: float)

enum TowerType { RED, GREEN, BLUE }

const PHANTOM_COLOR: Color = Color(1.0, 1.0, 1.0, 0.5)

@export var type: TowerType = TowerType.RED
@export var stats_base: TowerStatsBase

var _targets_in_range: Array[Enemy] = [] 
var _current_target: Enemy
var _enabled: bool = false
# when true, the tower fires instantly upon detecting an enemy
var _first_shot = true
# global stats
var stats = TowerStats:
	set(value):
		stats = value
		stats_change.emit(self)
var last_buffs: TowerBuff
# total stats
var damage: float = 0
var attack_range: float = 0
var attack_speed: float = 0:
	set(value):
		attack_speed = value
		attack_speed_change.emit(value)
		
var critic_chance: float = 0
var critic_damage: float = 0
# local stats
var local_damage: float = 0
var local_attack_range: float = 0
var local_attack_speed: float = 0
var local_critic_chance: float = 0
var local_critic_damage: float = 0
# tile_pos
var tile_pos: Vector2i

# level
var exp_data: TowerExpData:
	set(value):
		exp_data = value
		stats_change.emit(self)

@onready var range_area: Area2D = $RangeArea
@onready var range_preview: RangePreview = $RangePreview
@onready var range_collision: CollisionShape2D = $RangeArea/RangeCollision
@onready var mouse_detector: Control = $MouseDetector
@onready var attack_timer: Timer = $AttackTimer
@onready var cristal_light: CristalLight = $CristalLight
@onready var experience_handler: ExprienceHandler = $ExperienceHandler
@onready var sprite_2d: Sprite2D = $Sprite2D

func _ready():
	range_collision.shape = CircleShape2D.new()
	exp_data = experience_handler.exp_data
	experience_handler.exp_data_change.connect(_on_exp_data_change)
	experience_handler.level_up.connect(_on_level_up)
	placement_mode()
	_set_base_stats()
	_set_buffs(TowerUpgrades.get_buffs(type))
	TowerUpgrades.tower_buffs_change.connect(_on_tower_buffs_change)
	
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
	range_area.monitoring = false
	
# Enables the tower after its construction/placement.
# It is initially disabled to prevent actions while the player is placing it.
func enable() -> void:
	sprite_2d.modulate = Color.WHITE
	_enabled = true
	range_area.monitoring = true
	range_preview.visible = false

	await get_tree().create_timer(0.1).timeout
	
	TowerPlacementManager.tower_selected.connect(_on_tower_selected)
	mouse_detector.gui_input.connect(_on_gui_input)
# --------------------
# --- SELL ---
# --------------------
func sell() -> void:
	sold.emit(self)

# --------------------
# --- ATTACK ---
# --------------------
func _get_attack(new_debuffs: Array[EnemyDebuff] = []) -> Attack:
	var is_critic = _is_critical_hit()
	var attack_damege = damage * (1 + (critic_damage/100)) if is_critic else damage
	
	return Attack.new(attack_damege, is_critic, new_debuffs)
	
func _is_critical_hit() -> bool:
	var random_value: float = randf()
	return random_value < (critic_chance / 100.0)
	
# --------------------
# --- TARGETING ---
# --------------------
func _on_range_area_body_entered(body: Node2D) -> void:
	if not _enabled:
		return
	var enemy = body as Enemy
	enemy.die.connect(_on_enemy_die)
	
	_targets_in_range.append(enemy)
	# when no target
	if _current_target == null:
		_current_target = enemy
		if _first_shot:
			_fire()
			attack_fired.emit()
			attack_timer.start()
			_first_shot = false
		
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
		_first_shot = true
		return
	
	_fire()
	attack_fired.emit()

@abstract
func _fire() -> void

# --------------------
# --- STATS ---
# --------------------

func _set_base_stats() -> void:
	if not stats_base:
		push_error("[Tower] stats base not set")
	
	damage = stats_base.base_damage
	attack_range = stats_base.base_attack_range
	attack_speed = stats_base.base_attack_speed
	critic_chance = stats_base.base_critic_chance
	critic_damage = stats_base.base_critic_damage
	#local status
	local_damage = stats_base.base_damage
	local_attack_range = stats_base.base_attack_range
	local_attack_speed = stats_base.base_attack_speed
	local_critic_chance = stats_base.base_critic_chance
	local_critic_damage = stats_base.base_critic_damage
	_apply_stats_changes()

# read stats from tower_stats and assigns the values ​​to the corresponding nodes
func _set_buffs(tower_buff: TowerBuff) -> void:
	if not tower_buff:
		return
	
	# damage
	damage = (local_damage + tower_buff.extra_damage) * tower_buff.damage_mult
	# range
	attack_range = (local_attack_range + tower_buff.extra_attack_range) * tower_buff.attack_range_mult
	# attck speed
	attack_speed = (local_attack_speed - tower_buff.extra_attack_speed) * tower_buff.attack_speed_mult
	# critic change
	critic_chance = (local_critic_chance + tower_buff.extra_tower_critic_chance) * tower_buff.critic_chance_mult
	# critic damage
	critic_damage = (local_critic_damage + tower_buff.extra_critic_damage) * tower_buff.critic_damage_mult
	
	last_buffs = tower_buff
	_apply_stats_changes()
	
func _apply_stats_changes() -> void:
	attack_timer.wait_time = attack_speed
	range_preview.radius = attack_range
	(range_collision.shape as CircleShape2D).radius = attack_range
	stats = TowerStats.new(self)
	#tower_stats_panel.update_stats(stats, exp_data)

func _on_tower_buffs_change(tower_type: Tower.TowerType, tower_buffs: TowerBuff) -> void:
	if tower_type == type:
		_set_buffs(tower_buffs)
		
func _on_exp_data_change(new_exp_data: TowerExpData) -> void:
	exp_data = new_exp_data
	
func _on_level_up(_new_level: int) -> void:
	local_damage += stats_base.damage_per_level
	local_attack_range += stats_base.attack_range_level
	local_attack_speed += stats_base.attack_speed_per_level
	local_critic_chance += stats_base.critic_chance_per_level
	local_critic_damage += stats_base.critic_damage_per_level
	
	_set_buffs(last_buffs)
# --------------------
# --- MOUSE INTERACTION ---
# --------------------
func _on_tower_selected(tower: Tower) -> void:
	if tower != self:
		range_preview.visible = false

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			selected.emit(self)
			range_preview.visible = true
