class_name LootScreenHandler extends Node

@export var loot_screen_scene: PackedScene

func show_loot_screen(event_layer: CanvasLayer) -> void:
	var loot_screen: LootScreen = loot_screen_scene.instantiate()
	event_layer.add_child(loot_screen)
	
	var loot_items = RunContext.loot_manager.generate_loot_items()
	loot_screen.set_loot(loot_items)
	
	await  loot_screen.tree_exited
