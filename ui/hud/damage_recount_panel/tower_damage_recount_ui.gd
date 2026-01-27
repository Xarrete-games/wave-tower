class_name TowerDamageRecountUI extends Control

@export var tower_damage_recount_row: PackedScene

var rows: Dictionary[String, TowerDamageRecountRow] = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	RunContext.damage_recount.damage_recount_change.connect(_on_damage_recount_change)
	RunContext.progress.current_level_changed.connect(func(_level_num: int) -> void:
		var children = get_children()
		if children.size() > 0:
			# Keep the first child (e.g. header); free the rest from last->first
			for i in range(children.size() - 1, 0, -1):
				var child = children[i]
				# If this child is tracked in `rows`, remove its entry
				for key in rows.keys():
					if rows[key] == child:
						rows.erase(key)
						break
				child.queue_free()
	)

func _on_damage_recount_change(data: DamageRecountData) -> void:
	if not rows.has(data.id):
		var row = tower_damage_recount_row.instantiate() as TowerDamageRecountRow
		add_child(row)
		row.tower_id = data.id
		rows[data.id] = row
	rows[data.id].set_damage(data.wave, data.damage)
		
