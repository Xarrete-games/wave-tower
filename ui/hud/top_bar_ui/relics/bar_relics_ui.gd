class_name RelicsBar extends Control

const TOP_BAR_RELIC = preload("uid://f34dinc60kaa")

func _ready():
	RunContext.relics_manager.relics_change.connect(_update_relics)

func _update_relics(relics: Array):
	clear_relics_container()
	for relic in relics:
		var relic_instance: RelicUI = TOP_BAR_RELIC.instantiate()
		add_child(relic_instance)
		relic_instance.set_relic(relic)

func clear_relics_container() -> void:
	for relic in get_children():
		relic.queue_free()
