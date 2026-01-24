class_name RunHandler extends Node

const NEXT_WAVE_SCREEN = preload("uid://b7ttkk4pasgin")
const NEXT_LEVEL_SCREEN = preload("uid://crastw7xnqgvl")
const END_GAME_SCENE = preload("uid://ovtc0l4cimpl")
const WAVES_WITH_EVENTS = [2,6,8]
const WAVES_WITH_SHOPS = [4,8]
const WAVES_WITH_RELICS = [1,3,5,7,9,10]

var events: Array[EventData]
var shop_event: EventData
var choose_relic_event: EventData
var options_events: Array[EventData]

@export var event_layer: CanvasLayer
@export var events_screen_hander: EventsScreenHandler
@export var loot_screen_handler: LootScreenHandler



func _ready() -> void:
	_set_events_by_type()
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)
	RunContext.progress.last_wave_finished.connect(_on_last_wave_finished)
	RunContext.progress.current_level_changed.connect(_on_new_level_loaded)

func show_events_screen(event_data: EventData) -> void:
	await events_screen_hander.show_event_selected(event_data, event_layer)

func _set_events_by_type() -> void:
	events = DataLoader.get_all_events()
	shop_event = null
	choose_relic_event = null
	options_events = []
	
	for event_data in events:
		match event_data.type:
			EventData.Type.SHOP:
				shop_event = event_data
			EventData.Type.CHOOSE_RELIC:
				choose_relic_event = event_data
			EventData.Type.OPTIONS:
				options_events.append(event_data)

# NEXT WAVE SCREEN
func _show_next_wave_screen() -> void:
	var next_wave_screen = NEXT_WAVE_SCREEN.instantiate()
	event_layer.call_deferred("add_child", next_wave_screen)

# NEXT LEVEL SCREEN
func _show_next_level_menu() -> void:
	var next_level_screen = NEXT_LEVEL_SCREEN.instantiate()
	event_layer.call_deferred("add_child", next_level_screen)

# EVERY WAVE FINISHED
func _on_wave_finished() -> void:
	# LOOT SCREEN
	await loot_screen_handler.show_loot_screen(event_layer)

	# EVENT SCREEN
	var event = _get_next_event(RunContext.progress.current_wave)
	if event == null:
		_show_next_wave_screen()
		return
	await show_events_screen(event)
	_show_next_wave_screen()

func _on_last_wave_finished() -> void:
	RunContext.economy.gold += 50
	if RunContext.progress.is_last_level():
		await  get_tree().create_timer(5).timeout
		get_tree().change_scene_to_packed(END_GAME_SCENE)
	else:
		_show_next_level_menu()

func _on_new_level_loaded(_level_num: int) -> void:
	_show_next_wave_screen()

func _get_next_event(current_wave: int) -> EventData:
	var event_data: EventData = null
	if current_wave in WAVES_WITH_SHOPS and shop_event:
		event_data = shop_event
	elif current_wave in WAVES_WITH_RELICS and choose_relic_event:
		event_data = choose_relic_event
	elif current_wave in WAVES_WITH_EVENTS and options_events.size() > 0:
		options_events.shuffle()
		event_data = options_events.pop_back()
		
	return event_data
