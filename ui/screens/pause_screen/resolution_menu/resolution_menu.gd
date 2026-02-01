class_name ResolutionMenu extends Control

@export var resolution_option_button: OptionButton
@export var mode_option_button: OptionButton

var resolutions: Dictionary[String, Vector2] = {
	"3840x2160": Vector2(3840, 2160),
	"2560x1440": Vector2(2560, 1440),
	"1920x1080": Vector2(1920, 1080),
	"1600x900": Vector2(1600, 900),
	"1366x768": Vector2(1366, 768),
	"1280x720": Vector2(1280, 720),
	"800x600": Vector2(800, 600)
}
var visible_resolutions: Array[String] = []

func _ready() -> void:
	_populate_resolutions()
	update_button_values()

func update_button_values() -> void:
	var window_size := get_window().size
	var current := "%dx%d" % [window_size.x, window_size.y]

	var index := visible_resolutions.find(current)
	if index != -1:
		resolution_option_button.selected = index

func _populate_resolutions() -> void:
	resolution_option_button.clear()
	visible_resolutions.clear()

	var screen_size := DisplayServer.screen_get_size()

	for res_name in resolutions.keys():
		var res := resolutions[res_name]
		if res.x <= screen_size.x and res.y <= screen_size.y:
			visible_resolutions.append(res_name)
			resolution_option_button.add_item(res_name)

func _on_option_button_item_selected(index: int) -> void:
	var key: String = visible_resolutions[index]
	get_window().size = resolutions[key]
	center_window()

func center_window() -> void:
	var screen_center = DisplayServer.screen_get_position() + DisplayServer.screen_get_size() / 2
	var window_size = get_window().get_size_with_decorations()
	get_window().set_position(screen_center - window_size / 2)
