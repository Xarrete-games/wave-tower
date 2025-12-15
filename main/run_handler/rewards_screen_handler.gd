class_name RewardsScreenHandler extends Node

signal rewards_screen_close()

const REWARDS_SCREEN = preload("uid://bcxsfb0ox3gmq")
const REROLL_PRICE = 20

var rewards_screen: RewardsScreen

func show_rewards_screen(event_layer: CanvasLayer) -> void:
	rewards_screen = REWARDS_SCREEN.instantiate()
	var relic_offers = RunContext.offers_manager.create_relic_offers(3)
	event_layer.add_child(rewards_screen)
	rewards_screen.set_items_offer(relic_offers)
	rewards_screen.item_selected.connect(_on_item_selected)
	rewards_screen.tree_exited.connect(func ():
		rewards_screen = null
		rewards_screen_close.emit()
		)
	rewards_screen.reroll_pressed.connect(_on_reroll_pressed)
	
func _on_item_selected(item_offer: ItemOffer) -> void:
	rewards_screen.queue_free()
	RunContext.offers_manager.increase_relic_offer_price(item_offer)
	if item_offer.health_price > 0:
		RunContext.status.health -= item_offer.health_price
	
	var item = item_offer.create_item()
	if item is Relic:
		RunContext.relics.add_relic(item)

func _on_reroll_pressed() -> void:
	RunContext.economy.gold -= REROLL_PRICE
	var relic_offers = RunContext.offers_manager.create_relic_offers(3)
	rewards_screen.set_items_offer(relic_offers)
