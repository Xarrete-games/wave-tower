class_name EventOptionsScreen extends Control

signal event_completed()

@export var button_option_scene: PackedScene
@export var buttons_container: Control
@export var texture_rect: TextureRect
@export var title_label: Label
@export var description_label: Label

var event_script_instance: EventScript

func set_event(event_data: EventData) -> void:
	# Set UI elements
	title_label.text = event_data.title
	description_label.text = event_data.description
	texture_rect.texture = event_data.texture_background
	
	# Clear previous buttons
	for chil in buttons_container.get_children():
		chil.queue_free()

	# Create option buttons
	var index: int = 0

	if not event_data.runtime_script:
		push_error("Event data %s has no runtime script assigned." % event_data.id)
		return
	event_script_instance = event_data.runtime_script.new()

	for option_data in event_script_instance.get_options():
		var button_option: EventOptionButton = button_option_scene.instantiate()
		buttons_container.add_child(button_option)
		button_option.text = option_data.text
		button_option.option_data = option_data.data
		button_option.name = "OptionButton_%d" % index
		index += 1
		if option_data.disabled:
			button_option.disable_option()

		button_option.option_selected.connect(_on_option_selected
		)
	
func _on_option_selected(data: Variant) -> void:
	if event_script_instance:
		event_script_instance.handle_response(data)
	event_completed.emit()
	queue_free()
