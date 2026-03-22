class_name RelicOffersManager extends RefCounted

const BASE_PRICE_BY_RARITY: Dictionary[BaseData.Rarity, int] = {
	BaseData.Rarity.COMMON: 50,
	BaseData.Rarity.RARE: 80,
	BaseData.Rarity.EPIC: 120,
}

var all_relic_data: Array[RelicData] = []
var relic_price_multiplier_by_id: Dictionary[String, float] = {}

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
	var price_multiplier: float = relic_price_multiplier_by_id.get(data.id, 1.0)
	var price = round(base_price * price_multiplier * (1.0 - RunContext.economy.relics_discount_mult))

	return ItemOffer.new(
		data,
		price,
		data.health_price
)

func increase_relic_offer_price(item_offer: ItemOffer) -> void:
	if item_offer.item_data is RelicData:
		var relic_data := item_offer.item_data as RelicData
		relic_price_multiplier_by_id[relic_data.id] = relic_price_multiplier_by_id.get(relic_data.id, 1.0) * 1.2


	
