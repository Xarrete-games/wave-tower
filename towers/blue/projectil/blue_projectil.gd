extends Area2D
class_name BlueProjectil

@export var duration: float = 2

var _damage: float
var _area_range: float

@onready var collision_shape: CollisionShape2D = $CollisionShape
@onready var sprite_2d: Sprite2D = $Sprite2D
@onready var blue_attack: AudioStreamPlayer2D = $BlueAttack

var elapsed := 0.0
var shape: CircleShape2D

func _ready() -> void:
	shape = collision_shape.shape
	shape.radius = 0.0
	sprite_2d.scale = Vector2.ZERO
	blue_attack.play()

func _process(delta: float) -> void:
	elapsed += delta
	var t: float = clamp(elapsed / duration, 0.0, 1.0)

	# radio actual en píxels (va de 0 a max_radius)
	var current_radius: float = lerp(0.0, _area_range, t)
	shape.radius = current_radius

	# Escalar el sprite en función del tamaño real de la textura
	if sprite_2d.texture:
		var tex_size: Vector2 = sprite_2d.texture.get_size()
		# suponemos textura cuadrada; si no, usamos el mayor para asegurar cobertura
		var base_tex_radius: float = max(tex_size.x, tex_size.y) * 0.5
		# evita dividir por 0
		if base_tex_radius > 0.0:
			var desired_scale: float = current_radius / base_tex_radius
			sprite_2d.scale = Vector2.ONE * desired_scale

	# Desvanecimiento vinculado a la fracción de radio (opcional)
	sprite_2d.modulate.a = 1.0 - (current_radius / _area_range)

	# si llegamos al final, destruir
	if t >= 1.0:
		queue_free()

func set_stats(damage: float, area_range: float) -> void:
	_damage = damage
	_area_range = area_range

func _on_body_entered(body: Node2D) -> void:
	if body is Enemy:
		body.get_damage(_damage)
