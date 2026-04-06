class_name TowerBuffsBar
extends Control

@export var slot_scene: PackedScene
@export var slots_container: Control

var tower: Tower
var tower_buffs: Array[TowerBuff] = []
var buffs_modifiers_stacks: Dictionary[String, int] = {}

func _ready() -> void:
	tower = get_parent() as Tower
	tower.buff_added.connect(_on_tower_buff_added)
	tower.buff_removed.connect(_on_tower_buff_removed)


func _on_tower_buff_added(buff: TowerBuff) -> void:
	if not _buff_exists(buff.data.id):
		tower_buffs.append(buff)
		if buff is TowerBuffStatsModifier:
			var modifier = buff as TowerBuffStatsModifier
			
			buffs_modifiers_stacks[buff.data.id] = modifier.value
			var slot = slot_scene.instantiate() as TowerBuffsBarSlot
			slots_container.add_child(slot)
			slot.set_buff(buff, modifier.value)
	else:
		if buff is TowerBuffStatsModifier:
			var modifier = buff as TowerBuffStatsModifier
			buffs_modifiers_stacks[buff.data.id] += modifier.value
			for slot in slots_container.get_children():
				var buff_slot = slot as TowerBuffsBarSlot
				if buff_slot.tower_buff.data.id == buff.data.id:
					buff_slot.value = buffs_modifiers_stacks[buff.data.id]

func _on_tower_buff_removed(source_id: String) -> void:
	for i in range(tower_buffs.size()):
		var buff = tower_buffs[i]
		if buff.source.id == source_id:
			if buff is TowerBuffStatsModifier:
				var modifier = buff as TowerBuffStatsModifier
				buffs_modifiers_stacks[buff.data.id] -= modifier.value
				for slot in slots_container.get_children():
					var buff_slot = slot as TowerBuffsBarSlot
					if buff_slot.tower_buff.data.id == buff.data.id:
						buff_slot.value = buffs_modifiers_stacks[buff.data.id]
			tower_buffs.remove_at(i)
			return

func _buff_exists(buff_id: String) -> bool:
	for buff in tower_buffs:
		if buff.data.id == buff_id:
			return true
	return false


