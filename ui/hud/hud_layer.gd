class_name HUDLayer extends CanvasLayer

signal tower_button_pressed(tower_scene: PackedScene)
signal config_pressed()

func _on_towers_menu_tower_button_pressed(tower_scene: PackedScene) -> void:
	tower_button_pressed.emit(tower_scene)

func _on_top_bar_config_pressed() -> void:
	config_pressed.emit()
