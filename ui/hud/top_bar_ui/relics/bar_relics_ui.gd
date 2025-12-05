class_name RelicsBar extends Control

const TOP_BAR_RELIC = preload("uid://f34dinc60kaa")

func _ready():
	RelicsManager.relics_change.connect(_update_relics)

func _update_relics(relics: Array):
	clear_relics_container()
	for relic in relics:
		var relic_instance: RelicUI = TOP_BAR_RELIC.instantiate()
		add_child(relic_instance)
		relic_instance.set_relic(relic)
		#relic_instance.get_texture().texture = relic.get_texture()
		#if relic.amount > 1:
			#relic_instance.get_amount().text = str(relic.amount)

func clear_relics_container() -> void:
	for relic in get_children():
		relic.queue_free()
