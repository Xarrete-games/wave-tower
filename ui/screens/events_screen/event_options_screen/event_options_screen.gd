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
	for option_text in event_data.options:
		var button_option: EventOptionButton = button_option_scene.instantiate()
		buttons_container.add_child(button_option)
		button_option.text = option_text
		button_option.option_index = index
		index += 1
		button_option.option_selected.connect(_on_option_selected
		)
	if event_data.runtime_script:
		event_script_instance = event_data.runtime_script.new()

func _on_option_selected(option_index: int) -> void:
	if event_script_instance:
		event_script_instance.handle_response(option_index)
	event_completed.emit()
	queue_free()
