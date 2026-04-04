extends Node

enum PositionHint {
	RIGHT,
	BOTTOM,
}

const HINT = preload("uid://dfcxnivp7sm3h")
const offsett = Vector2(0, 0)

var hints: Dictionary[Control, Hint] = {}

func show_hint(parent: Control, text: String, title: String = "", pos: PositionHint = PositionHint.BOTTOM) -> void:
	var hint: Hint = HINT.instantiate()
	get_tree().get_root().add_child(hint)
	hint.set_text(text)
	hint.set_title(title)
	# position relative to the parent by default
	var base_pos = parent.global_position + get_offset(parent, pos)
	hint.set_position(base_pos)
	hints[parent] = hint
	# if node is on the right half, position the hint to the left of the parent
	if not is_on_left_side(parent) and pos == PositionHint.BOTTOM:
		var vp = get_viewport()
		var hint_w = hint.get_size().x
		var new_pos = hint.get_position()
		# place the hint so its right edge matches the parent's right edge
		new_pos.x = parent.global_position.x + parent.size.x - hint_w
		new_pos.x = clamp(new_pos.x, 0, vp.size.x - hint_w)
		# ensure vertical position is at the bottom of the parent
		new_pos.y = parent.global_position.y + parent.size.y
		hint.set_position(new_pos)

func remove_hint(parent: Control) -> void:
	if hints.has(parent):
		hints[parent].queue_free()
		hints.erase(parent)

func get_offset(parent: Control, pos: PositionHint) -> Vector2:

	if pos == PositionHint.RIGHT:
		return offsett + Vector2(parent.size.x, 0)
	else:
		return offsett + Vector2(0, parent.size.y)

func is_on_left_side(parent: Control) -> bool:
	var half_x = get_viewport().size.x * 0.5
	return parent.global_position.x < half_x

# usage
	
