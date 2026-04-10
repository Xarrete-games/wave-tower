class_name ShopScreen extends Control

signal item_purchase(item: ItemOffer)

const SHOP_SLOT = preload("uid://f428sxnliflm")

# containers
@export var relics_container: Control
@export var consumables_container: Control
@export var sell_relics_container: Control
# sections
@export var relics_section: Control
@export var consumables_section: Control
@export var sell_section: Control
# buttons
@export var exit_button: XarretaButton
@export var sell_button: XarretaButton

var is_on_sell_mode: bool = false

func _ready() -> void:
	_change_to_buy_mode()
	if not RunContext.economy.is_sell_active:
		sell_button.visible = false
	_build_relics_for_sale()

func set_relics(relics: Array) -> void:
	for relic in relics:
		var slot: ShopSlot = SHOP_SLOT.instantiate()
		relics_container.add_child(slot)
		slot.set_item(relic)
		slot.item_purchased.connect(_on_item_purchase)

func set_consumables(consumables: Array) -> void:
	for consumable in consumables:
		var slot: ShopSlot = SHOP_SLOT.instantiate()
		consumables_container.add_child(slot)
		slot.set_item(consumable)
		slot.item_purchased.connect(_on_item_purchase)

func _on_item_purchase(relic: ItemOffer, slot_purchased: ShopSlot) -> void:
	item_purchase.emit(relic)
	AudioManager.play_purchase()
	for slot: ShopSlot in relics_container.get_children():
		if slot == slot_purchased:
			if relic.item_data.id == "strategy_tome_economy":
				sell_button.visible = true
			slot.queue_free()
			return
	for slot: ShopSlot in consumables_container.get_children():
		if slot == slot_purchased:
			slot.queue_free()
			return

func _on_item_sold(item_offer: ItemOffer, slot_sold: ShopSlot) -> void:
	for slot: ShopSlot in sell_relics_container.get_children():
		if slot == slot_sold:
			slot.queue_free()
			RunContext.relics_manager.remove_relic(item_offer.item_data.id)
			RunContext.economy.add_gold(item_offer.price)
			AudioManager.play_purchase()
			sell_button.disable()
			_on_exit_button_pressed()
			return

func _on_exit_button_pressed() -> void:
	if is_on_sell_mode:
		_change_to_buy_mode()
	else:
		queue_free()

func _on_sell_button_pressed() -> void:
	_change_to_sell_mode()

func _change_to_sell_mode() -> void:
	is_on_sell_mode = true
	sell_button.visible = false
	exit_button.visible = true
	relics_section.visible = false
	consumables_section.visible = false
	sell_section.visible = true

func _change_to_buy_mode() -> void:
	sell_button.visible = true
	is_on_sell_mode = false
	relics_section.visible = true
	consumables_section.visible = true
	sell_section.visible = false

func _build_relics_for_sale() -> void:
	var current_relics: Array = RunContext.relics_manager.get_all_relics()
	var current_relics_data: Array = []
	for relic_data in current_relics:
		current_relics_data.append(relic_data.data)
	var relic_offers: Array = RunContext.offers_manager.create_relic_offers_from_data(current_relics_data)
	
	for relic_offer in relic_offers:
		var slot: ShopSlot = SHOP_SLOT.instantiate()
		sell_relics_container.add_child(slot)
		slot.set_item(relic_offer)
		slot.item_purchased.connect(_on_item_sold)
