class_name RunHandler extends Node

const NEXT_WAVE_SCREEN = preload("uid://b7ttkk4pasgin")
const NEXT_LEVEL_SCREEN = preload("uid://crastw7xnqgvl")
const END_GAME_SCENE = preload("uid://ovtc0l4cimpl")
const WAVES_WITH_EVENTS = [8]
const WAVES_WITH_SHOPS = [1,4]
const WAVES_WITH_RELICS = [2,6,10]

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
	# for procedural level testing
	_on_new_level_loaded(0)

func show_loot_screen() -> void:
	await loot_screen_handler.show_loot_screen(event_layer)

func show_choose_card_screen() -> void:
	var cards = RunContext.towers_manager.get_random_towers(2)
	await ChooseTowerScreen.show_screen(cards, event_layer)

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
	AudioManager.play_wave_clear()
	await show_loot_screen()
	# 2 CARD CHOICES
	await show_choose_card_screen()
	await show_choose_card_screen()

	if RunContext.is_on_restarting:
		return

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
	# Repeat schedule every 10 waves:
	# 1..10, 11..20, 21..30, ...
	var wave_in_cycle: int = ((current_wave - 1) % 10) + 1

	var event_data: EventData = null
	if wave_in_cycle in WAVES_WITH_SHOPS and shop_event:
		event_data = shop_event
	elif wave_in_cycle in WAVES_WITH_RELICS and choose_relic_event:
		event_data = choose_relic_event
	elif wave_in_cycle in WAVES_WITH_EVENTS and options_events.size() > 0:
		options_events.shuffle()
		event_data = options_events.pop_back()
		
	return event_data
