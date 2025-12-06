class_name ShopSlot extends Control

signal item_purchased(relic: RedRelic, slot: ShopSlot)

@export var texture_rect: TextureRect
@export var title_label: Label
@export var description_label: RichTextLabel
@export var gold_price: GoldPrice
@export var background: Polygon2D

var _relic: Relic

func set_relic(relic: Relic) -> void:
	texture_rect.texture = relic.texture
	title_label.text = relic.id
	description_label.text = relic.description
	gold_price.price = relic.price
	background.color = RelicsManager.get_rarity_color(relic.rarity)
	_relic = relic
	
func _on_gui_input(event: InputEvent) -> void:
	if Utils.is_left_click_event(event) and Score.gold >= _relic.price:
		AudioManager.play_button_click()
		item_purchased.emit(_relic, self)

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	texture_rect.custom_minimum_size = Vector2(120, 120)

func _on_mouse_exited() -> void:
	texture_rect.custom_minimum_size = Vector2(100, 100)
