#uttils
extends Node

func is_left_click_event(event: InputEvent) -> bool:
	return event is InputEventMouseButton and \
		event.button_index == MOUSE_BUTTON_LEFT and \
		not event.is_pressed()
