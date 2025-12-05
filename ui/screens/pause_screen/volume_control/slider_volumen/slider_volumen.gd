extends HSlider

@export
var bus_name: String
var bus_index: int

func _ready() -> void:
	bus_index = AudioServer.get_bus_index(bus_name)
	value_changed.connect(_on_value_changed)
	value = db_to_linear(
		AudioServer.get_bus_volume_db(bus_index)
	)

func _on_value_changed(new_value: float) -> void:
	AudioServer.set_bus_volume_db(
		bus_index,
		linear_to_db(new_value)
	)

func _on_button_pressed() -> void:
	get_parent().get_parent().get_parent().get_parent().get_parent().queue_free()
	get_tree().paused = not get_tree().paused
