@abstract
class_name Consumable extends GameItem

signal used(consumable: Consumable)
signal clicked(consumable: Consumable)

func _init(data: ItemData) -> void:
    super(data)
    type = ItemData.Type.CONSUMABLE

@abstract
func use() -> void

