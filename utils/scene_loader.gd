class_name SceneLoader extends Node

## Private method to open a directory and retrieve a list of .tscn file names.
## Returns an Array of file names or null on error.
static func _get_scene_files(path: String) -> Array:
	var dir: DirAccess = DirAccess.open(path)
	
	if not dir:
		push_error("[SceneLoader]: Invalid directory path: " + path + ".")
		return []

	var scene_files = []
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	# Filter only .tscn files
	while file_name != "":
		if not dir.current_is_dir() and file_name.ends_with(".tscn"):
			scene_files.append(file_name)
		file_name = dir.get_next()
	dir.list_dir_end()
	
	# Error if no .tscn files are found
	if scene_files.size() == 0:
		push_error("[SceneLoader]: No .tscn files found in " + path)
		return []
		
	return scene_files

## Loads a random PackedScene from the given directory path.
## Returns the loaded PackedScene or null on error.
static func get_random_scene_from_path(path: String) -> PackedScene:
	var scene_files = _get_scene_files(path)
	
	if scene_files.is_empty():
		return null

	# Load a random scene
	var random_index = randi() % scene_files.size()
	var random_scene_name = scene_files[random_index]
	
	var full_path = path.path_join(random_scene_name)
	var loaded_scene: PackedScene = load(full_path)
	
	if loaded_scene is PackedScene:
		return loaded_scene	
	else:
		push_error("[SceneLoader]: Resource is not a PackedScene: " + full_path)
		return null

## Loads a PackedScene at a specific index from the given directory path.
## Returns the loaded PackedScene or null on error.
static func get_indexed_scene_from_path(path: String, index: int) -> PackedScene:
	var scene_files = _get_scene_files(path)
	
	if scene_files.is_empty():
		return null

	# Check if the index is valid
	if index < 0 or index >= scene_files.size():
		push_error("[SceneLoader]: Index " + str(index) + " is out of bounds (0 to " + str(scene_files.size() - 1) + ") for path: " + path)
		return null
		
	# Get the scene name at the specified index
	var indexed_scene_name = scene_files[index]
	var full_path = path.path_join(indexed_scene_name)
	
	# Load the scene
	var loaded_scene: PackedScene = load(full_path)
	
	if loaded_scene is PackedScene:
		return loaded_scene	
	else:
		push_error("[SceneLoader]: Resource is not a PackedScene: " + full_path)
		return null
