@abstract
class_name Relic extends GameItem

func _init(data: ItemData) -> void:
    super(data)
    type = ItemData.Type.RELIC

@abstract
func apply_effect() -> void
