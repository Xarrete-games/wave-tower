class_name MusicHandler extends Node

const MAX_PLAYERS = 8

@export var tower_placer: TowerPlacer

@onready var red_players: Node = $RedPlayers
@onready var blue_players: Node = $BluePlayers
@onready var green_players: Node = $GreenPlayers
@onready var base_player: AudioStreamPlayer = $BasePlayer

var towers_placed = {
	Tower.TowerType.RED: 0,
	Tower.TowerType.BLUE: 0,
	Tower.TowerType.GREEN: 0
}

var tower_players: Dictionary

func _ready() -> void:
	tower_players = {
		Tower.TowerType.RED: red_players,
		Tower.TowerType.BLUE: blue_players,
		Tower.TowerType.GREEN: green_players
	}
	stop_music()
	tower_placer.tower_placed.connect(_on_tower_placed)

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
		
func _on_tower_placed(tower_type: Tower.TowerType) -> void:
	if towers_placed[tower_type] == MAX_PLAYERS:
		return

	var player_list: Array[Node] = (tower_players[tower_type] as Node).get_children()
	var player: AudioStreamPlayer = player_list.get(towers_placed[tower_type])
	towers_placed[tower_type] += 1
	_play_player(player)

func _play_player(player: AudioStreamPlayer) -> void:
	var tween = create_tween()
	tween.tween_property(player, "volume_db", 0, 1)

func _stop_player(player: AudioStreamPlayer) -> void:
	player.volume_db = -80
