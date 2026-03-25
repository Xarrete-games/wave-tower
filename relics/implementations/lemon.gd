class_name Lemon extends Relic

var is_active: bool = true

func on_tower_placed(_tower_instance: Tower) -> void:
	is_active = false

func on_get_price(ctx: PriceContext) -> void:
	if ctx.price_type == PriceContext.PriceType.TOWER and is_active:
		ctx.discount += 0.5

func on_wave_finished() -> void:
	is_active = true