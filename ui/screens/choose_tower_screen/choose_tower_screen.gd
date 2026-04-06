class_name ChooseTowerScreen extends Control

const CHOOSE_TOWER_SCREEN_SCENE: PackedScene = preload("uid://bxw2isn60dlre")
const TOWER_ITEM_SCENE: PackedScene = preload("uid://cw14sfvcipnum")

signal done()

@export var cards_container: Control
@export var ok_button: Button

static var instance: ChooseTowerScreen = null

var selected_tower_configuration: TowerDataWithInstance = null
var selected_item: ChooseTowerScreenItem = null


static func show_screen(configurations: Array[TowerDataWithInstance], canvas: CanvasLayer) -> void:
	instance = CHOOSE_TOWER_SCREEN_SCENE.instantiate() as ChooseTowerScreen
	canvas.add_child(instance)
	instance.populate_screen(configurations)
	await instance.done

func _ready() -> void:
	ok_button.pressed.connect(_on_ok_pressed)

func populate_screen(configurations: Array[TowerDataWithInstance]) -> void:
	for tower_configuration in configurations:
		var item = TOWER_ITEM_SCENE.instantiate() as ChooseTowerScreenItem
		item.tower_data = tower_configuration
		cards_container.add_child(item)
		item.selected.connect(_on_item_selected)

func _on_item_selected(item: ChooseTowerScreenItem) -> void:
	selected_item = item
	#instant select and confirm
	AudioManager.play_tower_obtain()
	_on_ok_pressed()

func _on_ok_pressed() -> void:
	if not selected_item:
		return

	ClickEvents.add_tower_card.emit(selected_item.tower_data)
	done.emit()
	queue_free()
