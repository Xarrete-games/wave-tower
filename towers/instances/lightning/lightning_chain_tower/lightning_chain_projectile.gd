class_name LightningChainProjectile extends Node2D

@export var extend_speed: float = 1500.0
@export var max_bounces: int = 3
@export var bounce_delay: float = 0.1

var enemies_in_range: Array[Enemy] = []
var _target: Enemy
var _current_length: float = 0.0
var _max_length: float = 0.0
var _hit: bool = false
var _bounces_done: int = 0
var _hit_enemies: Array[Enemy] = []
var _start_global: Vector2
var _end_global: Vector2
var _attack: Attack

@onready var sparks: GPUParticles2D = %Sparks
@onready var flare: GPUParticles2D = %Flare
@onready var area_2d: Area2D = %Area
@onready var line: Line2D = %Line

func _ready() -> void:
	line.top_level = true
	area_2d.top_level = true

func _process(delta: float) -> void:
	if not is_instance_valid(_target):
		if not _hit:
			queue_free()
		return

	if _hit:
		return

	_end_global = _target.target_position
	_max_length = _start_global.distance_to(_end_global)

	_current_length += extend_speed * delta
	_current_length = min(_current_length, _max_length)

	var dir: Vector2 = (_end_global - _start_global).normalized()
	var tip_global: Vector2 = _start_global + dir * _current_length

	line.set_point_position(1, tip_global)
	area_2d.global_position = tip_global

	if _current_length >= _max_length and not _hit:
		_on_hit()

func set_target(target: Enemy, attack: Attack, bounces: int) -> void:
	max_bounces = bounces
	_target = target
	_attack = attack
	_start_global = _end_global if _end_global != Vector2.ZERO else global_position
	_end_global = target.global_position

	_max_length = _start_global.distance_to(_end_global)
	_current_length = 0.0
	_hit = false

	line.clear_points()
	line.add_point(_start_global)
	line.add_point(_start_global)

	flare.visible = false
	sparks.visible = false

func _on_hit() -> void:
	_hit = true
	_target.apply_damage(_attack)
	flare.visible = true
	sparks.visible = true

	_hit_enemies.append(_target)
	_bounces_done += 1

	# wait before trying to bounce
	await get_tree().create_timer(bounce_delay, false).timeout

	_try_bounce()

func _try_bounce() -> void:
	if _bounces_done >= max_bounces:
		queue_free()
		return

	var next_enemy: Enemy = _get_closest_valid_enemy()
	if next_enemy == null:
		queue_free()
		return

	set_target(next_enemy, _attack, max_bounces)

func _get_closest_valid_enemy() -> Enemy:
	var closest: Enemy = null
	var min_dist: float = INF

	for enemy: Enemy in enemies_in_range:
		if not is_instance_valid(enemy):
			continue
		if not enemy.is_inside_tree():
			continue
		if enemy in _hit_enemies:
			continue

		var d = enemy.global_position.distance_squared_to(_end_global)
		if d < min_dist:
			min_dist = d
			closest = enemy

	return closest

func _on_area_2d_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	enemies_in_range.erase(enemy)

func _on_area_2d_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemies_in_range.append(enemy)
	enemy.tree_exited.connect(
		func():
			enemies_in_range.erase(enemy)
	)
