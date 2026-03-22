class_name RelicData extends BaseData

@export_group("Relic")
@export var health_price: int = 0
@export var is_cursed: bool = false
@export var is_tome: bool = false
@export var only_for_events: bool = false
@export var max_stacks: int = 1

@export_group("Script")
@export var runtime_script: Script

func create_item() -> Relic:
	return runtime_script.new(self)
