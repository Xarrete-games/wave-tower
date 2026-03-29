class_name HUDLayer extends CanvasLayer

var level_waves_panel: EnemyWavesPanel = null

func _ready() -> void:
	ClickEvents.level_progess_hovered.connect(_show_level_waves_panel)
	ClickEvents.level_progess_unhovered.connect(_hide_level_waves_panel)

func _show_level_waves_panel() -> void:
	pass
	# TODO : romove unused code
	#level_waves_panel = level_waves_panel_scene.instantiate()
	#add_child(level_waves_panel)

func _hide_level_waves_panel() -> void:
	if level_waves_panel:
		level_waves_panel.queue_free()
		level_waves_panel = null
