class_name ChooseRelicScreen extends Control

signal item_selected(item: RelicData)
signal reroll_pressed()

const CHOOSE_RELIC_CARD = preload("uid://dgcv5fdqvfext")

@export var cards_container: Control
@export var reroll_priece: GoldPrice

var _enabled = false
var _reroll_priece: int = 20

func _ready() -> void:
	await get_tree().create_timer(0.3).timeout
	_enabled = true

func set_relics(relics: Array[RelicData]) -> void:
	for child in cards_container.get_children():
		child.queue_free()
	
	for relic_data in relics:
		var card: ChooseRelicCard = CHOOSE_RELIC_CARD.instantiate()
		cards_container.add_child(card)
		card.set_relic(relic_data)
		card.card_pressed.connect(_on_card_pressed)
	reroll_priece.price = _reroll_priece

func _on_card_pressed(relic_data: RelicData) -> void:
	if not _enabled:
		return
	
	item_selected.emit(relic_data)
	
func _on_reroll_button_xarreta_pressed() -> void:
	if _reroll_priece <= RunContext.economy.gold:
		reroll_pressed.emit()

func _on_exit_button_xarreta_pressed() -> void:
	queue_free()
