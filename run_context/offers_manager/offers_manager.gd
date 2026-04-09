class_name OffersManager extends RefCounted

var all_consumables_data: Array = []
var relics_offers_manager: RelicOffersManager
var consumables_offers_manager: ConsumablesOffersManager

func _init() -> void:
	relics_offers_manager = RelicOffersManager.new()
	consumables_offers_manager = ConsumablesOffersManager.new()

# ---------------------------------------------------------
# RELIC OFFERS API
# ---------------------------------------------------------
	
func create_relic_offers(amount: int) -> Array[ItemOffer]:
	return relics_offers_manager.create_relic_offers(amount)

func create_relic_offer_from_data(data) -> ItemOffer:
	return relics_offers_manager.create_relic_offer_from_data(data)

func create_relic_offers_from_data(data: Array) -> Array[ItemOffer]:
	var offers: Array[ItemOffer] = []
	for datum in data:
		offers.append(relics_offers_manager.create_relic_offer_from_data(datum))
	return offers

# ---------------------------------------------------------
# COSUMABLES OFFERS API
# ---------------------------------------------------------

func create_consumables_offers(amount: int) -> Array[ItemOffer]:
	return consumables_offers_manager.create_consumables_offers(amount)
	
func create_consumable_offer_from_data(data) -> ItemOffer:
	return consumables_offers_manager.create_consumable_offer_from_data(data)	

func purchase_offer(item_offer: ItemOffer) -> Variant:
	RunContext.economy.gold -= item_offer.price
	if item_offer.health_price > 0:
		RunContext.status.health -= item_offer.health_price

	var item = item_offer.item_data.create_item()
	if item is Consumable:
		RunContext.consumables_manager.add_consumable(item)
	else:
		RunContext.relics_manager.add_relic(item)

	return item
