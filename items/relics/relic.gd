@abstract
class_name Relic extends GameItem

enum Rarity { COMMON, RARE, EPIC}

var rarity: Rarity
var max_stacks: int = 1

func _init(data: RelicItemData) -> void:
	super(data)
	type = ItemData.Type.RELIC
	rarity = data.rarity
	max_stacks = data.max_stacks

@abstract
func apply_effect() -> void
