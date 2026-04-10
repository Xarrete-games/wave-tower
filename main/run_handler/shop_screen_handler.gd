class_name ShopScreenHandler extends Node

signal shop_closed

const SHOP_SCREEN: PackedScene = preload("uid://bpf44acq183yv")

func open_shop(event_layer: CanvasLayer) -> void:
	pass
	var relics: Array = RunContext.offers_manager.create_relic_offers(5)
	var consumables: Array = RunContext.offers_manager.create_consumables_offers(5)
	var shop_screen: ShopScreen = SHOP_SCREEN.instantiate()
	event_layer.add_child(shop_screen)
	shop_screen.set_relics(relics)
	shop_screen.set_consumables(consumables)
	shop_screen.item_purchase.connect(_on_item_purchased)

	shop_screen.tree_exited.connect(shop_closed.emit)

func _on_item_purchased(item_offer: ItemOffer) -> void:
	RunContext.offers_manager.purchase_offer(item_offer)
