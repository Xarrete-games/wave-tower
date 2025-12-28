class_name FireBallSprite extends Sprite2D

const COLOR_1: Color = Color("#ff00ff")
const COLOR_2: Color = Color("#0000ff")

var color_tween: Tween

func _ready() -> void:
	_start_color_tween()


func _start_color_tween() -> void:
	modulate = COLOR_1

	color_tween = create_tween()
	color_tween.set_loops() # infinito
	color_tween.set_trans(Tween.TRANS_SINE)
	color_tween.set_ease(Tween.EASE_IN_OUT)

	color_tween.tween_property(
		self,
		"modulate",
		COLOR_2,
		0.15
	)
	color_tween.tween_property(
		self,
		"modulate",
		COLOR_1,
		0.15
	)