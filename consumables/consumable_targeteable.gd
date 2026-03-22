@abstract
class_name ConsumableTargeteable extends Consumable

enum TargetType { BLOCKED_TILE, TOWER }

func use(target: Variant) -> void:
	action(target)
	used.emit(self)

@abstract
func action(target: Variant) -> void
