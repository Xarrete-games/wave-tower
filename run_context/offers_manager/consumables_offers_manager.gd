class_name ConsumablesOffersManager extends RefCounted

var all_consumables_data: Array[ConsumableData] = []

func _init() -> void:
	all_consumables_data = DataLoader.get_all_consumables()

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
		data.health_price
	)