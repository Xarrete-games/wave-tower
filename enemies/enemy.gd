class_name Enemy extends CharacterBody2D

signal die(ememy: Enemy)
signal target_reached(enemy: Enemy)

enum  EnemyType { NORMAL, FAST, TANK }

const GOLD_DROPPED = preload("uid://cxs4ar5enx4mn")

@export var speed: float = 80.0
@export var max_healt: float = 20
@export var gold_value: int = 1
@export var damage: int = 1

var health: float
var _path_follow: PathFollow2D
var hit_tween: Tween

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var explosion: AnimatedSprite2D = $Explosion
@onready var health_bar: HealthBar = $HealthBar
@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var gold_dropped_pos: Marker2D = $GoldDroppedPos

func get_damage(damage: float) -> void:
	_set_health(health - damage)

	if hit_tween and hit_tween.is_running():
		hit_tween.kill()

	hit_tween = create_tween()
	hit_tween.set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT)

	animated_sprite_2d.modulate = Color.RED
	hit_tween.tween_property(animated_sprite_2d, "modulate", Color.WHITE, 0.2)

	if health <= 0:
		_die()
		
func set_path_follow(path_follow: PathFollow2D) -> void:
	_path_follow = path_follow

func _ready() -> void:
	health_bar.set_max_health(max_healt)
	_set_health(max_healt)
	
func _process(delta):
	if _path_follow == null:
		return
	#increse progress
	_path_follow.progress += speed * delta
	
	# target reached
	if _path_follow.progress_ratio >= 0.99:
		_on_target_reached()
		return
	
	var path_global_pos = _path_follow.global_position
	
	global_position = path_global_pos
	
func _die() -> void:
	die.emit(self)
	_show_gold_dropped()
	Score.add_gold(gold_value)
	_path_follow.queue_free()
	queue_free()

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
