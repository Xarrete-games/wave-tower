class_name IceCream extends Relic

func apply_effect() -> void:
	RunContext.loot_manager.extra_gold += 10

func remove_effect() -> void:
	RunContext.loot_manager.extra_gold -= 10