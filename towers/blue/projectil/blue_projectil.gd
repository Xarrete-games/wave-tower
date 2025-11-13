extends Area2D
class_name BlueProjectil

const BLUE_EXPLOSION = preload("uid://bntnbljmfy1p4")

@export var expand_speed: float = 300.0
@export var y_scale: float = 0.5
@export var thickness: float = 20.0
@export var color: Color = Color(0.302, 0.173, 1.0, 1.0)

var _damage: float
var _max_area_range: float

@onready var collision_shape: CollisionShape2D = $CollisionShape
@onready var blue_attack: AudioStreamPlayer2D = $BlueAttack

var radius: float = 0.0
var shape: CircleShape2D

func _ready() -> void:
	shape = collision_shape.shape as CircleShape2D
	shape.radius = 0.0
	blue_attack.play()

func _process(delta: float) -> void:
	# update collision radius
	radius += expand_speed * delta
	shape.radius = radius
	
	# update visual circle
	queue_redraw()

	# queue_free at max range
	if radius >= _max_area_range:
		queue_free()

func set_stats(damage: float, area_range: float) -> void:
	_damage = damage
	_max_area_range = area_range

func _draw() -> void:
	var radius_progress: float = clamp(radius / _max_area_range, 0.0, 1.0)
	var fade_start_threshold := 0.8
	var alpha_fade: float

	if radius_progress < fade_start_threshold:
		alpha_fade = 1.0
	else:
		var fade_progress = (radius_progress - fade_start_threshold) / (1.0 - fade_start_threshold)
		alpha_fade = pow(1.0 - fade_progress, 2.0)

	#Apply fade
	var wave_color = Color(color.r, color.g, color.b, color.a * alpha_fade)

	#set Y scale to 0.5 for isometric
	var y_scale_transform := Transform2D().scaled(Vector2(1, y_scale))
	draw_set_transform_matrix(y_scale_transform)

	# draw the arc
	draw_arc(Vector2.ZERO, radius, 0, TAU, 64, wave_color, thickness)

func _on_body_entered(body: Node2D) -> void:
	if body is Enemy:
		var explosion: CPUParticles2D = BLUE_EXPLOSION.instantiate()
		body.get_damage(_damage)
		add_child(explosion)
		explosion.global_position = body.global_position
		explosion.emitting = true
		await explosion.finished
		explosion.queue_free()
		
