class_name MusicHandler extends Node

const MAX_PLAYERS = 8

@onready var red_players: Node = $RedPlayers
@onready var blue_players: Node = $BluePlayers
@onready var green_players: Node = $GreenPlayers
@onready var base_player: AudioStreamPlayer = $BasePlayer

var tower_players: Dictionary

func _ready() -> void:
	tower_players = {
		Tower.TowerType.RED: red_players,
		Tower.TowerType.BLUE: blue_players,
		Tower.TowerType.GREEN: green_players
	}
	stop_music()
	TowerPlacementManager.tower_count_change.connect(_on_tower_count_change)

func play_music() -> void:
	base_player.play()
	_start_players(red_players)
	_start_players(blue_players)
	_start_players(green_players)
	
func stop_music() -> void:
	_stop_players(red_players)
	_stop_players(blue_players)
	_stop_players(green_players)
	base_player.stop()
	
func _start_players(node: Node) -> void:
	for player: AudioStreamPlayer in node.get_children():
		player.play()

func _stop_players(node: Node) -> void:
	for player: AudioStreamPlayer in node.get_children():
		_stop_player(player)
		
func _on_tower_count_change(
	tower_type: Tower.TowerType, 
	amount: int, 
	_event: TowerPlacementManager.TowerEvent) -> void:
	if amount > MAX_PLAYERS or amount == 0:
		return

	var player_list: Array[Node] = (tower_players[tower_type] as Node).get_children()
	var player: AudioStreamPlayer = player_list.get(amount - 1)
	_play_player(player)

func _play_player(player: AudioStreamPlayer) -> void:
	var tween = create_tween()
	tween.tween_property(player, "volume_db", 0, 1)

func _stop_player(player: AudioStreamPlayer) -> void:
	player.volume_db = -80
