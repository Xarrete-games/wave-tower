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

var default_modulate_color: Color = Color.WHITE
# speed
var _base_speed: float = 100.0
var _speed_mult: float = 1.0
var is_right_direction: bool = true

var _enabled: bool = true
var _is_dead: bool = false
#var _is_freeze: bool = false

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

# --- Sistema de waypoints ---
# Array de puntos globales que el enemigo debe seguir en orden
var _waypoints: Array[Vector2] = []
# Índice del waypoint actual al que nos dirigimos
var _current_waypoint_index: int = 0
# Velocidad suavizada para steering
var _velocity: Vector2 = Vector2.ZERO
# Distancia mínima para considerar que llegamos a un waypoint
const WAYPOINT_ARRIVAL_THRESHOLD: float = 8.0
# Factor de suavizado del steering (mayor = giros más bruscos)
const STEERING_FACTOR: float = 8.0
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
	
	if _waypoints.size() > 0:
		_process_waypoints(delta)


## Procesa el movimiento siguiendo el sistema de waypoints.
## Usa steering para suavizar los giros entre puntos.
func _process_waypoints(delta: float) -> void:
	# ¿Hemos llegado al final de la ruta?
	if _current_waypoint_index >= _waypoints.size():
		_on_target_reached()
		return
	
	var target_point: Vector2 = _waypoints[_current_waypoint_index]
	var distance: float = global_position.distance_to(target_point)
	
	# ¿Hemos llegado al waypoint actual?
	if distance <= WAYPOINT_ARRIVAL_THRESHOLD:
		_current_waypoint_index += 1
		# Resetear velocidad para el siguiente segmento (opcional: quitar para transiciones más suaves)
		# _velocity = Vector2.ZERO
		return
	
	# Guardar posición anterior para animación
	var previous_global_x: float = global_position.x
	var previous_global_y: float = global_position.y
	
	# Steering: calcular velocidad deseada y suavizar
	var direction: Vector2 = (target_point - global_position).normalized()
	var desired_velocity: Vector2 = direction * speed
	_velocity = _velocity.move_toward(desired_velocity, STEERING_FACTOR * speed * delta)
	
	# Aplicar movimiento
	var displacement: Vector2 = _velocity * delta
	
	# Evitar overshoot: si el paso es mayor que la distancia, ir directo al punto
	if displacement.length() >= distance:
		global_position = target_point
	else:
		global_position += displacement
	
	# Flip del sprite según dirección horizontal
	_update_sprite_direction(previous_global_x)
	
	# Animación según dirección vertical
	_update_animation(previous_global_y)


## Actualiza el flip del sprite según la dirección de movimiento horizontal.
func _update_sprite_direction(previous_x: float) -> void:
	is_right_direction = global_position.x > previous_x
	if is_right_direction != _last_is_right_direction:
		animated_sprite_2d.flip_h = !animated_sprite_2d.flip_h
		_last_is_right_direction = is_right_direction


## Actualiza la animación según la dirección de movimiento vertical.
func _update_animation(previous_y: float) -> void:
	var animation: String = "top right" if previous_y > global_position.y else "down right"
	if animated_sprite_2d.animation != animation or not animated_sprite_2d.is_playing():
		animated_sprite_2d.play(animation)

## Establece la ruta de waypoints que el enemigo debe seguir.
## waypoints: Array de posiciones globales en orden [spawn, ..., target]
## Resetea el índice actual a 0 y la velocidad a cero.
func set_waypoints(waypoints: Array[Vector2]) -> void:
	_waypoints = waypoints.duplicate()
	_current_waypoint_index = 0
	_velocity = Vector2.ZERO


## Retorna true si el enemigo tiene waypoints pendientes.
func has_waypoints() -> bool:
	return _waypoints.size() > 0 and _current_waypoint_index < _waypoints.size()


## Retorna el waypoint actual al que se dirige (o Vector2.ZERO si no hay).
func get_current_waypoint() -> Vector2:
	if _current_waypoint_index < _waypoints.size():
		return _waypoints[_current_waypoint_index]
	return Vector2.ZERO


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
