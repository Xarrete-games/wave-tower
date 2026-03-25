class_name ButterfishNigiri extends Relic

const DISCOUNT_AMOUNT: float = 0.1

func on_get_price(ctx: PriceContext) -> void:
	if ctx.price_type == PriceContext.PriceType.CONSUMABLE:
		ctx.discount += DISCOUNT_AMOUNT