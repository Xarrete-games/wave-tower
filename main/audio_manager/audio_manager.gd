extends Node

@onready var button_click: AudioStreamPlayer = $ButtonClick
@onready var button_hover: AudioStreamPlayer = $ButtonHover
@onready var coins: AudioStreamPlayer = $Coins
@onready var main_piano_player: AudioStreamPlayer = $MainPianoPlayer
@onready var purchase_player: AudioStreamPlayer = $PurchasePlayer
@onready var wave_clear: AudioStreamPlayer = $WaveClear
@onready var defeated_sound: AudioStreamPlayer = $DefeatedSound
@onready var relic_obtain: AudioStreamPlayer = $Relic_obtain
@onready var tower_obtain: AudioStreamPlayer = $Tower_obtain
@onready var place_tower: AudioStreamPlayer = $Place_tower
@onready var armor: AudioStreamPlayer = $Armor
@onready var loss_hp: AudioStreamPlayer = $Loss_HP
@onready var loss_armor: AudioStreamPlayer = $Loss_Armor

func play_coins():
	coins.play()

func play_relic_obtain():
	relic_obtain.play()

func play_tower_obtain():
	tower_obtain.play()

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
	wave_clear.play()

func play_defeated_sound():
	defeated_sound.play()

func play_place_tower():
	place_tower.play()

func play_player_hurt():
	loss_hp.play()

func play_armor_block():
	loss_armor.play()

func play_gain_armor():
	armor.play()
