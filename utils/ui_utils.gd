#uttils
extends Node

const COLOR_PALETTE: ColorPalette = preload("uid://dbyjsuchchdht")


var primary_color: Color = COLOR_PALETTE.colors[0]
var secondary_color: Color = COLOR_PALETTE.colors[1]
var accent_color: Color = COLOR_PALETTE.colors[2]


func is_left_click_event(event: InputEvent) -> bool:
	return event is InputEventMouseButton and \
		event.button_index == MOUSE_BUTTON_LEFT and \
		not event.is_pressed()

func is_right_click_event(event: InputEvent) -> bool:
	return event is InputEventMouseButton and \
		event.button_index == MOUSE_BUTTON_RIGHT and \
		not event.is_pressed()
