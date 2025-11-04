@tool
class_name RedProjectil extends RayCast2D

@export var cast_speed: int = 7000
@export var max_lenght: int = 1400
@export var color: Color = Color.RED: set = set_color
@export var is_casting: bool = false: set = set_is_casting
@export var growth_time: float = 0.1

var tween: Tween = null

@onready var line_2d: Line2D = $Line2D
@onready var line_width: float = line_2d.width

func _ready():
	set_color(color)
	set_is_casting(is_casting)
	
func _physics_process(delta: float) -> void:
	
	if not is_casting:
		return
	look_at(target_position)

	
	var distance_to_target = global_position.distance_to(target_position)

	var ray_length = min(distance_to_target, max_lenght)
	
	# Aplicamos la longitud al target_position (solo en el eje X local)
	# y lo movemos gradualmente (efecto de crecimiento):
	target_position.x = move_toward(
		target_position.x,
		ray_length,
		cast_speed * delta
	)
	target_position.y = 0.0
	
	force_raycast_update()
	var laser_end_position := target_position
	
	if is_colliding():
		# Si choca, el final del láser es el punto de colisión (en coordenadas locales).
		laser_end_position = to_local(get_collision_point())
	else:
		print("NO COLLIDING")
		print(target_position)
		print(global_position)
	# Actualizar la Line2D para que dibuje hasta el punto final real.
	line_2d.points[1] = laser_end_position
	

func set_is_casting(new_value: bool) -> void:
	if is_casting == new_value:
		return
	is_casting = new_value
	
	set_physics_process(is_casting)
	
	if is_casting == false:
		target_position = Vector2.ZERO
		dissapear()
	else:
		appear()

func set_color(new_color: Color) -> void:
	color = new_color
	if line_2d:
		line_2d.modulate = new_color
	
func dissapear() -> void:
	if not line_2d:
		return
	if tween and tween.is_running():
		tween.kill()
	tween = create_tween()
	tween.tween_property(line_2d, "width", 0.0, growth_time * 2.0).from_current()
	tween.tween_callback(line_2d.hide)
	
func appear() -> void:
	if not line_2d:
		return
	line_2d.visible = true
	if tween and tween.is_running():
		tween.kill()
	tween = create_tween()
	tween.tween_property(line_2d, "width", line_width, growth_time * 2.0).from(0.0)
