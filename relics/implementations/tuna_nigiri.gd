class_name TunaNigiri extends Relic

func on_get_price(ctx: PriceContext) -> void:
	if ctx.price_type == PriceContext.PriceType.TOWER:
		ctx.discount += 0.1