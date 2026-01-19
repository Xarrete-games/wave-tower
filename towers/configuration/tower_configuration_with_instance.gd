class_name TowerConfigurationWithInstance extends Resource

@export var configuration: TowerConfiguration
@export var scene: PackedScene

func get_instance() -> Tower:
	var instance = scene.instantiate() as Tower
	instance.configuration = configuration
	instance.type = configuration.type
	return instance
