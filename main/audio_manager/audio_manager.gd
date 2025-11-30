extends Node2D

signal tempo
@onready var button_click: AudioStreamPlayer = $ButtonClick
@onready var button_hover: AudioStreamPlayer = $ButtonHover
@onready var coins: AudioStreamPlayer = $Coins

func play_coins():
	coins.play()
	
func play_button_hover():
	button_hover.play()
	
func play_button_click():
	button_click.play()

func _on_tempo_timeout() -> void:
	tempo.emit()
