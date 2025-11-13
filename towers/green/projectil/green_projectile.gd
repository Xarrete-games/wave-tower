class_name GreenProjectile extends Area2D

@export var animation_time = 0.25
@export var speed = 400

var _direction: Vector2
var _damage: float = 5
var _is_expanding: bool = true

@onready var green_ring_3: Sprite2D = $Waves/GreenRing3
@onready var waves: Node2D = $Waves
@onready var green_attack: AudioStreamPlayer2D = $GreenAttack
@onready var ring_animation_timer: Timer = $RingAnimationTimer

func set_direction(dir: Vector2, damage: float) -> void:
	_direction = dir
	_damage = damage
	look_at(global_position + _direction)

func _ready():
	ring_animation_timer.wait_time = animation_time
	ring_animation_timer.start()
	
func _process(delta: float) -> void:
	global_position += _direction * speed * delta

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.get_damage(_damage)

func _on_duration_timeout() -> void:
	queue_free()

func _on_ring_animation_timer_timeout() -> void:
	# define start and end scale
	var end_scale: Vector2
	
	if _is_expanding:
		end_scale = Vector2(0.30, 0.30)
	else:
		end_scale = Vector2(0.15, 0.15)
		
	_is_expanding = !_is_expanding
	
	# animate waves
	for wave in waves.get_children():
		var new_tween: Tween = create_tween()
		
		new_tween.tween_property(
			wave, 
			"scale", 
			end_scale, 
			animation_time
		).set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_OUT)
