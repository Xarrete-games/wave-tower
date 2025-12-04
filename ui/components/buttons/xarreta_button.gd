class_name XarretaButton extends Button

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
