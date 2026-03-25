class_name ConsumablesOffersManager extends RefCounted

const BASE_PRICE_BY_RARITY: Dictionary[BaseData.Rarity, int] = {
	BaseData.Rarity.COMMON: 50,
	BaseData.Rarity.RARE: 80,
	BaseData.Rarity.EPIC: 120,
}

var all_consumables_data: Array[ConsumableData] = []

func _init() -> void:
	all_consumables_data = DataLoader.get_all_consumables()

func create_consumables_offers(amount: int) -> Array[ItemOffer]:
	var consumables_data = all_consumables_data.duplicate()

	var offers: Array[ItemOffer] = []
	for data in consumables_data:
		if offers.size() >= amount:
			break

		offers.append(create_consumable_offer_from_data(data))

	return offers

func create_consumable_offer_from_data(data: BaseData) -> ItemOffer:
	var base_price: int = BASE_PRICE_BY_RARITY.get(data.rarity, BASE_PRICE_BY_RARITY[BaseData.Rarity.COMMON])
	var ctx = PriceContext.new(PriceContext.PriceType.CONSUMABLE, base_price)
	EconomyHooks.on_get_price(ctx)

	return ItemOffer.new(
		data,
		ctx.final_price,
		0
	)