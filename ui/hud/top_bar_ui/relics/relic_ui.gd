class_name RelicUI extends Control

const semi_transparent_color: Color = Color(1, 1, 1, 0.5)
const opaque_color: Color = Color(1, 1, 1, 1)

@onready var texture: TextureRect = $VBoxContainer/MarginContainer/texture
@onready var amount: Label = $VBoxContainer/MarginContainer/amount

var relic: Relic

func set_relic(p_relic: Relic) -> void:
	texture.texture = p_relic.data.icon
	self.relic = p_relic
	if p_relic.disabled:
		texture.modulate = semi_transparent_color
	else:
		texture.modulate = opaque_color
	
func _on_mouse_entered() -> void:
	HintManager.show_hint(self, relic.data.description, relic.data.display_name)

func _on_mouse_exited() -> void:
	HintManager.remove_hint(self)
