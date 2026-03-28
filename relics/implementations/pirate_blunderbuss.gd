class_name PirateBlunderbuss extends Relic

func on_before_get_loot(ctx: LootContext) -> void:
	ctx.chance_drop_consumable += 20