@abstract
class_name UpgradeableTower extends Tower

@export var upgrade_sprite: Texture2D

const MAX_LEVEL: int = 2

var level: int = 1

func _ready():
	super._ready()

func upgrade() -> void:
	RunContext.economy.gold -= data.upgrade_price
	level += 1
	experience_handler.level_up.emit(level)

	if upgrade_sprite:
		sprite_2d.texture = upgrade_sprite

func is_max_level() -> bool:
	return level >= MAX_LEVEL

func get_upgradeable_towers() -> Array[TowerDataWithInstance]:
	return data.upgradeable_towers
