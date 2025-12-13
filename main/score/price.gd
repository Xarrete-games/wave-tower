#price.gd
extends Node

signal tower_price_change(tower_type: Tower.Type, price: int)
const PRICE_INCREASE_PERCENT: float = 0.25

enum TowerBuild {
	RED = 50,
	GREEN = 60,
	BLUE = 70,
}

var base_prices: Dictionary[Tower.Type, int] = {
	Tower.Type.RED: TowerBuild.RED,
	Tower.Type.GREEN: TowerBuild.GREEN,
	Tower.Type.BLUE: TowerBuild.BLUE,
}

var build_prices: Dictionary[Tower.Type, int] = {
	Tower.Type.RED: TowerBuild.RED,
	Tower.Type.GREEN: TowerBuild.GREEN,
	Tower.Type.BLUE: TowerBuild.BLUE,
}

var sell_prices: Dictionary[Tower.Type, int] = {
	Tower.Type.RED: TowerBuild.RED,
	Tower.Type.GREEN: TowerBuild.GREEN,
	Tower.Type.BLUE: TowerBuild.BLUE,
}

func _ready() -> void:
	TowerPlacementManager.tower_count_change.connect(_on_tower_count_change)

func get_price(tower_type: Tower.Type) -> int:
	return build_prices[tower_type]

func get_sell_price(tower_type: Tower.Type) -> int:
	return int(round(sell_prices[tower_type] / 2.0))

func get_base_price(tower_type: Tower.Type) -> int:
	return base_prices[tower_type]

func get_next_price(base_price: int, amount: int) -> int:
	var increase_amount = float(base_price) * PRICE_INCREASE_PERCENT * float(amount)
	return base_price + roundi(increase_amount)

func _on_tower_count_change(
	tower_type: Tower.Type, 
	amount: int, 
	event: TowerPlacementManager.TowerEvent) -> void:
		if event == TowerPlacementManager.TowerEvent.SOLD:
			_on_tower_sold(tower_type, amount)
		else:
			_on_tower_placed(tower_type, amount)

func _on_tower_sold(tower_type: Tower.Type, amount: int) -> void:
	var base_price = get_base_price(tower_type)
	var new_price = get_next_price(base_price, amount)
	
	build_prices[tower_type] = new_price
	if amount >= 1:
		var new_sell_price = get_next_price(base_price, amount - 1)
		sell_prices[tower_type] = new_sell_price
	else:
		sell_prices[tower_type] = base_price
	
	_emit_tower_price(tower_type)

func _on_tower_placed(tower_type: Tower.Type, amount: int) -> void:
	# update build price and sell price
	var base_price = get_base_price(tower_type)
	var new_price = get_next_price(base_price, amount)
	
	sell_prices[tower_type] = build_prices[tower_type]
	build_prices[tower_type] = new_price
	_emit_tower_price(tower_type)
	

func _emit_tower_price(tower_type: Tower.Type) -> void:
	tower_price_change.emit(tower_type, build_prices[tower_type])
