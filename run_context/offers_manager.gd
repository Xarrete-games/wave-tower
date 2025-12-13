class_name OffersManager extends RefCounted

var all_relic_data: Array[ItemData] = []

func _init() -> void:
	all_relic_data = DataLoader.get_all_relics()

func get_relic_offer_by_id(relic_ids: Array[String]) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []

	for relic_id in relic_ids:
		var relic_data = all_relic_data.filter(func(data: ItemData):
			return data.id == relic_id
		)

		if relic_data.size() == 0:
			continue

		var offer = create_offer_from_data(relic_data[0])
		offers.append(offer)
	return offers		
	

func create_relic_offers(amount: int) -> Array[ItemOffer]:
	var filter_relics = all_relic_data.filter(func(data: ItemData):
		return not RelicsManager.is_maxed(data.id)
	)
	filter_relics.shuffle()

	var offers: Array[ItemOffer] = []
	for data in filter_relics:
		if offers.size() >= amount:
			break

		offers.append(create_offer_from_data(data))

	return offers

func create_offer_from_data(data: ItemData) -> ItemOffer:
	var price = data.price * (1.0 - RunContext.economy.relics_discount_mult)

	return ItemOffer.new(
		data,
		price,
		data.price_increased,
		data.health_price
)

func increase_offer_price(item_offer: ItemOffer) -> void:
	if item_offer.price_increases:
		item_offer.item_data.price = round(item_offer.item_data.price * 1.2)