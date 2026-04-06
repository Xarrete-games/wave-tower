@abstract
class_name ConsumableTargeteable extends Consumable

enum TargetType { BLOCKED_TILE, TOWER }

var target: Variant

func use(p_target: Variant) -> void:
	target = p_target
	action(target)
	used.emit(self)

@abstract
func action(p_target: Variant) -> void
