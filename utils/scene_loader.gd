class_name SceneLoader extends Node

static func get_random_scene_from_path(path: String) -> PackedScene:
	var dir: DirAccess = DirAccess.open(path)
	
	if not dir:
		push_error("[SceneLoader]: Invalidad directory " + path + ".")
		return null
	var scene_files = []
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	# filer only .tscn files
	while file_name != "":
		if not dir.current_is_dir() and file_name.ends_with(".tscn"):
			scene_files.append(file_name)
		file_name = dir.get_next()
	dir.list_dir_end()
	
	# if not files error
	if scene_files.size() == 0:
		push_error("[SceneLoader]: .tscn not found in " + path)
		return null
		
	# load a random scene
	var random_index = randi() % scene_files.size()
	var random_scene_name = scene_files[random_index]
	var full_path = path.path_join(random_scene_name)
	var loaded_scene: PackedScene = load(full_path)
	
	if loaded_scene is PackedScene:
		return loaded_scene	
	else:
		push_error("[SceneLoader]: Resource is not a PackedScene: " + full_path)
		return null
		
