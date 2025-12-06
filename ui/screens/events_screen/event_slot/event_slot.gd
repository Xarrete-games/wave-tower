class_name EventSlot extends Control

signal event_pressed(event: Event)

@export var event_texture: TextureRect
@export var description_label: RichTextLabel
@export var title_lable: Label

var _event: Event

func set_event(event: Event) -> void:
	event_texture.texture = event.icon
	description_label.text = event.description
	title_lable.text = event.id
	_event = event
	
func _on_gui_input(event: InputEvent) -> void:
	if Utils.is_left_click_event(event):
		event_pressed.emit(_event)
		AudioManager.play_button_click()

func _on_mouse_entered() -> void:
	event_texture.custom_minimum_size = Vector2(230, 230)
	AudioManager.play_button_hover()

func _on_mouse_exited() -> void:
	event_texture.custom_minimum_size = Vector2(200, 200)
