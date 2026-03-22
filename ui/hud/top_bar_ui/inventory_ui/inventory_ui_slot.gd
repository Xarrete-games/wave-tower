class_name InventoryUISlot extends Control

@onready var texture_rect: TextureRect = $CenterContainer/TextureRect


var _consumable: Consumable = null

func _ready() -> void:
	RunContext.consumables_manager.consumable_used.connect(_on_consumable_used)

func is_empty() -> bool:
	return _consumable == null

func set_consumable(consumable: Consumable) -> void:
	_consumable = consumable
	texture_rect.texture = consumable.data.icon

func _on_gui_input(event: InputEvent) -> void:
	if _consumable == null:
		return

	if UIUtils.is_left_click_event(event):
		AudioManager.play_button_click()
		HintManager.remove_hint(self)
		_consumable.clicked.emit(_consumable)

func _on_consumable_used(consumable: Consumable) -> void:
	if _consumable == consumable:
		_consumable = null
		texture_rect.texture = null

func _on_mouse_entered() -> void:
	if _consumable == null:
		return
	AudioManager.play_button_hover()
	#texture_rect.custom_minimum_size = Vector2(50, 50)
	if _consumable.data.description != "":
		HintManager.show_hint(self, _consumable.data.description)

func _on_mouse_exited() -> void:
	#texture_rect.custom_minimum_size = Vector2(40, 40)
	HintManager.remove_hint(self)
