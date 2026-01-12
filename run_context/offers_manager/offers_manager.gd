class_name OffersManager extends RefCounted

var all_consumables_data: Array[ItemData] = []
var relics_offers_manager: RelicOffersManager = RelicOffersManager.new()

func _init() -> void:
	relics_offers_manager = RelicOffersManager.new()
	all_consumables_data = DataLoader.get_all_consumables()


# ---------------------------------------------------------
# RELIC OFFERS API
# ---------------------------------------------------------

func get_relic_offer_by_id(relic_ids: Array[String]) -> Array[ItemOffer]:
	return relics_offers_manager.get_relics_offers_by_ids(relic_ids)	
	
func create_relic_offers(amount: int) -> Array[ItemOffer]:
	return relics_offers_manager.create_relic_offers(amount)

func create_relic_offer_from_data(data: ItemData) -> ItemOffer:
	return relics_offers_manager.create_relic_offer_from_data(data)

func increase_relic_offer_price(item_offer: ItemOffer) -> void:
	relics_offers_manager.increase_relic_offer_price(item_offer)

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


