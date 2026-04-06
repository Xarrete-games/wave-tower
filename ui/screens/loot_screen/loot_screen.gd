class_name LootScreen extends Control

@export var loot_screen_item_scene: PackedScene
@export var items_container: Control

func _ready() -> void:
	items_container.child_exiting_tree.connect(_on_item_removed)

func set_loot(data: Array[LootItemData]) -> void:
	for item_data in data:
		var loot_screen_item: LootScreenItem = loot_screen_item_scene.instantiate()
		items_container.add_child(loot_screen_item)
		loot_screen_item.set_loot_item(item_data)

func _on_xarrete_action_button_xarreta_pressed() -> void:
	queue_free()

func _on_item_removed(_item: Node) -> void:
	if items_container.get_child_count() == 1:
		queue_free()
