# Run Context
extends Node

signal initialized

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")


var is_initialized: bool = false
var offers_manager: OffersManager
var progress: RunProgress
var economy: Economy
var status: Status
var is_on_restarting: bool = false

func reset_run() -> void:
	is_initialized = false
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	status = Status.new()
	status.player_died.connect(_on_die, CONNECT_ONE_SHOT)
	is_on_restarting = false
	initialized.emit()
	is_initialized = true

func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)