#ClickEvents
extends Node

signal tower_button_pressed(tower_configuration: TowerConfigurationWithInstance)
signal next_wave_pressed()
signal next_level_pressed()
signal config_button_pressed()
signal speed_button_pressed()
signal reset_game_button_pressed()
signal tower_selected(tower: Tower)
signal tower_remove_pressed(tower: Tower)
signal tower_upgrade_pressed(tower: Tower, tower_to_upgrade: Tower)
