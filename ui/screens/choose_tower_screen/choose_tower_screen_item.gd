class_name ChooseTowerScreenItem extends Control

signal selected(item: ChooseTowerScreenItem)

@export var tower_configuration: TowerConfigurationWithInstance
@export var title_label: Label
@export var description_label: Label
@export var texture: TextureRect
#stats
@export var damage_stat: TowerStatUi
@export var range_stat: TowerStatUi
@export var attack_speed_stat: TowerStatUi
#gold price
@export var gold_price: GoldPrice


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_apply_configuration()

func set_configuration(tower_configuration_p: TowerConfigurationWithInstance) -> void:
	self.tower_configuration = tower_configuration_p

func _apply_configuration() -> void:
	var configuration = tower_configuration.configuration
	title_label.text = configuration.display_name
	description_label.text = configuration.description
	texture.texture = configuration.icon
	damage_stat.set_value(configuration.base_damage)
	range_stat.set_value(configuration.base_attack_range)
	attack_speed_stat.set_value(configuration.base_attack_speed)
	gold_price.price = configuration.base_price


func _on_gui_input(event: InputEvent) -> void:
	if UIUtils.is_left_click_event(event):
		ChooseTowerScreen.instance.selected_tower_configuration = tower_configuration
		AudioManager.play_button_click()
		selected.emit(self)