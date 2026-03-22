class_name EnemyDebuffData extends BaseData

@export_group("Debuff")
@export var debuff_type: EnemyDebuff.Type
@export var value: float = 0.0
@export var duration: float = 0.0
@export var tick_interval: float = 0.0
@export var max_stacks: int = 99

@export_group("Script")
@export var runtime_script: Script

func create_item() -> Variant:
	if runtime_script == null:
		push_error("[EnemyDebuffData] Missing runtime_script for debuff id: %s" % id)
		return null
	return runtime_script.new(self)
