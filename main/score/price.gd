#price.gd
extends Node

signal tower_price_change(tower_type: Tower.TowerType, price: int)
const price_increase_percent: float = 0.20

enum TowerBuild {
	RED = 50,
	GREEN = 90,
	BLUE = 70,
}

var red_tower_price: int = TowerBuild.RED
var green_tower_price: int = TowerBuild.GREEN
var blue_tower_price: int = TowerBuild.BLUE

func tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	var base_price: int

	match tower_type:
		Tower.TowerType.RED:
			base_price = TowerBuild.RED
		Tower.TowerType.GREEN:
			base_price = TowerBuild.GREEN
		Tower.TowerType.BLUE:
			base_price = TowerBuild.BLUE		
		_:
			push_error("[Price.gd] Invalid tower type")
			return
	
	# calculate new price
	var increase_amount = float(base_price) * price_increase_percent * float(amount)
	var new_price = base_price + roundi(increase_amount)
	
	# update and emit new price
	match tower_type:
		Tower.TowerType.RED:
			red_tower_price = new_price
		Tower.TowerType.GREEN:
			green_tower_price = new_price
		Tower.TowerType.BLUE:
			blue_tower_price = new_price

	tower_price_change.emit(tower_type, new_price)

func get_price(tower_type: Tower.TowerType) -> int:
	match tower_type:
		Tower.TowerType.RED: return red_tower_price
		Tower.TowerType.GREEN: return green_tower_price
		Tower.TowerType.BLUE: return blue_tower_price
	
	push_error("[Price.gd] Invalid tower type")
	return 0
