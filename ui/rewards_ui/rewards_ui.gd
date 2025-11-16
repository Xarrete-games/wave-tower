class_name RewardsUI extends Control

signal reliq_selected(relic: Relic)

const REWARD_CARD = preload("uid://dgcv5fdqvfext")

var _enabled = false

@export var cards_container: Control

func _ready() -> void:
	await get_tree().create_timer(0.1).timeout
	_enabled = true

func set_relics(relics: Array[Relic]) -> void:
	for relic in relics:
		var card: Rewardard = REWARD_CARD.instantiate()
		cards_container.add_child(card)
		card.set_relic(relic)
		card.card_pressed.connect(_on_card_pressed)

func _on_card_pressed(relic: Relic) -> void:
	if not _enabled:
		return
	
	reliq_selected.emit(relic)
