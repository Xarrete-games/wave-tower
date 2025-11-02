class_name Enemy extends CharacterBody2D

@onready var animation_player: AnimationPlayer = $AnimationPlayer


@export var speed: float = 60.0
@export var max_healt: float = 100
@export var gold_value: int = 1

signal die(ememy: Enemy)
signal reached_target(enemy: Enemy)

var health: float
var _path_follow: PathFollow2D

func get_damage(damage: float) -> void:
	health -= damage
	#health_bar.set_current_health(health)
	if health <= 0:
		
		_die()
func set_path_follow(path_follow: PathFollow2D) -> void:
	_path_follow = path_follow

func _ready() -> void:
	#animation_player.play("mixamo_com")
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
	
	var path_pos_2d = _path_follow.position
	position = path_pos_2d
	

func _die() -> void:
	die.emit(self)
	#var blood = blood_resource.instantiate()
	#get_tree().root.add_child(blood)
	#blood.global_position = global_position
	queue_free()

func _on_target_reached() -> void:
	reached_target.emit(self)
	queue_free()
	
func _set_health(value: float) -> void:
	self.health = value
	#self.health_bar.set_max_health(value)
	#self.health_bar.set_current_health(value)
