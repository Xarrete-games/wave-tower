class_name LootScreenItem extends Control

@export var gold_icon: Texture2D
@export var texture_rect: TextureRect
@export var label: Label

var loot_item_data: LootItemData

func set_loot_item(p_loot_item_data: LootItemData) -> void:
	loot_item_data = p_loot_item_data
	if p_loot_item_data.consumable:
		texture_rect.texture = p_loot_item_data.consumable.texture
		label.text = "%s" % p_loot_item_data.consumable.display_name
	else:
		texture_rect.texture = gold_icon
		label.text = "%d Gold" % p_loot_item_data.gold_amount


func _on_gui_input(event: InputEvent) -> void:
	if UIUtils.is_left_click_event(event):
		if loot_item_data.consumable:
			if RunContext.consumables_manager.is_full():
				# TODO error SFX
				return
			RunContext.consumables_manager.add_consumable(loot_item_data.consumable.create_item())
			
		else:
			RunContext.economy.add_gold(loot_item_data.gold_amount)
		queue_free()
		
func _on_mouse_exited() -> void:
	var stylebox: StyleBox = get_theme_stylebox("panel").duplicate()
	stylebox.bg_color = UIUtils.accent_color
	add_theme_stylebox_override("panel", stylebox)
	if loot_item_data.consumable != null and loot_item_data.consumable.description != "":
		HintManager.remove_hint(self)

func _on_mouse_entered() -> void:
	var stylebox: StyleBox = get_theme_stylebox("panel").duplicate()
	stylebox.bg_color = UIUtils.primary_color
	add_theme_stylebox_override("panel", stylebox)
	if loot_item_data.consumable != null and loot_item_data.consumable.description != "":
		HintManager.show_hint(self, loot_item_data.consumable.description, HintManager.PositionHint.RIGHT)
