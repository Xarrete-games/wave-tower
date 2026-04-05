class_name BuffData extends BaseData

@export_group("Script")
@export var runtime_script: Script

func create_item(source: Source = null) -> TowerBuff:
	if runtime_script == null:
		push_error("[BuffData] Missing runtime_script for buff id: %s" % id)
		return null
	if not runtime_script.has_method("create_instance"):
		push_error("[BuffData] runtime_script must implement static create_instance(data, source): %s" % runtime_script.resource_path)
		return null

	var actual_source = source
	if actual_source == null:
		actual_source = Source.new(Source.SourceType.GLOBAL, id)

	return runtime_script.call("create_instance", self, actual_source) as TowerBuff
