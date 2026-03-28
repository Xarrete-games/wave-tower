class_name IceCream extends Relic

func on_before_get_loot(ctx: LootContext) -> void:
	ctx.extra_gold += 10