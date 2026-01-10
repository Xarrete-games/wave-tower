# Run Context
extends Node

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")

# flags
var is_on_restarting: bool = false

# subsystems
var offers_manager: OffersManager
var progress: RunProgress
var economy: Economy
var status: Status
var towers_manager: TowersManager
var towers_buffs: TowersBuffs
var buff_scheduler: BuffScheduler
var towers_price: TowersPrice
var relics_manager: RelicsManager
var consumables_manager: ConsumablesManager
var enemy_debuff_manager: EnemyDebuffManager
var enemy_manager: EnemyManager

func reset_run() -> void:
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	status = Status.new()
	status.player_died.connect(_on_die, CONNECT_ONE_SHOT)
	towers_manager = TowersManager.new()
	towers_buffs = TowersBuffs.new()
	buff_scheduler = BuffScheduler.new()
	towers_price = TowersPrice.new()
	relics_manager = RelicsManager.new()
	consumables_manager = ConsumablesManager.new()
	enemy_debuff_manager = EnemyDebuffManager.new()
	enemy_manager = EnemyManager.new()
	is_on_restarting = false

func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)