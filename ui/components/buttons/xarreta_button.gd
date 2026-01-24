class_name XarretaButton extends Button

@export var theme_override: Theme
@export var disabled_theme_override: Theme

signal xarreta_mouse_entered()
signal xarreta_mouse_exited()
signal xarreta_pressed()

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	xarreta_mouse_entered.emit()
	
func _on_mouse_exited() -> void:
	xarreta_mouse_exited.emit()

func _on_pressed() -> void:
	AudioManager.play_button_click()
	xarreta_pressed.emit()

func disable_button() -> void:
	disabled = true
	if disabled_theme_override:
		theme = disabled_theme_override

func enable_button() -> void:
	disabled = false
	if theme_override:
		theme = theme_override
