@abstract
class_name Relic extends GameItem

enum Rarity { COMMON, RARE, EPIC}

var rarity: Rarity
var max_stacks: int = 1
var is_cursed: bool = false
var disabled: bool = false

func _init(data: RelicItemData) -> void:
	super(data)
	type = ItemData.Type.RELIC
	rarity = data.rarity
	max_stacks = data.max_stacks
	is_cursed = data.is_cursed

@abstract
func apply_effect() -> void

@abstract
func remove_effect() -> void
