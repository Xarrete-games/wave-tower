class_name AnimationComponent extends Node

signal entered()

const IMMEDIATE_TRANSITION: Tween.TransitionType = Tween.TRANS_LINEAR

@export_group("Options")
@export var from_center: bool = true
@export var parallel_animations: bool = true
@export var enter_animation: bool = false
@export var properties: Array[String] = [
	"scale",
	"position",
	"rotation",
	"size",
	"self_modulate"
]
@export var flicked: bool = false
@export_group("Hover Settings")
@export var hover_time: float = 0.2
@export var hover_delay: float = 0.0
@export var hover_transition: Tween.TransitionType
@export var hover_easing: Tween.EaseType = Tween.EaseType.EASE_IN_OUT
@export var hover_scale: Vector2 = Vector2(1, 1)
@export var hover_position: Vector2
@export var hover_rotation: float
@export var hover_size: Vector2
@export var hover_modulate: Color = Color.WHITE

@export_group("Enter Settings")
@export var wait_for: AnimationComponent
@export var enter_time: float = 0.2
@export var enter_delay: float = 0.0
@export var enter_transition: Tween.TransitionType
@export var enter_easing: Tween.EaseType = Tween.EaseType.EASE_IN_OUT
@export var enter_scale: Vector2 = Vector2(1, 1)
@export var enter_position: Vector2
@export var enter_rotation: float
@export var enter_size: Vector2
@export var enter_modulate: Color = Color.WHITE

var target: Control
var default_scale: Vector2
var hover_values: Dictionary
var enter_values: Dictionary
var default_values: Dictionary

func _ready() -> void:
	target = get_parent() as Control
	call_deferred("setup")

func connect_signals() -> void:
	target.mouse_entered.connect(add_tween.bind(
		hover_values,
		parallel_animations,
		hover_time,
		hover_delay,
		hover_transition,
		hover_easing,
	))

	target.mouse_exited.connect(add_tween.bind(
		default_values,
		parallel_animations,
		hover_time,
		hover_delay,
		hover_transition,
		hover_easing,
	))
	if wait_for:
		wait_for.entered.connect(func() -> void:
			add_tween(
				default_values,
				parallel_animations,
				enter_time,
				enter_delay,
				enter_transition,
				enter_easing,
				true
			)
		)

func setup() -> void:
	if from_center:
		target.pivot_offset = target.size / 2
	default_scale = target.scale
	default_values = {
		"scale": target.scale,
		"position": target.position,
		"rotation": target.rotation,
		"size": target.size,
		"self_modulate": target.self_modulate,
	}
	hover_values = {
		"scale": hover_scale,
		"position": target.position + hover_position,
		"rotation": target.rotation + deg_to_rad(hover_rotation),
		"size": target.size * hover_size,
		"self_modulate": hover_modulate,
	}
	enter_values = {
		"scale": enter_scale,
		"position": target.position + enter_position,
		"rotation": target.rotation + deg_to_rad(enter_rotation),
		"size": target.size * enter_size,
		"self_modulate": enter_modulate,
	}
	connect_signals()
	if enter_animation:
		on_enter()
	else:
		entered.emit()

func on_enter() -> void:
	add_tween(
		enter_values,
		true,
		0.0,
		0.0,
		IMMEDIATE_TRANSITION,
		Tween.EaseType.EASE_IN,
	)

	if not wait_for:
		add_tween(
			default_values,
			parallel_animations,
			enter_time,
			enter_delay,
			enter_transition,
			enter_easing,
			true
		)

func add_tween(
	values: Dictionary, 
	parallel: bool, 
	seconds: float,
	delay: float,
	transition: Tween.TransitionType, 
	easing: Tween.EaseType,
	entering: bool = false
	) -> void:
	
	# proteect if a hover remove the component
	if not is_inside_tree():
		return
	var tween: Tween = get_tree().create_tween()
	tween.set_parallel(parallel)
	tween.set_pause_mode(Tween.TWEEN_PAUSE_PROCESS)
	tween.pause()
	for property in properties:
		tween.tween_property(target, str(property), values[property], seconds).set_trans(transition).set_ease(easing)
	await get_tree().create_timer(delay).timeout
	tween.play()
	if entering:
		await tween.finished
		entered.emit()
