class_name RelicOffersManager extends RefCounted

const BASE_PRICE_BY_RARITY: Dictionary[int, int] = {
	0: 50,
	1: 80,
	2: 120,
}

var all_relic_data: Array = []

func _init() -> void:
	all_relic_data = DataLoader.get_all_relics()

func get_relics_offers_by_ids(relic_ids: Array[String]) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []

	for relic_id in relic_ids:
		var relic_data = all_relic_data.filter(func(data):
			return data.id == relic_id
		)

		if relic_data.size() == 0:
			continue

		var offer = create_relic_offer_from_data(relic_data[0])
		offers.append(offer)
	return offers		
	
func create_relic_offers(amount: int) -> Array[ItemOffer]:
	var filter_relics = all_relic_data.filter(func(data):
		return not RunContext.relics_manager.has_relic(data.id) and not data.is_cursed and not data.only_for_events
	)
	filter_relics.shuffle()

	var offers: Array[ItemOffer] = []
	for data in filter_relics:
		if offers.size() >= amount:
			break

		offers.append(create_relic_offer_from_data(data))

	return offers

func create_relic_offer_from_data(data) -> ItemOffer:
	var base_price: int = BASE_PRICE_BY_RARITY.get(data.rarity, BASE_PRICE_BY_RARITY[0])
	var ctx = PriceContext.new(PriceContext.PriceType.RELIC, base_price)
	Hooks.on_get_price(ctx)

	return ItemOffer.new(
		data,
		ctx.final_price,
		data.health_price
	)


	
