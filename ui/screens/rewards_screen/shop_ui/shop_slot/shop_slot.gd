class_name ShopSlot extends Control

signal item_purchased(relic: RedRelic, slot: ShopSlot)

@export var title_label: Label
@export var description_label: RichTextLabel
@export var gold_price: GoldPrice
@export var shop_slot_icon: ShopSlotIcon

var _relic: Relic

func set_relic(relic: Relic) -> void:
	title_label.text = relic.id
	description_label.text = relic.description
	gold_price.price = relic.price
	shop_slot_icon.set_icon(relic.texture)
	shop_slot_icon.set_background_color(RelicsManager.get_rarity_color(relic.rarity))
	_relic = relic
	
func _on_gui_input(event: InputEvent) -> void:
	if Utils.is_left_click_event(event) and Score.gold >= _relic.price:
		AudioManager.play_button_click()
		item_purchased.emit(_relic, self)

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	shop_slot_icon.increased_icon_size()
	#texture_rect.custom_minimum_size = Vector2(120, 120)

func _on_mouse_exited() -> void:
	shop_slot_icon.icon_normal_size()
