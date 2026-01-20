class_name ShopSlotIcon extends SubViewportContainer

@onready var root_2d: Node2D = $SubViewport/Root2D
@onready var relic_texture: Sprite2D = $SubViewport/Root2D/RelicTexture
@onready var hexagon: Polygon2D = $SubViewport/Root2D/Hexagon

func _ready() -> void:
	root_2d.position = root_2d.get_viewport().size * 0.5
	icon_normal_size()

func set_icon(texture: Texture2D) -> void:
	relic_texture.texture = texture

func set_background_color(color: Color) -> void:
	hexagon.color = color

func increased_icon_size() -> void:
	_set_sprite_pixel_size(relic_texture, Vector2(80, 80))

func icon_normal_size() -> void:
	_set_sprite_pixel_size(relic_texture, Vector2(64, 64))

func _set_sprite_pixel_size(sprite: Sprite2D, target_size: Vector2):
	if not sprite.texture:
		return

	var tex_size = sprite.texture.get_size()
	sprite.scale = target_size / tex_size