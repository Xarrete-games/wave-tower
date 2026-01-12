class_name EventData extends Resource

enum Type { SHOP, OPTIONS }

@export var id: String
@export var type: Type
@export var title: String
@export var description: String
@export var options: Array[String]
@export var icon: Texture2D
@export var texture_background: Texture2D
@export_group("Script")
@export var runtime_script: Script 
