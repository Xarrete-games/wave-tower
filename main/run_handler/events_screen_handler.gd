class_name EventsScreenHandler extends Node

signal event_finished()

const EVENT_SCREEN: PackedScene = preload("uid://boinmjkgpsi1q")


@onready var shop_screen_handler: ShopScreenHandler = $ShopScreenHandler
@onready var events_options_screen_handler: EventsOptionsScreenHandler = $EventsOptionsScreenHandler
@onready var choose_relic_screen_handler: ChooseRelicScreenHandler = $ChooseRelicScreenHandler

func show_event_selected(event: EventData, event_layer: CanvasLayer) -> void:
	match event.type:
		EventData.Type.CHOOSE_RELIC:
			await choose_relic_screen_handler.show_choose_relic_event(event_layer)
		EventData.Type.SHOP:
			shop_screen_handler.open_shop(event_layer)
			await  shop_screen_handler.shop_closed
		EventData.Type.OPTIONS:
			await events_options_screen_handler.show_options_event(event, event_layer)
	event_finished.emit()
