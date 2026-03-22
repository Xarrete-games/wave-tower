class_name ChooseRelicScreenHandler extends Node

signal rewards_screen_close()

const REWARDS_SCREEN = preload("uid://bcxsfb0ox3gmq")
const REROLL_PRICE = 20

var rewards_screen: ChooseRelicScreen

func show_choose_relic_event(event_layer: CanvasLayer) -> void:
	rewards_screen = REWARDS_SCREEN.instantiate()

	var number_of_relics = 4 if RunContext.relics_manager.has_relic("captain_cap") else 3

	var relics = DataLoader.get_random_available_relics(number_of_relics)
	event_layer.add_child(rewards_screen)
	rewards_screen.set_relics(relics)
	rewards_screen.item_selected.connect(_on_item_selected)
	
	rewards_screen.reroll_pressed.connect(_on_reroll_pressed)
	await  rewards_screen.tree_exited
	rewards_screen = null
	
func _on_item_selected(relic_data: RelicData) -> void:
	rewards_screen.queue_free()
	if relic_data.health_price > 0:
		RunContext.status.health -= relic_data.health_price
	
	var item = relic_data.create_item()
	if item is Relic:
		RunContext.relics_manager.add_relic(item)

func _on_reroll_pressed() -> void:
	RunContext.economy.gold -= REROLL_PRICE
	var number_of_relics = 4 if RunContext.relics_manager.has_relic("captain_cap") else 3
	var relics = DataLoader.get_random_available_relics(number_of_relics)
	rewards_screen.set_relics(relics)
