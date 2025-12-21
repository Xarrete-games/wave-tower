class_name Enemy extends CharacterBody2D

signal die(ememy: Enemy)
signal target_reached(enemy: Enemy)

enum  Type { NORMAL, BUBA, TANK, GOLEM, SKELETON, BLACK_GOLEM, BLACK_SKELETON, GOLD_SKELETON, INVOKER }

const GOLD_DROPPED = preload("uid://cxs4ar5enx4mn")
const DAMAGE_NUMBERS = preload("uid://bkiu4qgh3ug1m")

@export var base_speed: float = 80
@export var max_healt: float = 20
@export var base_gold_value: int = 1
@export var damage: int = 1

var health: float
var hit_tween: Tween
var gold_value: int
var _last_is_right_direction: bool = false
var _path_follow: PathFollow2D
var default_modulate_color: Color = Color.WHITE
# speed
var _base_speed: float = 100.0
var _speed_mult: float = 1.0

var speed: float:
	get: return _base_speed * _speed_mult
	set(value):
		_base_speed = value

var speed_mult: float:
	get: return _speed_mult
	set(value):
		_speed_mult = value

var progress_ratio: float:
	get:
		if _path_follow == null:
			return 0.0
		return _path_follow.progress_ratio

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var explosion: AnimatedSprite2D = $Explosion
@onready var health_bar: HealthBar = $HealthBar
@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var numbers_displayed_pos: Marker2D = $NumbersDisplayedPos
@onready var gold_dropped_pos: Marker2D = $GoldDroppedPos
@onready var collision_shape_2d: CollisionShape2D = $CollisionShape2D
# debuff
@onready var debuff_handler: DebuffHandler = $DebuffHandler

func _ready() -> void:
	disable()
	speed = base_speed
	health_bar.set_max_health(max_healt)
	_set_health(max_healt)
	gold_value = base_gold_value + RunContext.economy.extra_gold_dropped
	RunContext.economy.extra_gold_dropped_change.connect(
		func(value): gold_value = base_gold_value + value)

func _process(delta: float):
	debuff_handler.update_all(self, delta)
	if _path_follow == null:
		return
	# save previous position	
	var previous_global_x = global_position.x
	var previous_global_y = global_position.y
	#increse progress
	_path_follow.progress += speed * delta
	
	# target reached
	if _path_follow.progress_ratio >= 0.99:
		_on_target_reached()
		return
	
	var path_global_pos = _path_follow.global_position
	
	global_position = path_global_pos
	
	# FLIP SPRITE
	var is_right_direction = global_position.x > previous_global_x
	if is_right_direction != _last_is_right_direction:
		animated_sprite_2d.flip_h = !animated_sprite_2d.flip_h
		_last_is_right_direction = is_right_direction
	
	# handle animation
	var animation = "top right" if previous_global_y > global_position.y else "down right"
	if animated_sprite_2d.animation != animation:
		animated_sprite_2d.play(animation)
	enable()
	
func set_path_follow(path_follow: PathFollow2D) -> void:
	_path_follow = path_follow

func disable() -> void:
	animated_sprite_2d.visible = false
	collision_shape_2d.disabled = true
	health_bar.visible = false

func enable() -> void:
	animated_sprite_2d.visible = true
	collision_shape_2d.disabled = false
	health_bar.visible = true
# --------------------
# --- HEALT ---
# --------------------
# percentage of remaining heal
func get_percentage_remaining_health() -> float:
	if max_healt <= 0:
		return 0.0
	var health_ratio: float = health / max_healt
	var percentage: float = health_ratio * 100.0
	
	return min(100.0, percentage)

func get_remaining_health() -> float:
	return health

func apply_debuff(debuff: EnemyDebuff) -> void:
	debuff_handler.add_debuff(debuff, self)

func apply_damage(attack: Attack) -> void:
	_set_health(health - attack.damage)
	_play_hit_animation()
	_show_damage(attack)

	if health <= 0:
		_die()

func update_visual_color():
	animated_sprite_2d.modulate = default_modulate_color

func _play_hit_animation() -> void:
	if hit_tween and hit_tween.is_running():
		hit_tween.kill()
	animated_sprite_2d.modulate = Color.RED
	
	hit_tween = create_tween()
	hit_tween.set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT)

	hit_tween.tween_callback(func():
		animated_sprite_2d.modulate = default_modulate_color
	)
	hit_tween.tween_interval(0.2)

func _die() -> void:
	die.emit(self)
	_show_gold_dropped()
	RunContext.economy.gold += gold_value
	_path_follow.queue_free()
	queue_free()

func _show_damage(attack: Attack) -> void:
	const MAX_OFFSET: int = 30
	var damage_numbers: DamageNumbers = DAMAGE_NUMBERS.instantiate()
	var base_position: Vector2 = numbers_displayed_pos.global_position
	
	var random_offset_x: int = randi_range(-MAX_OFFSET, MAX_OFFSET)
	var random_offset_y: int = randi_range(-MAX_OFFSET, MAX_OFFSET)
	
	damage_numbers.global_position = base_position + Vector2(random_offset_x, random_offset_y)
	
	get_tree().root.add_child(damage_numbers)
	damage_numbers.set_attack(attack)

func _show_gold_dropped() -> void:
	var gold_droped: GoldDropped = GOLD_DROPPED.instantiate()
	get_tree().root.add_child(gold_droped)
	gold_droped.set_gold(gold_value)
	gold_droped.global_position = gold_dropped_pos.global_position

func _on_target_reached() -> void:
	target_reached.emit(self)
	queue_free()
	
func _set_health(new_value: float) -> void:
	health = new_value
	health_bar.update_health(health)
