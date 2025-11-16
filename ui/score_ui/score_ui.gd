class_name ScoreUi extends Control

@onready var amount_gold_label: Label = $MarginContainer/HBox/HBoxContainer/AmountGoldLabel
@onready var relics_container: HBoxContainer = $MarginContainer/HBox/HBoxContainer/RelicsContainer


func _ready():
	_on_gold_change(Score.gold)
	Score.gold_change.connect(_on_gold_change)
	RelicsManager.relics_change.connect(_update_relics)
	
func _on_gold_change(amount: int) -> void:
	amount_gold_label.text = str(amount)

func _update_relics(relics: Array):
	clear_relics_container()
	for relic in relics:
		var relic_texture = TextureRect.new()
		relic_texture.texture = relic.get_texture()
		relics_container.add_child(relic_texture)

func clear_relics_container() -> void:
	for relic in relics_container.get_children():
		relic.queue_free()
