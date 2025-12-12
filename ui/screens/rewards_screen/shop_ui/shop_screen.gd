class_name ShopScreen extends Control

signal item_purchase(relic: Relic)

const SHOP_SLOT = preload("uid://f428sxnliflm")

@export var relics_container: Control
@export var items_container: Control

func set_relics(relics: Array[ItemOffer]) -> void:
	for relic in relics:
		var slot: ShopSlot = SHOP_SLOT.instantiate()
		relics_container.add_child(slot)
		slot.set_relic(relic)
		slot.item_purchased.connect(_on_item_purchase)

func _on_item_purchase(relic: Relic, slot_purchased: ShopSlot) -> void:
	item_purchase.emit(relic)
	AudioManager.play_purchase()
	for slot: ShopSlot in relics_container.get_children():
		if slot == slot_purchased:
			slot.queue_free()
			return

func _on_xarrete_action_button_xarreta_pressed() -> void:
	queue_free()
