class_name EventData extends Resource

enum Type { OPTIONS, SHOP, CHOOSE_RELIC }
enum Role { FRIENDLY, RANDOM, HOSTILE }

@export var id: String
@export var type: Type
@export var role: Role
@export var title: String
@export_multiline var description: String
@export var icon: Texture2D
@export var texture_background: Texture2D
@export_group("Script")
@export var runtime_script: Script 
