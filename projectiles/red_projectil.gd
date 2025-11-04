@tool
class_name RedProjectil extends Node2D

@export var cast_speed: int = 7000
@export var max_lenght: int = 1400
@export var color: Color = Color.RED: set = set_color
@export var is_casting: bool = false: set = set_is_casting
@export var growth_time: float = 0.1

# El láser apunta a esta posición global. Puede ser null.
# Usamos un setter para activar/desactivar la visualización
var target_position: Vector2 = Vector2.ZERO: set = set_target_position 

var tween: Tween = null
var current_laser_length: float = 0.0 # Longitud actual para el efecto de crecimiento

@onready var line_2d: Line2D = $Line2D
@onready var line_width: float = line_2d.width

func _ready():
	# Aseguramos que la configuración inicial se aplique
	set_color(color)
	# No necesitamos set_is_casting aquí si el valor inicial ya está en false
	set_physics_process(is_casting)


func _physics_process(delta: float) -> void:
	if not is_casting || target_position == Vector2.ZERO:
		return
		
	# --- PASO 1: Rotar el láser hacia el objetivo ---
	# Rotamos el nodo completo para que el Line2D (que empieza en 0,0) apunte al target.
	look_at(target_position)

	# --- PASO 2: Calcular la longitud máxima del rayo ---
	var distance_to_target = global_position.distance_to(target_position)
	var final_length = min(distance_to_target, max_lenght)
	
	# --- PASO 3: Simular el crecimiento del láser (visual) ---
	# Movemos la longitud actual del láser hacia la longitud final calculada.
	current_laser_length = move_toward(
		current_laser_length,
		final_length,
		cast_speed * delta
	)
	
	# --- PASO 4: Actualizar la Line2D ---
	# La Line2D dibuja desde (0, 0) hasta (X, 0) en coordenadas locales.
	# X es la longitud actual del láser.
	line_2d.points[1] = Vector2(current_laser_length, 0.0)


# --- SETTERS ---

func set_target_position(new_position: Vector2) -> void:
	# Este setter maneja la posición del objetivo.
	# Si la posición es Vector2.ZERO (o si usas 'null' en tu lógica externa,
	# aunque GDScript prefiere valores iniciales), el láser debería apagarse o detenerse.
	target_position = new_position
	
	# Si recibimos un objetivo válido, aseguramos que esté casteando
	if target_position != Vector2.ZERO and not is_casting:
		set_is_casting(true)

func set_is_casting(new_value: bool) -> void:
	if is_casting == new_value:
		return
		
	is_casting = new_value
	set_physics_process(is_casting)
	
	if is_casting == false:
		dissapear()
	else:
		# Reiniciar la longitud para que vuelva a crecer cuando aparece.
		current_laser_length = 0.0
		appear()

func set_color(new_color: Color) -> void:
	color = new_color
	if line_2d:
		line_2d.modulate = new_color

# --- TWEENS (Visuales) ---

func dissapear() -> void:
	if not line_2d:
		return
	if tween and tween.is_running():
		tween.kill()
		
	tween = create_tween()
	# Animación de ancho a cero
	tween.tween_property(line_2d, "width", 0.0, growth_time * 2.0).from_current()
	# Opcional: Ocultar el nodo y reiniciar la longitud después de la animación
	tween.tween_callback(line_2d.hide)
	tween.tween_callback(func(): current_laser_length = 0.0)
	
func appear() -> void:
	if not line_2d:
		return
	line_2d.visible = true
	if tween and tween.is_running():
		tween.kill()
		
	tween = create_tween()
	# Animación de ancho de cero a ancho original
	tween.tween_property(line_2d, "width", line_width, growth_time * 2.0).from(0.0)
