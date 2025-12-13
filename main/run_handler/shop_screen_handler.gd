class_name ShopScreenHandler extends Node

signal shop_closed

const SHOP_SCREEN: PackedScene = preload("uid://bpf44acq183yv")

func open_shop(event_layer: CanvasLayer) -> void:
	pass
	var relics: Array[ItemOffer] = RunContext.offers_manager.create_relic_offers(5)
	var shop_screen: ShopScreen = SHOP_SCREEN.instantiate()
	event_layer.add_child(shop_screen)
	shop_screen.set_relics(relics)
	shop_screen.item_purchase.connect(_on_Item_purchased)

	shop_screen.tree_exited.connect(shop_closed.emit)

func _on_Item_purchased(item_offer: ItemOffer) -> void:
	Score.gold -= item_offer.price
	var item = item_offer.create_item()
	if item is Relic:
		RelicsManager.add_relic(item)