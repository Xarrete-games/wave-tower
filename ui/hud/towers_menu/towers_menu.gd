class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

@export var buttons_container: Control
@export var tower_hint: TowerButtonHint

var button_in_hover: TowerButton = null

func _ready() -> void:
	tower_hint.visible = false
	await RunContext.initialized
	for button in buttons_container.get_children():
		button.tower_button_pressed.connect(_on_tower_button_pressed)
		button.hover.connect(_on_tower_button_hover)
		button.unhover.connect(_on_tower_button_unhover)

func _on_tower_button_pressed(tower_scene: PackedScene) -> void:
	ClickEvents.tower_button_pressed.emit(tower_scene)

func _on_tower_button_hover(tower_button: TowerButton) -> void:
	button_in_hover = tower_button
	tower_hint.set_stats(button_in_hover.base_stats)
	# Position the hint above the button
	var rect: Rect2 = tower_button.get_global_rect()	
	tower_hint.global_position = Vector2(
		rect.position.x + rect.size.x * 0.5 - tower_hint.size.x * 0.5,
		rect.position.y - tower_hint.size.y - 30
	)
	tower_hint.visible = true

func _on_tower_button_unhover(tower_button: TowerButton) -> void:
	if button_in_hover == tower_button:
		button_in_hover = null
		tower_hint.visible = false
