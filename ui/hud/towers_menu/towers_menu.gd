class_name TowersMenu extends Control

@export var buttons_container: Control
@export var tower_hint: TowerButtonHint
@export var tower_button_scene: PackedScene

var button_in_hover: TowerButton = null
# dictionary of tower id/amount
var buttons: Dictionary[String, TowerButton] = {}

func _ready() -> void:
	tower_hint.visible = false
	RunContext.towers_manager.tower_card_amount_change.connect(_on_tower_card_added)
	_init_button_cards()
	

func _init_button_cards() -> void:
	var towers_cards_amount = RunContext.towers_manager.tower_cards_amount
	for tower_configuration_id in towers_cards_amount.keys():
		var amount = towers_cards_amount[tower_configuration_id]
		var tower_configuration = RunContext.towers_manager.get_tower_configuration_by_id(tower_configuration_id)
		_on_tower_card_added(tower_configuration, amount)

func _on_tower_card_added(tower_data, amount: int) -> void:
	if amount == 0:
		if tower_data.data.id in buttons:
			buttons[tower_data.data.id].queue_free()
			buttons.erase(tower_data.data.id)
			return

	if tower_data.data.id not in buttons:
		var new_button = tower_button_scene.instantiate() as TowerButton
		buttons_container.add_child(new_button)
		new_button.tower_data = tower_data
		buttons[tower_data.data.id] = new_button
		new_button.amount = amount
		# connect signals
		new_button.tower_button_pressed.connect(_on_tower_button_pressed)
		new_button.hover.connect(_on_tower_button_hover)
		new_button.unhover.connect(_on_tower_button_unhover)
	else:
		if tower_data.data.id in buttons:
			buttons[tower_data.data.id].amount = amount
	 
func _on_tower_button_pressed(tower_data, price: int) -> void:
	print("[TowersMenu] forwarding build event price=", price)
	ClickEvents.emit_tower_build_button_pressed(tower_data, price)

func _on_tower_button_hover(tower_button: TowerButton) -> void:
	button_in_hover = tower_button
	tower_hint.set_stats(button_in_hover.tower_data.data)
	# Position the hint above the button
	var rect: Rect2 = tower_button.get_global_rect()	
	tower_hint.global_position = Vector2(
		rect.position.x + rect.size.x * 0.25 - tower_hint.size.x * 0.5,
		tower_hint.global_position.y
	)
	tower_hint.visible = true

func _on_tower_button_unhover(tower_button: TowerButton) -> void:
	if button_in_hover == tower_button:
		button_in_hover = null
		tower_hint.visible = false
