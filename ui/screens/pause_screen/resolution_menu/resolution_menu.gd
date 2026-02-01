class_name ResolutionMenu extends Control

var resolutions: Dictionary[String, Vector2] = {
	"3840x2160": Vector2(3840, 2160),
	"2560x1440": Vector2(2560, 1440),
	"1920x1080": Vector2(1920, 1080),
	"1600x900": Vector2(1600, 900),
	"1366x768": Vector2(1366, 768),
	"1280x720": Vector2(1280, 720),
	"800x600": Vector2(800, 600)
}

@onready var option_button: OptionButton = $OptionButton

func _ready() -> void:
	_populate_resolutions()
	update_button_values()

func update_button_values() -> void:
	var windows_size: String = str(get_window().size.x) + "x" + str(get_window().size.y)
	print("Current window size: ", windows_size)
	var resolution_index: int = resolutions.keys().find(windows_size)
	option_button.selected = resolution_index

func _populate_resolutions() -> void:
	option_button.clear()

	var screen_size := DisplayServer.screen_get_size()

	for res_name in resolutions.keys():
		var res := resolutions[res_name]
		if res.x <= screen_size.x and res.y <= screen_size.y:
			option_button.add_item(res_name)

func _on_option_button_item_selected(index: int) -> void:
	var key = option_button.get_item_text(index)
	get_window().size = resolutions[key]
	center_window()


func center_window() -> void:
	var screen_center = DisplayServer.screen_get_position() + DisplayServer.screen_get_size() / 2
	var window_size = get_window().get_size_with_decorations()
	get_window().set_position(screen_center - window_size / 2)
