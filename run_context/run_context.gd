# Run Context
extends Node

signal run_reset

var offers_manager: OffersManager
var progress: RunProgress
var economy: Economy
var is_on_restarting: bool = false

func reset_run() -> void:
	offers_manager = OffersManager.new()
	progress = RunProgress.new()
	economy = Economy.new()
	is_on_restarting = false
	run_reset.emit()
