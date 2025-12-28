@abstract
class_name UpgradeableTower extends Tower

const MAX_LEVEL: int = 2

var level: int = 1

func _ready():
	super._ready()

func upgrade() -> void:
	RunContext.economy.gold -= configuration.upgrade_price
	level += 1
	experience_handler.level_up.emit(level)

func is_max_level() -> bool:
	print("Checking if max level: ", level, " / ", MAX_LEVEL)
	return level >= MAX_LEVEL

func get_upgradeable_towers() -> Array[TowerConfigurationWithInstance]:
	return configuration.upgradeable_towers




