@tool
class_name RedProjectil extends Node2D

@export var cast_speed: int = 7000
@export var color: Color = Color.RED: set = set_color
@export var growth_time: float = 0.1

var _target: Enemy 
var _attack: Attack
var tween: Tween = null
var current_laser_length: float = 0.0
var is_casting: bool = false

@onready var line_2d: Line2D = $Line2D
@onready var line_width: float = line_2d.width
@onready var red_attack_start: AudioStreamPlayer2D = $RedAttack_start
@onready var red_attack_loop: AudioStreamPlayer2D = $RedAttack_loop
@onready var red_attack_finish: AudioStreamPlayer2D = $RedAttack_finish
@onready var fire_particles: CPUParticles2D = $FireParticles

# VARIABLES PARA DIBUJAR LA ONDA
var amplitude := 10			# Altura máxima (+/-)
var speed := -16 				# Velocidad de la oscilación
var phase_offset := PI / 3	# Desfase entre puntos


func _ready():
	set_color(color)
	fire_particles.emitting = false

func _physics_process(delta: float) -> void:
	if not is_casting or not _target:
		return
	# --- STEP 1: rotate the laser beam towards the target ---
	look_at(_target.global_position)
	# --- STEP 2: The laser's length is modified depending on the enemy's position; 
	# --- the speed at which the laser changes length depends on cast_speed. ---
	var distance_to_target = global_position.distance_to(_target.global_position)
	#update fire partivles position
	fire_particles.global_position = _target.global_position
	
	current_laser_length = move_toward(
		current_laser_length,
		distance_to_target,
		cast_speed * delta
	)
	var imax = int (line_2d.get_point_count() - 1)
	var time := Time.get_ticks_msec() / 1000.0
	for i in range(line_2d.get_point_count()):
		var pos = line_2d.get_point_position(i)
		pos.x = i * current_laser_length /imax
		pos.y = sin(time * speed + i * phase_offset) * amplitude * sin(PI * i / imax)
		line_2d.set_point_position(i, pos)
	
	#line_2d.points[1] = Vector2(current_laser_length, 0.0)

#stop the laser
func stop() -> void:
	_target = null
	_set_is_casting(false)

func set_attack(new_attack: Attack) -> void:
	_attack = new_attack

#set the target to follow
func set_target(target: Enemy) -> void:
	if target == _target:
		return	
	
	_target = target
	
	if not is_casting:
		_set_is_casting(true)

#damages the current target	
func hit_target() -> void:
	if not _target:
		return
	_target.get_damage(_attack)

func set_color(new_color: Color) -> void:
	color = new_color
	if line_2d:
		line_2d.modulate = new_color

func _set_is_casting(new_value: bool) -> void:
	if is_casting == new_value:
		return
		
	is_casting = new_value
	

	if is_casting == false:
		_dissapear()
		return
	else:
		current_laser_length = 0.0
		_appear()

#animation to make the laser disappear
func _dissapear() -> void:
	if red_attack_loop.is_inside_tree():
		red_attack_loop.stop()
		
	if red_attack_finish.is_inside_tree():
		red_attack_finish.play()
		
	if not line_2d:
		return
		
	if tween and tween.is_running():
		tween.kill()
		
	fire_particles.emitting = false	
	tween = create_tween()
	tween.tween_property(line_2d, "width", 0.0, growth_time * 2.0).from_current()
	tween.tween_callback(line_2d.hide)
	tween.tween_callback(func(): current_laser_length = 0.0)

#animation to make the laser appear
func _appear() -> void:
	red_attack_start.play()
	if not line_2d:
		return
	line_2d.visible = true
	if tween and tween.is_running():
		tween.kill()
		
	fire_particles.emitting = true	
	tween = create_tween()
	tween.tween_property(line_2d, "width", line_width, growth_time * 2.0).from(0.0)
	await red_attack_start.finished
	red_attack_loop.play()
	
	
