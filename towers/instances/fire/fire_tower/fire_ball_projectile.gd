class_name FireBallProjectile
extends Node2D

const SPEED: float = 400.0
const HIT_RADIUS: float = 12.0

const COLOR_1: Color = Color("#ff00ff")
const COLOR_2: Color = Color("#0000ff")

var enemy: Enemy
var attack: Attack
var color_tween: Tween

@onready var fire_ball_sprite: Sprite2D = $FireBallSprite

func _ready() -> void:
	
	_start_color_tween()



func _process(delta: float) -> void:
	if not is_instance_valid(enemy):
		queue_free()
		return
	
	var target_position: Vector2 = enemy.global_position

	var direction: Vector2 = (target_position - global_position)
	var distance: float = direction.length()

	if distance <= HIT_RADIUS:
		enemy.apply_damage(attack)
		queue_free()
		return

	global_position += direction.normalized() * SPEED * delta


func set_target(p_enemy: Enemy, p_attack: Attack) -> void:
	enemy = p_enemy
	attack = p_attack

func _start_color_tween() -> void:
	fire_ball_sprite.modulate = COLOR_1

	color_tween = create_tween()
	color_tween.set_loops() # infinito
	color_tween.set_trans(Tween.TRANS_SINE)
	color_tween.set_ease(Tween.EASE_IN_OUT)

	color_tween.tween_property(
		fire_ball_sprite,
		"modulate",
		COLOR_2,
		0.15
	)
	color_tween.tween_property(
		fire_ball_sprite,
		"modulate",
		COLOR_1,
		0.15
	)