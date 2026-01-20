class_name EventsScreenHandler extends Node

signal event_finished()

const EVENT_SCREEN: PackedScene = preload("uid://boinmjkgpsi1q")


@onready var shop_screen_handler: ShopScreenHandler = $ShopScreenHandler
@onready var events_options_screen_handler: EventsOptionsScreenHandler = $EventsOptionsScreenHandler
@onready var choose_relic_screen_handler: ChooseRelicScreenHandler = $ChooseRelicScreenHandler

# func _ready() -> void:
# 	events = DataLoader.get_all_events()

# func show_events(p_event_layer: CanvasLayer) -> void:
# 	event_layer = p_event_layer
# 	var event_screen: EventScreen = EVENT_SCREEN.instantiate()
	
# 	if not is_inside_tree():
# 		await ready
# 	await get_tree().process_frame
# 	event_layer.add_child(event_screen)
# 	event_screen.set_events(events)
# 	event_screen.event_selected.connect(_on_event_selected, CONNECT_ONE_SHOT)
# 	await event_finished

func show_event_selected(event: EventData, event_layer: CanvasLayer) -> void:
	match event.type:
		EventData.Type.CHOOSE_RELIC:
			await choose_relic_screen_handler.show_choose_relic_event(event_layer)
			event_finished.emit()
		EventData.Type.SHOP:
			shop_screen_handler.open_shop(event_layer)
			await  shop_screen_handler.shop_closed
			event_finished.emit()
		EventData.Type.OPTIONS:
			await events_options_screen_handler.show_options_event(event, event_layer)
			event_finished.emit()
