class_name TowerDataWithInstance extends Resource


@export var data: TowerData
@export var scene: PackedScene

func get_instance() -> Tower:
	var instance = scene.instantiate() as Tower
	instance.data = data
	instance.type = data.type
	return instance
