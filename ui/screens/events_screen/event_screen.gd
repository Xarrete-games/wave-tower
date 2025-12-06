class_name EventScreen extends Control

signal event_selected(event: Event)

const EVENT_SLOT = preload("uid://cl23rwbjcvak5")

@export var events_container: Control

func set_events(events: Array[Event]) -> void:
	for event in events:
		var event_slot: EventSlot = EVENT_SLOT.instantiate()
		events_container.add_child(event_slot)
		event_slot.set_event(event)
		event_slot.event_pressed.connect(_on_event_pressed)

func _on_event_pressed(event: Event) -> void:
	event_selected.emit(event)
	queue_free()
