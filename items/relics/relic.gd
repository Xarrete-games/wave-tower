@abstract
class_name Relic extends GameItem

enum Rarity { COMMON, RARE, EPIC, ALL }

var rarity: Rarity
var max_stacks: int = 1
var is_cursed: bool = false
var is_tome: bool = false
var disabled: bool = false

func _init(data: RelicItemData) -> void:
	super(data)
	rarity = data.rarity
	max_stacks = data.max_stacks
	is_cursed = data.is_cursed
	is_tome = data.is_tome

@abstract
func apply_effect() -> void

@abstract
func remove_effect() -> void
