class_name RewardsUI extends Control

signal reliq_selected(relic: Relic)

const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")
const REWARD_CARD = preload("uid://dgcv5fdqvfext")

@export var cards_container: Control
@export var reroll_price_label: Label

var _enabled = false
var _reroll_priece = 0
var _has_enough_gold_for_reroll = false

func _ready() -> void:
	Score.gold_change.connect(_check_price)
	await get_tree().create_timer(0.3).timeout
	_enabled = true

func set_relics(relics: Array[Relic]) -> void:
	for child in cards_container.get_children():
		child.queue_free()
	
	for relic in relics:
		var card: Rewardard = REWARD_CARD.instantiate()
		cards_container.add_child(card)
		card.set_relic(relic)
		card.card_pressed.connect(_on_card_pressed)
	_set_reroll_price(RewardsManager.reroll_price)

func _on_card_pressed(relic: Relic) -> void:
	if not _enabled:
		return
	
	reliq_selected.emit(relic)
	
func _set_reroll_price(price: int) -> void:
	_reroll_priece = price
	reroll_price_label.text = str(price)
	_check_price(Score.gold)

func _check_price(current_gold: int) -> void:
	_has_enough_gold_for_reroll = current_gold >= _reroll_priece
	reroll_price_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_gold_for_reroll
		else LABEL_SETTINGS_24_INVALID
	)
	
func _on_exit_button_pressed() -> void:
	queue_free()

func _on_reroll_button_pressed() -> void:
	if _has_enough_gold_for_reroll:
		RewardsManager.reroll()
