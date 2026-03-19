extends Node2D

@onready var button_click: AudioStreamPlayer = $ButtonClick
@onready var button_hover: AudioStreamPlayer = $ButtonHover
@onready var coins: AudioStreamPlayer = $Coins
@onready var main_piano_player: AudioStreamPlayer = $MainPianoPlayer
@onready var purchase_player: AudioStreamPlayer = $PurchasePlayer
@onready var music_player: AudioStreamPlayer = $WaveClear

func play_coins():
	coins.play()
	
func play_button_hover():
	button_hover.play()
	
func play_button_click():
	button_click.play()

func play_main_piano():
	if main_piano_player.playing:
		return
	else:
		main_piano_player.play()

func play_purchase():
	purchase_player.play()

func stop_main_piano():
	main_piano_player.stop()

func play_wave_clear():
	music_player.play()
