class_name EnemyDataUi extends Control

@onready var texture_rect: TextureRect = $TextureRect
@onready var amount_label: Label = $AmountLabel

func set_data(enemy_type_data: EnemyWaveInfo) -> void:
	texture_rect.texture = enemy_type_data.icon
	amount_label.text = str(enemy_type_data.amount)
