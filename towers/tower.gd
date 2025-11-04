class_name Tower extends Node2D

@export var build_price: Price.TowerBuild = Price.TowerBuild.RED

const PROJECTIL = preload("uid://dipmywfwdmrlo")

var _targets_in_range: Array[Enemy] = [] 
var _current_target: Enemy
var _enabled: bool = false

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos
@onready var red_projectil: RedProjectil = $RedProjectil


func enable() -> void:
	_enabled = true
	red_projectil.set_is_casting(true)

func _ready():
	red_projectil.dissapear()


func _physics_process(delta: float) -> void:
	if not _current_target:
		return
	red_projectil.appear()
	red_projectil.target_position = _current_target.global_position

func _on_range_area_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.die.connect(_on_enemy_die)
	
	_targets_in_range.append(enemy)
	if _current_target == null:
		_current_target = enemy

func _on_range_area_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	_remove_target_and_get_next(enemy)

func _on_enemy_die(enemy: Enemy) -> void:
	_remove_target_and_get_next(enemy)

func _remove_target_and_get_next(enemy: Enemy) -> void:
	# disconnect signal
	if enemy.die.is_connected(_on_enemy_die):
		enemy.die.disconnect(_on_enemy_die)
	# remove enemy
	_targets_in_range.erase(enemy)
	#exit when enemy is not the target
	if enemy != _current_target:
		return
	if _targets_in_range.is_empty():
		red_projectil.dissapear()
		_current_target = null
	else:
		#logic to select next target
		_current_target = _targets_in_range[0]


func _on_attack_timer_timeout() -> void:
	if _current_target == null or not _enabled:
		return
	
	# add proyectile
	
	var projectile_instance = PROJECTIL.instantiate()
	get_tree().root.add_child(projectile_instance)
	
	# position
	projectile_instance.global_position = projectile_spawn_pos.global_position
	
	# direction
	var direction = (_current_target.global_position - global_position).normalized()
	projectile_instance.set_direction(direction)
