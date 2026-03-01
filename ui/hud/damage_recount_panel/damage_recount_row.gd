class_name TowerDamageRecountRow extends Control

var damage_per_wave: Array[float] = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]
var tower_id: String = "":
	set(value):
		tower_id = value
		if id_label:
			id_label.text = tower_id

var total_damage: float = 0.0

@onready var id_label: Label = $IdLabel

func set_damage(_wave: int, _damage: float) -> void:
	return
	# var label = get_children()[wave] as Label
	# label.text = str(int(damage))
	# update_total_damage(wave, damage)
	
func update_total_damage(_wave: int, _damage: float) -> void:
	return
	# total_damage -= damage_per_wave[wave - 1]
	# damage_per_wave[wave - 1] = damage
	# total_damage += damage
	# var total_label = get_children()[11] as Label
	# total_label.text = str(int(total_damage))
