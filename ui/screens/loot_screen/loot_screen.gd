class_name LootScreen extends Control

@export var loot_screen_item_scene: PackedScene
@export var items_container: Control


func set_loot(data: Array[LootItemData]) -> void:
	for item_data in data:
		var loot_screen_item: LootScreenItem = loot_screen_item_scene.instantiate()
		items_container.add_child(loot_screen_item)
		loot_screen_item.set_loot_item(item_data)

func _on_xarrete_action_button_xarreta_pressed() -> void:
	queue_free()
