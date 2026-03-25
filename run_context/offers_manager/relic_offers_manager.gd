class_name RelicOffersManager extends RefCounted

const BASE_PRICE_BY_RARITY: Dictionary[BaseData.Rarity, int] = {
	BaseData.Rarity.COMMON: 50,
	BaseData.Rarity.RARE: 80,
	BaseData.Rarity.EPIC: 120,
}

var all_relic_data: Array[RelicData] = []

func _init() -> void:
	all_relic_data = DataLoader.get_all_relics()

func get_relic_offer_by_id(relic_id: String) -> ItemOffer:
	var index = all_relic_data.find(func(data: RelicData):
		return data.id == relic_id
	)

	if index == -1:
		return null

	return create_relic_offer_from_data(all_relic_data[index])

func get_relics_offers_by_ids(relic_ids: Array[String]) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []

	for relic_id in relic_ids:
		var relic_data = all_relic_data.filter(func(data: RelicData):
			return data.id == relic_id
		)

		if relic_data.size() == 0:
			continue

		var offer = create_relic_offer_from_data(relic_data[0])
		offers.append(offer)
	return offers		
	
func create_relic_offers(amount: int) -> Array[ItemOffer]:
	var filter_relics = all_relic_data.filter(func(data: RelicData):
		return not RunContext.relics_manager.is_maxed(data.id) and not data.is_cursed and not data.only_for_events
	)
	filter_relics.shuffle()

	var offers: Array[ItemOffer] = []
	for data in filter_relics:
		if offers.size() >= amount:
			break

		offers.append(create_relic_offer_from_data(data))

	return offers

func create_relic_offer_from_data(data: RelicData) -> ItemOffer:
	var base_price: int = BASE_PRICE_BY_RARITY.get(data.rarity, BASE_PRICE_BY_RARITY[BaseData.Rarity.COMMON])
	var ctx = PriceContext.new(PriceContext.PriceType.RELIC, base_price)
	EconomyHooks.on_get_price(ctx)

	return ItemOffer.new(
		data,
		ctx.final_price,
		data.health_price
	)


	
