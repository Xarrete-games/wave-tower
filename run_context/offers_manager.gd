class_name OffersManager extends RefCounted

var all_relic_data: Array[ItemData] = []
var all_consumables_data: Array[ItemData] = []

func _init() -> void:
	all_relic_data = DataLoader.get_all_relics()
	all_consumables_data = DataLoader.get_all_consumables()


# ---------------------------------------------------------
# RELIC OFFERS API
# ---------------------------------------------------------

func get_relic_offer_by_id(relic_ids: Array[String]) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []

	for relic_id in relic_ids:
		var relic_data = all_relic_data.filter(func(data: ItemData):
			return data.id == relic_id
		)

		if relic_data.size() == 0:
			continue

		var offer = create_relic_offer_from_data(relic_data[0])
		offers.append(offer)
	return offers		
	
func create_relic_offers(amount: int) -> Array[ItemOffer]:
	var filter_relics = all_relic_data.filter(func(data: ItemData):
		return not RunContext.relics_manager.is_maxed(data.id)
	)
	filter_relics.shuffle()

	var offers: Array[ItemOffer] = []
	for data in filter_relics:
		if offers.size() >= amount:
			break

		offers.append(create_relic_offer_from_data(data))

	return offers

func create_relic_offer_from_data(data: ItemData) -> ItemOffer:
	var price = data.price * (1.0 - RunContext.economy.relics_discount_mult)

	return ItemOffer.new(
		data,
		price,
		data.price_increased,
		data.health_price
)

func increase_relic_offer_price(item_offer: ItemOffer) -> void:
	if item_offer.price_increases:
		item_offer.item_data.price = round(item_offer.item_data.price * 1.2)

# ---------------------------------------------------------
# COSUMABLES OFFERS API
# ---------------------------------------------------------

func get_cosumables_offer_by_id(consumables_ids: Array[String]) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []

	for consumable_id in consumables_ids:
		var consumable_data = all_consumables_data.filter(func(data: ItemData):
			return data.id == consumable_id
		)

		if consumable_data.size() == 0:
			continue

		var offer = create_consumable_offer_from_data(consumable_data[0])
		offers.append(offer)
	return offers		
	
func create_consumables_offers(amount: int) -> Array[ItemOffer]:
	var consumables_data = all_consumables_data.duplicate()

	var offers: Array[ItemOffer] = []
	for data in consumables_data:
		if offers.size() >= amount:
			break

		offers.append(create_consumable_offer_from_data(data))

	return offers

func create_consumable_offer_from_data(data: ItemData) -> ItemOffer:
	var price = data.price * (1.0 - RunContext.economy.consumables_discount_mult)

	return ItemOffer.new(
		data,
		price,
		data.price_increased,
		data.health_price
)


