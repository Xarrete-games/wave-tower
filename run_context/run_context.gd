# Run Context
extends Node

var offers_manager: OffersManager

func reset_run() -> void:
	offers_manager = OffersManager.new()
