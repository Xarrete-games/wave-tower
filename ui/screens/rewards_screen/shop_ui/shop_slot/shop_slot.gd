class_name ShopSlot extends Control

signal item_purchased(item_offer: ItemOffer, slot: ShopSlot)

@export var title_label: Label
@export var description_label: RichTextLabel
@export var gold_price: GoldPrice
@export var shop_slot_icon: ShopSlotIcon

var _item: ItemOffer

func set_relic(item_offer: ItemOffer) -> void:
	title_label.text = item_offer.item_data.id
	description_label.text = item_offer.item_data.description
	gold_price.price = item_offer.item_data.price
	shop_slot_icon.set_icon(item_offer.item_data.texture)
	shop_slot_icon.set_background_color(RelicsManager.get_rarity_color(item_offer.item_data.rarity))
	_item = item_offer
	
func _on_gui_input(event: InputEvent) -> void:
	if Utils.is_left_click_event(event) and Score.gold >= _item.item_data.price:
		AudioManager.play_button_click()
		item_purchased.emit(_item, self)

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	shop_slot_icon.increased_icon_size()
	#texture_rect.custom_minimum_size = Vector2(120, 120)

func _on_mouse_exited() -> void:
	shop_slot_icon.icon_normal_size()
