class_name ShopSlot extends Control

signal item_purchased(item_offer: ItemOffer, slot: ShopSlot)

@export var title_label: Label
@export var description_label: RichTextLabel
@export var gold_price: GoldPrice
@export var shop_slot_icon: ShopSlotIcon
@onready var health_price: HealthPrice = $HealthPrice

var _item: ItemOffer
var _price: int = 0
var has_enough_health: bool = false
var item_data

func _ready() -> void:
	health_price.visible = false
	RunContext.relics_manager.relic_added.connect(_on_relic_added)

func set_item(item_offer: ItemOffer) -> void:
	_price = item_offer.price
	item_data = item_offer.item_data
	title_label.text = item_data.display_name
	description_label.text = item_data.description
	tooltip_text = item_data.description
	gold_price.price = _price
	shop_slot_icon.set_icon(item_data.icon)

	if item_data.get("is_tome") != null or item_data.get("is_cursed") != null:
		var relic_data = item_data
		shop_slot_icon.set_background_color(RunContext.relics_manager.get_rarity_color(relic_data.rarity))
	# health price
	_chek_health(RunContext.status.health, item_offer.health_price)
	if item_offer.health_price > 0:
		health_price.visible = true
		health_price.price = item_offer.health_price
	
	RunContext.status.health_change.connect(func (current_health: int) -> void:
		_chek_health(current_health, item_offer.health_price))
	
	_item = item_offer
	
func _on_gui_input(event: InputEvent) -> void:
	if _item.item_data.get("consumable_type") != null and RunContext.consumables_manager.is_full():
		return

	if UIUtils.is_left_click_event(event) and RunContext.economy.gold >= _price and has_enough_health:
		AudioManager.play_button_click()
		item_purchased.emit(_item, self)

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	shop_slot_icon.increased_icon_size()

func _on_mouse_exited() -> void:
	shop_slot_icon.icon_normal_size()

func _on_relic_added(relic) -> void:
	var id = relic.data.id
	if id == "salmon_nigiri" or id == "butterfish_nigiri" or id == "soya_sauce" or id == "tuna_nigiri":
		if _item.item_data.get("is_tome") != null or _item.item_data.get("is_cursed") != null:
			set_item(RunContext.offers_manager.create_relic_offer_from_data(_item.item_data))
		elif _item.item_data.get("consumable_type") != null:
			set_item(RunContext.offers_manager.create_consumable_offer_from_data(_item.item_data))

func _chek_health(current_health: int, health_cost: int) -> void:
	has_enough_health = current_health > health_cost
