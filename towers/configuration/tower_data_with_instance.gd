class_name TowerDataWithInstance extends Resource

@export var configuration: TowerData
@export var scene: PackedScene

func get_instance() -> Tower:
	var instance = scene.instantiate() as Tower
	instance.configuration = configuration
	instance.type = configuration.type
	return instance
