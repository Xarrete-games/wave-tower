#price.gd
extends Node

signal tower_price_change(tower_type: Tower.TowerType, price: int)
const price_increase_percent: float = 0.20

enum TowerBuild {
	RED = 80,
	GREEN = 90,
	BLUE = 100,
}

var base_prices: Dictionary[Tower.TowerType, int] = {
	Tower.TowerType.RED: TowerBuild.RED,
	Tower.TowerType.GREEN: TowerBuild.GREEN,
	Tower.TowerType.BLUE: TowerBuild.BLUE,
}

var build_prices: Dictionary[Tower.TowerType, int] = {
	Tower.TowerType.RED: TowerBuild.RED,
	Tower.TowerType.GREEN: TowerBuild.GREEN,
	Tower.TowerType.BLUE: TowerBuild.BLUE,
}

var sell_prices: Dictionary[Tower.TowerType, int] = {
	Tower.TowerType.RED: TowerBuild.RED,
	Tower.TowerType.GREEN: TowerBuild.GREEN,
	Tower.TowerType.BLUE: TowerBuild.BLUE,
}


# free towers
var _free_towers_available = 0

func _ready() -> void:
	TowerPlacementManager.tower_count_change.connect(_on_tower_count_change)

func add_free_tower(amount: int = 1) -> void:
	_free_towers_available += amount
	_emit_all_towers_change()

func get_price(tower_type: Tower.TowerType) -> int:
	if _next_tower_is_free():
		return 0	
	return build_prices[tower_type]

func get_sell_price(tower_type: Tower.TowerType) -> int:
	return sell_prices[tower_type]

func get_base_price(tower_type: Tower.TowerType) -> int:
	return base_prices[tower_type]

func get_next_price(base_price: int, amount: int) -> int:
	var increase_amount = float(base_price) * price_increase_percent * float(amount)
	return base_price + roundi(increase_amount)

func _on_tower_count_change(
	tower_type: Tower.TowerType, 
	amount: int, 
	event: TowerPlacementManager.TowerEvent) -> void:
		if event == TowerPlacementManager.TowerEvent.SOLD:
			_on_tower_sold(tower_type, amount)
		else:
			_on_tower_placed(tower_type, amount)

func _on_tower_sold(tower_type: Tower.TowerType, amount: int) -> void:
	var base_price = get_base_price(tower_type)
	var new_price = get_next_price(base_price, amount)
	
	build_prices[tower_type] = new_price
	if amount >= 1:
		var new_sell_price = get_next_price(base_price, amount - 1)
		sell_prices[tower_type] = new_sell_price
	else:
		sell_prices[tower_type] = base_price
	
	_emit_tower_price(tower_type)

func _on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	# update build price and sell price
	var base_price = get_base_price(tower_type)
	var new_price = get_next_price(base_price, amount)
	
	sell_prices[tower_type] = build_prices[tower_type]
	build_prices[tower_type] = new_price
	if _next_tower_is_free():
		_free_towers_available -= 1
		if _free_towers_available == 0:
			_emit_all_towers_change()
	else:
		_emit_tower_price(tower_type)
	
func _next_tower_is_free() -> bool:
	return _free_towers_available >= 1

func _emit_tower_price(tower_type: Tower.TowerType) -> void:
	if _next_tower_is_free():
		tower_price_change.emit(tower_type, 0)
	else:
		tower_price_change.emit(tower_type, build_prices[tower_type])

func _emit_all_towers_change() -> void:
	# when towers are free
	if _next_tower_is_free():
		for tower_type in Tower.TowerType.values():
			tower_price_change.emit(tower_type, 0)
	else:
		for tower_type in Tower.TowerType.values():
			tower_price_change.emit(tower_type, build_prices[tower_type])
