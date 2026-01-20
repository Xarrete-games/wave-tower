class_name TowerDamageRecountRow extends Control

var tower_id: String = "":
	set(value):
		tower_id = value
		if id_label:
			id_label.text = tower_id

@onready var id_label: Label = $IdLabel

func set_damage(wave: int, damage: float) -> void:
	var label = get_children()[wave] as Label
	label.text = str(damage)
