class_name OffersManager extends RefCounted

var all_offers: Array[ItemOffer] = []
var all_rewards: Array[Relic] = []


func create_relic_offers(amount: int) -> Array[ItemOffer]:
	var candidates: Array[ItemData] = RelicCatalog.get_all().duplicate()

	# remove max stack relics
	candidates = candidates.filter(func(data: ItemData):
		return not RelicsManager.is_maxed(data.id)
	)

	# # add more filters

	candidates.shuffle()

	var offers: Array[ItemOffer] = []
	for data in candidates:
		if offers.size() >= amount:
			break

		offers.append(_create_offer_from_data(data))

	return offers

func _create_offer_from_data(data: ItemData) -> ItemOffer:

	# TODO GET THE NEW PRICE
	# if not context.is_free:
	# 	price = PriceCalculator.calculate(data, context)

	return ItemOffer.new(
		data,
		data.price,
		data.price_increased,
		data.health_price
	)