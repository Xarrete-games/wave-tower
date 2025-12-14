# Run Context
extends Node

signal initialized

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")

# flags
var is_initialized: bool = false
var is_on_restarting: bool = false

# subsystems
var offers_manager: OffersManager
var progress: RunProgress
var economy: Economy
var status: Status
var towers_count: TowersCount
var towers_upgrades: TowersUpgrades
var towers_price: TowersPrice
var relics: RelicsManager
var enemy_debuff: EnemyDebuffManager
var enemy_manager: EnemyManager

func reset_run() -> void:
	is_initialized = false
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	status = Status.new()
	status.player_died.connect(_on_die, CONNECT_ONE_SHOT)
	towers_count = TowersCount.new()
	towers_upgrades = TowersUpgrades.new()
	towers_price = TowersPrice.new()
	relics = RelicsManager.new()
	enemy_debuff = EnemyDebuffManager.new()
	enemy_manager = EnemyManager.new()
	is_on_restarting = false
	initialized.emit()
	is_initialized = true

func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)