class_name EventsOptionsScreenHandler extends Node

@export var events_options_screen: PackedScene


func show_options_event(event: EventData, p_event_layer: CanvasLayer) -> void:
	var options_screen: EventOptionsScreen = events_options_screen.instantiate()
	
	if not is_inside_tree():
		await ready
	await get_tree().process_frame

	p_event_layer.add_child(options_screen)
	options_screen.set_event(event)
	await options_screen.event_completed
