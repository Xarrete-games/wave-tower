extends Node

signal tower_price_change(tower_type: Tower.Type, price: int)

const PRICE_INCREASE_PERCENT := 0.25

enum TowerBuild {
	RED = 50,
	GREEN = 60,
	BLUE = 70,
}

var base_prices: Dictionary[Tower.Type, int]= {
	Tower.Type.RED: TowerBuild.RED,
	Tower.Type.GREEN: TowerBuild.GREEN,
	Tower.Type.BLUE: TowerBuild.BLUE,
}

var build_prices: Dictionary[Tower.Type, int]= base_prices.duplicate()
var sell_prices: Dictionary[Tower.Type, int]= base_prices.duplicate()

func _ready() -> void:
	await RunContext.initialized
	RunContext.towers_count.tower_count_change.connect(_on_tower_count_change)

func get_price(tower_type: Tower.Type) -> int:
	return build_prices[tower_type]

func get_sell_price(tower_type: Tower.Type) -> int:
	return int(round(sell_prices[tower_type] / 2.0))

func get_base_price(tower_type: Tower.Type) -> int:
	return base_prices[tower_type]

func _get_price_for_amount(base_price: int, amount: int) -> int:
	if amount <= 0:
		return base_price
	
	var increase := base_price * PRICE_INCREASE_PERCENT * amount
	return base_price + roundi(increase)

func _on_tower_count_change(tower_type: Tower.Type, amount: int) -> void:
	var base_price := get_base_price(tower_type)

	# precio para construir una torre más
	build_prices[tower_type] = _get_price_for_amount(base_price, amount)

	# precio base de venta (torre anterior)
	sell_prices[tower_type] = _get_price_for_amount(base_price, amount - 1)

	_emit_tower_price(tower_type)

func _emit_tower_price(tower_type: Tower.Type) -> void:
	tower_price_change.emit(tower_type, build_prices[tower_type])
