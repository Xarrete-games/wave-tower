# Run Context
extends Node

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")

# flags
var is_on_restarting: bool = false
# level info
var composite_tile_map: CompositeTileMap

# subsystems
var offers_manager
var progress
var economy
var status
var towers_manager: TowersManager
var relics_manager
var consumables_manager
var enemy_manager

func _ready() -> void:
	reset_run()

func reset_run() -> void:
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	relics_manager = RelicsManager.new()
	status = Status.new()
	status.setup(progress, relics_manager)
	towers_manager = TowersManager.new()
	towers_manager.setup(progress)
	consumables_manager = ConsumablesManager.new()
	enemy_manager = EnemyManager.new()
	is_on_restarting = false

	status.player_died.connect(_on_die, CONNECT_ONE_SHOT)

func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)
