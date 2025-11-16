extends Node

func on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	match tower_type:
		Tower.TowerType.RED: _hand_red_updates
		Tower.TowerType.GREEN: _hand_green_updates
		Tower.TowerType.BLUE: _hand_blue_updates
		
func _hand_red_updates(amount: int) -> void:
	pass
	
func _hand_green_updates(amount: int) -> void:
	pass
	
func _hand_blue_updates(amount: int) -> void:
	pass
