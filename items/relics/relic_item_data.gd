class_name RelicItemData extends ItemData

@export_group("Relic Stats")
@export var rarity: Relic.Rarity
@export var max_stacks: int = 1

func _init():
    type = ItemData.Type.RELIC