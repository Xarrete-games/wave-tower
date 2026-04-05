# Run Context
extends Node

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")

# flags
var is_on_restarting: bool = false
# level info
var composite_tile_map: CompositeTileMap

# subsystems
var offers_manager: OffersManager
var progress: RunProgress
var economy: Economy
var status: Status
var towers_manager: TowersManager
var relics_manager: RelicsManager
var consumables_manager: ConsumablesManager
var enemy_manager: EnemyManager

func _ready() -> void:
	reset_run()

func reset_run() -> void:
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	relics_manager = RelicsManager.new()
	status = Status.new(progress, relics_manager)
	towers_manager = TowersManager.new(progress)
	consumables_manager = ConsumablesManager.new()
	enemy_manager = EnemyManager.new()
	is_on_restarting = false

	status.player_died.connect(_on_die, CONNECT_ONE_SHOT)

func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)
