@abstract
class_name AttackModifier extends RefCounted

var source: Source

func _init(p_source: Source) -> void:
	source = p_source

@abstract
func on_before_hit(ctx: AttackContext) -> void
