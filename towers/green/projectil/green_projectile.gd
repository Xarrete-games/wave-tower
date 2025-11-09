class_name GreenProjectile extends Area2D


@export var speed = 400

var _direction: Vector2
var _damage: float = 5

@onready var waves: Node2D = $Waves
@onready var green_attack: AudioStreamPlayer2D = $GreenAttack

func set_direction(dir: Vector2, damage: float) -> void:
	_direction = dir
	_damage = damage
	look_at(global_position + _direction)

func _ready():
	for animated_sprite: AnimatedSprite2D in waves.get_children():
		animated_sprite.play("anim")
	green_attack.play()
	
func _process(delta: float) -> void:
	global_position += _direction * speed * delta

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.get_damage(_damage)

func _on_duration_timeout() -> void:
	queue_free()
