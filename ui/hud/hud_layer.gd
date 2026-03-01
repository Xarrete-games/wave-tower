class_name HUDLayer extends CanvasLayer

@export var level_waves_panel_scene: PackedScene
@export var damage_recount_panel_scene: PackedScene

var level_waves_panel: EnemyWavesPanel = null

@onready var damage_recount_panel: Control = $DamageRecountPanel

func _ready() -> void:
	ClickEvents.level_progess_hovered.connect(_show_level_waves_panel)
	ClickEvents.level_progess_unhovered.connect(_hide_level_waves_panel)

func _input(event: InputEvent) -> void:
	if event.is_action_released("recount_panel"):
		
		damage_recount_panel.visible = false
	if event.is_action_pressed("recount_panel"):
		damage_recount_panel.visible = true

func _show_level_waves_panel() -> void:
	pass
	# TODO : romove unused code
	#level_waves_panel = level_waves_panel_scene.instantiate()
	#add_child(level_waves_panel)

func _hide_level_waves_panel() -> void:
	if level_waves_panel:
		level_waves_panel.queue_free()
		level_waves_panel = null
