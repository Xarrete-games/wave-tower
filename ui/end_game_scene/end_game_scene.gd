class_name EndGameScene extends CanvasLayer

var MAIN_MENU = load("uid://4i6kl0xurgeg")
var fade_duration: float = 5.0
var ready_to_exit: bool = false

@onready var panel: Panel = $Panel
@onready var label_3: Label = $PanelContainer/CenterContainer/VBoxContainer2/Label3

func _ready() -> void:
	label_3.visible = false
	panel.modulate = Color.WHITE
	panel.modulate.a = 1.0 
	fade_in()

func _input(event: InputEvent) -> void:	
	if event is InputEventKey:
		if event.is_pressed(): 
			if ready_to_exit:
				get_tree().change_scene_to_packed(MAIN_MENU)
		
func fade_in() -> void:
	var tween = create_tween()
	tween.tween_property(panel, "modulate:a", 0.0, fade_duration)

func _on_exit_timer_timeout() -> void:
	label_3.visible = true
	ready_to_exit = true
