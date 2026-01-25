class_name RelicItemData extends ItemData

@export_group("Relic Stats")
@export var rarity: Relic.Rarity
@export var is_cursed: bool = false
@export var is_tome: bool = false
@export var only_for_events: bool = false
@export var max_stacks: int = 1