class_name EnemyPaths extends Node2D

const MAX_OFFSET:int  = 40
const MIN_OFFSERT: int = 40

func get_new_path_follow(path_num: int) -> PathFollow2D:
	if path_num >= get_child_count():
		push_error("[EnemyPaths] invalidad path_num: %d , number of paths: %d" % [path_num, get_child_count()])
	
	var path_follow = PathFollow2D.new()

	#generate offset value
	var random_v_offset: float = randf_range(-MIN_OFFSERT, MAX_OFFSET)
	path_follow.v_offset = random_v_offset
	path_follow.rotates = false
	
	var path = get_child(path_num)
	
	path.add_child(path_follow)
	
	return path_follow
