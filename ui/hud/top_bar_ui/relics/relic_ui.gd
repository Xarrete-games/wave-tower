class_name RelicUI extends Control

const semi_transparent_color: Color = Color(1, 1, 1, 0.5)
const opaque_color: Color = Color(1, 1, 1, 1)


@onready var texture: TextureRect = $VBoxContainer/MarginContainer/texture
@onready var amount_label: Label = $VBoxContainer/MarginContainer/amount
@onready var animation_player: AnimationPlayer = $AnimationPlayer

var relic: Relic

func _ready() -> void:
	animation_player.play("on_enter")

func set_relic(p_relic: Relic) -> void:
	texture.texture = p_relic.data.icon
	self.relic = p_relic
	if p_relic.disabled:
		texture.modulate = semi_transparent_color
	else:
		texture.modulate = opaque_color
	
	amount_label.visible = p_relic.data.show_counter
	if p_relic.data.show_counter:
		amount_label.text = str(p_relic.counter)

func _on_mouse_entered() -> void:
	HintManager.show_hint(self, relic.data.description, relic.data.display_name)

func _on_mouse_exited() -> void:
	HintManager.remove_hint(self)
