class_name GreenProjectile extends Area2D

@export var damage = 5
@export var speed = 400

var _direction: Vector2

@onready var waves: Node2D = $Waves

func set_direction(dir: Vector2) -> void:
	_direction = dir
	look_at(global_position + _direction)

func _ready():
	for animated_sprite: AnimatedSprite2D in waves.get_children():
		animated_sprite.play("anim")
	
func _process(delta: float) -> void:
	global_position += _direction * speed * delta

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.get_damage(damage)

func _on_duration_timeout() -> void:
	queue_free()
