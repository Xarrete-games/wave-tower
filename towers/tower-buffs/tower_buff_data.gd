class_name BuffData extends BaseData

@export_group("Script")
@export var runtime_script: Script

func create_item(source: Source = null, value: int = 0) -> TowerBuff:
	if runtime_script == null:
		push_error("[BuffData] Missing runtime_script for buff id: %s" % id)
		return null
	if not runtime_script.has_method("create_instance"):
		push_error("[BuffData] runtime_script must implement static create_instance(data, source, value): %s" % runtime_script.resource_path)
		return null

	return runtime_script.call("create_instance", self, source, value) as TowerBuff
