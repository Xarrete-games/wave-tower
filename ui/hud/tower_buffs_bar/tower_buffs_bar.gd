class_name TowerBuffsBar
extends Control

@export var slot_scene: PackedScene

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
			
			buffs_modifiers_stacks[buff.data.id] += 1
			# else:
			# 	buffs_modifiers_stacks[buff.data.id] = 1

func _on_tower_buff_removed(source_id: String) -> void:
	pass

func _buff_exists(buff_id: String) -> bool:
	for buff in tower_buffs:
		if buff.data.id == buff_id:
			return true
	return false


