class_name RewardsUI extends Control

signal reliq_selected(relic: Relic)

const REWARD_CARD = preload("uid://dgcv5fdqvfext")

@export var cards_container: Control
@export var reroll_priece: GoldPrice

var _enabled = false
var _reroll_priece: int = 0

func _ready() -> void:
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
	_reroll_priece = RewardsManager.reroll_price
	reroll_priece.price = _reroll_priece

func _on_card_pressed(relic: Relic) -> void:
	if not _enabled:
		return
	
	reliq_selected.emit(relic)
	
func _on_exit_button_pressed() -> void:
	queue_free()

func _on_reroll_button_pressed() -> void:
	if _reroll_priece <= Score.gold:
		RewardsManager.reroll()
