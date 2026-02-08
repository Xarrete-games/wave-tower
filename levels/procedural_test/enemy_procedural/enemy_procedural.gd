class_name EnemyProcedural  extends CharacterBody2D

signal die(enemy: Enemy, attack: Attack)
signal target_reached(enemy: Enemy)

enum Type {SPECTRE, BUBA, BIG_SPECTRE, GOLEM, SKELETON, BLACK_GOLEM, BLACK_SKELETON, GOLD_SKELETON, INVOKER, SKULL}

const GOLD_DROPPED = preload("uid://cxs4ar5enx4mn")
const DAMAGE_NUMBERS = preload("uid://bkiu4qgh3ug1m")

@export var base_speed: float = 80
@export var max_health: float = 50
@export var base_gold_value: int = 1
@export var damage: int = 1

var health: float
var hit_tween: Tween
var gold_value: int
var _last_is_right_direction: bool = false

# PathFollow2D removed: movement handled exclusively via NavigationAgent2D
var default_modulate_color: Color = Color.WHITE
# speed
var _base_speed: float = 100.0
var _speed_mult: float = 1.0
var is_right_direction: bool = true

var _enabled: bool = true
var _is_dead: bool = false
var _is_freeze: bool = false

var speed: float:
	get: return _base_speed * _speed_mult
	set(value):
		_base_speed = value

var speed_mult: float:
	get: return _speed_mult
	set(value):
		_speed_mult = value

# progress_ratio removed (was tied to PathFollow2D)

var target_position: Vector2:
	get:
		if is_right_direction:
			return target_position_right.global_position
		else:
			return target_position_left.global_position

var damage_taken_modifiers: Array[DamageTakenModifier]
	
@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var health_bar: HealthBar = $HealthBar
@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var collision_shape_2d: CollisionShape2D = $CollisionShape2D
# debuff
@onready var debuff_handler: DebuffHandler = $DebuffHandler
# target positions
@onready var target_position_left: Marker2D = $TargetPositionLeft
@onready var target_position_right: Marker2D = $TargetPositionRight
# new navigation agent
@onready var navigation_agent_2d: NavigationAgent2D = $NavigationAgent2D
var _nav_target: Vector2 = Vector2.ZERO
var _use_navigation_agent: bool = true
func _ready() -> void:
	#disable()
	speed = base_speed
	health_bar.set_max_health(max_health)
	_set_health(max_health)
	
	#damage taken modifiers
	# damage_taken_modifiers = RunContext.enemy_debuff_manager.get_modifiers()
	# RunContext.enemy_debuff_manager.modifier_change.connect(
	# 	func(modifiers: Array[DamageTakenModifier]):
	# 		damage_taken_modifiers = modifiers
	# )
	# # extra gold dropped
	# gold_value = base_gold_value + RunContext.economy.extra_gold_dropped
	# RunContext.economy.extra_gold_dropped_change.connect(
	# 	func(value): gold_value = base_gold_value + value)

func _process(delta: float):
	#debuff_handler.update_all(self as Enemy)
	# Use NavigationAgent2D exclusively for movement
	if navigation_agent_2d and _use_navigation_agent:
		_process_navigation_agent(delta)
	return

func _process_navigation_agent(delta: float) -> void:
	# get next position from the agent using known method names dynamically
	var next_pos: Vector2
	var method_candidates: Array = ["get_next_path_position", "get_next_location", "get_next_position", "get_next_point"]
	for m in method_candidates:
		if navigation_agent_2d.has_method(m):
			next_pos = navigation_agent_2d.call(m)
			break

	# fallback: try property access for next path position
	if next_pos == null:
		if navigation_agent_2d.has_method("get_next_path_position"):
			next_pos = navigation_agent_2d.call("get_next_path_position")

	if next_pos == null:
		# nothing we can do this frame
		return

	# move towards next_pos
	var previous_global_x = global_position.x
	var previous_global_y = global_position.y

	var step = speed * delta
	global_position = global_position.move_toward(next_pos, step)

	# if agent reports target reached, emit and stop
	var finished: bool = false
	if navigation_agent_2d.has_method("is_navigation_finished"):
		finished = navigation_agent_2d.call("is_navigation_finished")
	elif navigation_agent_2d.has_method("is_target_reached"):
		finished = navigation_agent_2d.call("is_target_reached")
	else:
		# heuristic: close enough to target
		if global_position.distance_to(_nav_target) <= max(4.0, step * 0.5):
			finished = true

	if finished:
		_on_target_reached()
		return

	# FLIP SPRITE
	is_right_direction = global_position.x > previous_global_x
	if is_right_direction != _last_is_right_direction:
		animated_sprite_2d.flip_h = !animated_sprite_2d.flip_h
		_last_is_right_direction = is_right_direction

	# handle animation
	var animation = "top right" if previous_global_y > global_position.y else "down right"
	if animated_sprite_2d.animation != animation or not animated_sprite_2d.is_playing():
		animated_sprite_2d.play(animation)

func set_navigation_target(pos: Vector2) -> void:
	_nav_target = pos
	if navigation_agent_2d:
		# try known setter methods/properties
		if navigation_agent_2d.has_method("set_target_position"):
			navigation_agent_2d.call("set_target_position", pos)
			return
		if navigation_agent_2d.has_method("set_target_location"):
			navigation_agent_2d.call("set_target_location", pos)
			return
		# fallback: try to set a property
		if navigation_agent_2d.has_property("target_position"):
			navigation_agent_2d.target_position = pos
			return
	

# PathFollow-based path plumbing removed; movement handled by NavigationAgent2D only.

func disable() -> void:
	animated_sprite_2d.visible = false
	collision_shape_2d.disabled = true
	health_bar.visible = false

func enable() -> void:
	_enabled = true
	animated_sprite_2d.visible = true
	health_bar.visible = true
	#wait a frame to avoid immediate collision
	await get_tree().process_frame
	collision_shape_2d.disabled = false
# --------------------
# --- HEALT ---
# --------------------

# percentage of remaining heal
func get_percentage_remaining_health() -> float:
	if max_health <= 0:
		return 0.0
	var health_ratio: float = health / max_health
	var percentage: float = health_ratio * 100.0
	
	return min(100.0, percentage)

func get_remaining_health() -> float:
	return health

func get_debuff_stacks(debuff_type: EnemyDebuff.Type) -> int:
	return debuff_handler.get_stacks(debuff_type)

func has_any_debuff() -> bool:
	return debuff_handler.has_any_defbuff()

func apply_debuff(_debuff: EnemyDebuff, _amount: int = 1) -> void:
	pass
	#debuff_handler.add_debuff(debuff, amount, self)

func apply_damage(attack: Attack) -> void:
	if _is_dead:
		return

	var acc: DamageTakenModifierAcc = DamageTakenModifierAcc.new(attack.source, attack.origin_source)
	for modifier in damage_taken_modifiers:
		pass
		#modifier.modify_damage(self, acc)

	var modified_damage: float = (attack.damage + acc.flat_damage) * acc.damage_mult
	attack.damage = modified_damage
	var damage_done: float = min(attack.damage, health)
	_set_health(health - attack.damage)
	_play_hit_animation()
	_show_damage(attack)

	RunContext.damage_recount.record_damage(RunContext.progress.current_wave, attack.origin_source.id, damage_done)
	if health <= 0 and not _is_dead:
		_is_dead = true
		_die(attack)

func _play_hit_animation() -> void:
	if hit_tween and hit_tween.is_running():
		hit_tween.kill()
	
	hit_tween = create_tween()
	hit_tween.tween_property(
		animated_sprite_2d,
		"modulate",
		Color.RED,
		0.2
	)
	await hit_tween.finished
	animated_sprite_2d.modulate = default_modulate_color

func _die(attack: Attack) -> void:
	die.emit(self, attack)
	_show_gold_dropped()
	RunContext.economy.gold += gold_value
	queue_free()

func _show_damage(attack: Attack) -> void:
	const MAX_OFFSET: int = 30
	var damage_numbers: DamageNumbers = DAMAGE_NUMBERS.instantiate()
	var base_position: Vector2 = target_position
	
	var random_offset_x: int = randi_range(-MAX_OFFSET, MAX_OFFSET)
	var random_offset_y: int = randi_range(-MAX_OFFSET, MAX_OFFSET)
	
	damage_numbers.global_position = base_position + Vector2(random_offset_x, random_offset_y)
	
	get_tree().root.add_child(damage_numbers)
	damage_numbers.set_attack(attack)

func _show_gold_dropped() -> void:
	var gold_droped: GoldDropped = GOLD_DROPPED.instantiate()
	get_tree().root.add_child(gold_droped)
	gold_droped.set_gold(gold_value)
	gold_droped.global_position = target_position

func _on_target_reached() -> void:
	target_reached.emit(self)
	queue_free()
	
func _set_health(new_value: float) -> void:
	health = new_value
	health_bar.update_health(health)
