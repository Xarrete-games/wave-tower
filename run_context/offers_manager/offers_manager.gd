class_name OffersManager extends RefCounted

var all_consumables_data: Array[ConsumableData] = []
var relics_offers_manager: RelicOffersManager
var consumables_offers_manager: ConsumablesOffersManager

func _init() -> void:
	relics_offers_manager = RelicOffersManager.new()
	consumables_offers_manager = ConsumablesOffersManager.new()

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
	return consumables_offers_manager.get_cosumables_offer_by_id(consumables_ids)	
	
func create_consumables_offers(amount: int) -> Array[ItemOffer]:
	return consumables_offers_manager.create_consumables_offers(amount)
	
func create_consumable_offer_from_data(data: ItemData) -> ItemOffer:
	return consumables_offers_manager.create_consumable_offer_from_data(data)	
