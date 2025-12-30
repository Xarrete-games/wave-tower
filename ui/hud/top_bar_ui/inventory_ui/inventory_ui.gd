class_name InventoryUI extends Control

@export var slots_container: Control

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	RunContext.consumables.consumable_added.connect(_on_consumable_added)

func _on_consumable_added(consumable: Consumable) -> void:
	for slot: InventoryUISlot in slots_container.get_children():
		if slot.is_empty():
			slot.set_consumable(consumable)
			return
