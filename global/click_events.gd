#ClickEvents
extends Node

signal tower_build_button_pressed(tower_configuration: TowerConfigurationWithInstance, price: int)
signal next_wave_pressed()
signal next_level_pressed()
signal config_button_pressed()
signal speed_button_pressed()
signal reset_game_button_pressed()
signal tower_selected(tower: Tower)
signal tower_remove_pressed(tower: Tower)
signal tower_hovered(tower: Tower)
signal tower_unhovered(tower: Tower)
signal tower_upgrade_pressed(tower: Tower, tower_to_upgrade: Tower, price: int)
signal level_progess_hovered()
signal level_progess_unhovered()
