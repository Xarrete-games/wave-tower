class_name RelicsBar extends Control

const TOP_BAR_RELIC = preload("uid://f34dinc60kaa")

func _ready():
	RunContext.relics_manager.relic_added.connect(_on_relic_added)
	RunContext.relics_manager.relic_removed.connect(_on_relic_removed)

func _on_relic_added(relic: Relic) -> void:
	var relic_instance: RelicUI = TOP_BAR_RELIC.instantiate()
	add_child(relic_instance)
	relic_instance.set_relic(relic)

func _on_relic_removed(relic_id: String) -> void:
	for relic_ui in get_children():
		if relic_ui.relic.data.id == relic_id:
			relic_ui.queue_free()
