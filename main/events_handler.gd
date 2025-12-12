class_name EventsHandler extends Node

signal event_finished()

const EVENT_SCREEN: PackedScene = preload("uid://boinmjkgpsi1q")
const SHOP_SCREEN: PackedScene = preload("uid://bpf44acq183yv")

@export var events: Array[Event]
@export var event_layer: CanvasLayer

func show_events() -> void:
	var event_screen: EventScreen = EVENT_SCREEN.instantiate()
	event_layer.add_child(event_screen)
	event_screen.set_events(events)
	event_screen.event_selected.connect(_on_event_selected, CONNECT_ONE_SHOT)
	await event_finished

func _on_event_selected(event: Event) -> void:
	if event.type == Event.Type.SHOP:
		_open_shop()
	else:
		push_error("[EVENT HANDLER] Invalid event")

func _open_shop() -> void:
	pass
	var relics: Array[ItemOffer] = RunContext.offers_manager.create_relic_offers(5)
	var shop_screen: ShopScreen = SHOP_SCREEN.instantiate()
	event_layer.add_child(shop_screen)
	shop_screen.set_relics(relics)
	shop_screen.item_purchase.connect(func(relic: Relic):
		 RelicsManager.add_relic(relic)

	)

	
	shop_screen.tree_exited.connect(event_finished.emit)
