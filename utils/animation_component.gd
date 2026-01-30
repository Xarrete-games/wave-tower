class_name AnimationComponent extends Node

@export var from_center: bool = true
@export var hover_scale: Vector2 = Vector2(1.1, 1.1)
@export var time: float = 0.2
@export var transition_type: Tween.TransitionType = Tween.TransitionType.TRANS_LINEAR

var target: Control
var default_scale: Vector2


func _ready() -> void:
	target = get_parent() as Control
	connect_signals()
	call_deferred("setup")


func connect_signals() -> void:
	target.mouse_entered.connect(on_hovered)
	target.mouse_exited.connect(off_hovered)

func setup() -> void:
	if from_center:
		target.pivot_offset = target.size / 2
	default_scale = target.scale

func on_hovered() -> void:
	add_tween("scale", hover_scale, time)

func off_hovered() -> void:
	add_tween("scale", default_scale, time)

func add_tween(property: String, value: Variant, seconds: float) -> void:
	var tween: Tween = get_tree().create_tween()
	tween.tween_property(
		target,
		property,
		value,
		seconds
	).set_trans(transition_type)