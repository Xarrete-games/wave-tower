class_name Enemy extends CharacterBody2D

signal die(ememy: Enemy)
signal target_reached(enemy: Enemy)

enum  EnemyType { NORMAL, BUBA, TANK, GOLEM, SKELETON, BLACK_GOLEM, BLACK_SKELETON }

const GOLD_DROPPED = preload("uid://cxs4ar5enx4mn")
const DAMAGE_NUMBERS = preload("uid://bkiu4qgh3ug1m")

@export var base_speed: float = 80
@export var max_healt: float = 20
@export var base_gold_value: int = 1
@export var damage: int = 1

var health: float
var hit_tween: Tween
var gold_value: int
var current_debuffs: Dictionary[EnemyDebuff.DebuffType, EnemyDebuff] = {}
var current_debuffs_timers: Dictionary[EnemyDebuff.DebuffType, Timer] = {}
var _path_follow: PathFollow2D
var _default_modulate_color: Color = Color.WHITE
var _speed = base_speed
var _last_is_right_direction: bool = false

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var explosion: AnimatedSprite2D = $Explosion
@onready var health_bar: HealthBar = $HealthBar
@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var numbers_displayed_pos: Marker2D = $NumbersDisplayedPos
@onready var gold_dropped_pos: Marker2D = $GoldDroppedPos
# debuffs timers
@onready var poison_damage_timer: Timer = $PoisonDamageTimer
@onready var burn_damage_timer: Timer = $BurnDamageTimer

# percentage of remaining heal
func get_remaining_heal() -> float:
	if max_healt <= 0:
		return 0.0
	var health_ratio: float = health / max_healt
	var percentage: float = health_ratio * 100.0
	
	return min(100.0, percentage)

func get_damage(attack: Attack) -> void:
	_handle_debuffs(attack.debuffs)
	_set_health(health - attack.damage)
	_play_hit_animation()
	_show_damage(attack)

	if health <= 0:
		_die()
		
func set_path_follow(path_follow: PathFollow2D) -> void:
	_path_follow = path_follow

func _ready() -> void:
	_speed = base_speed
	health_bar.set_max_health(max_healt)
	_set_health(max_healt)
	gold_value = base_gold_value + Score.extra_gold_dropped
	Score.extra_gold_dropped_change.connect(
		func(value): gold_value = base_gold_value + value)

func _process(delta):
	if _path_follow == null:
		return
	# save previous position	
	var previous_global_x = global_position.x
	var previous_global_y = global_position.y
	#increse progress
	_path_follow.progress += _speed * delta
	
	# target reached
	if _path_follow.progress_ratio >= 0.99:
		_on_target_reached()
		return
	
	var path_global_pos = _path_follow.global_position
	
	global_position = path_global_pos
	
	# FLIP SPRITE
	var is_right_direction = global_position.x > previous_global_x
	if is_right_direction != _last_is_right_direction:
		animated_sprite_2d.flip_h = !animated_sprite_2d.flip_h
		_last_is_right_direction = is_right_direction
	
	# handle animation
	var animation = "top right" if previous_global_y > global_position.y else "down right"
	if animated_sprite_2d.animation != animation:
		animated_sprite_2d.play(animation)

# --------------------
# --- HEALT ---
# --------------------
func _play_hit_animation() -> void:
	if hit_tween and hit_tween.is_running():
		hit_tween.kill()

	hit_tween = create_tween()
	hit_tween.set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT)

	animated_sprite_2d.modulate = Color.RED
	hit_tween.tween_property(animated_sprite_2d, "modulate", _default_modulate_color, 0.2)


func _die() -> void:
	die.emit(self)
	_show_gold_dropped()
	Score.add_gold(gold_value)
	_path_follow.queue_free()
	queue_free()

func _show_damage(attack: Attack) -> void:
	var damage_numbers: DamageNumbers = DAMAGE_NUMBERS.instantiate()
	get_tree().root.add_child(damage_numbers)
	damage_numbers.set_attack(attack)
	damage_numbers.global_position = numbers_displayed_pos.global_position

func _show_gold_dropped() -> void:
	var gold_droped: GoldDropped = GOLD_DROPPED.instantiate()
	get_tree().root.add_child(gold_droped)
	gold_droped.set_gold(gold_value)
	gold_droped.global_position = gold_dropped_pos.global_position

func _on_target_reached() -> void:
	target_reached.emit(self)
	queue_free()
	
func _set_health(new_value: float) -> void:
	health = new_value
	health_bar.update_health(health)
	
# --------------------
# --- DEBUFFS ---
# --------------------
func _handle_debuffs(debuffs: Array[EnemyDebuff]) -> void:
	for debuff in debuffs:
		# ignore debuffs with out value
		if debuff.value == 0 or debuff.duration == 0:
			continue
		
		# if exist refreshing the debuff
		if current_debuffs.has(debuff.type):
			current_debuffs[debuff.type] = debuff
			current_debuffs_timers[debuff.type].wait_time = debuff.duration
			current_debuffs_timers[debuff.type].start()
			
		# else add it
		else:
			# create timer and scene
			var debuff_timer = Timer.new()
			add_child(debuff_timer)
			debuff_timer.one_shot = true
			debuff_timer.wait_time = debuff.duration
			debuff_timer.timeout.connect(_on_debuff_timer_end.bind(debuff.type))
			debuff_timer.start()
			# save data
			current_debuffs[debuff.type] = debuff
			current_debuffs_timers[debuff.type] = debuff_timer
			handle_new_debuff(debuff.type)
			
func _on_debuff_timer_end(debuff_type: EnemyDebuff.DebuffType) -> void:
	current_debuffs.erase(debuff_type)
	current_debuffs_timers[debuff_type].queue_free()
	current_debuffs_timers.erase(debuff_type)
	handle_debuff_end(debuff_type)

func handle_new_debuff(debuff_type: EnemyDebuff.DebuffType) -> void:
	match(debuff_type):
		# reduce attack speed and change color
		EnemyDebuff.DebuffType.FROST:
			var reduction_factor: float = current_debuffs[EnemyDebuff.DebuffType.FROST].value
			_speed = base_speed * (1.0 - reduction_factor)
			_default_modulate_color = Color.AQUA
			animated_sprite_2d.modulate = Color.AQUA
		EnemyDebuff.DebuffType.POISON:
			poison_damage_timer.start()
			_default_modulate_color = Color.CHARTREUSE
			animated_sprite_2d.modulate = Color.CHARTREUSE
		EnemyDebuff.DebuffType.BURN:
			burn_damage_timer.start()
			_default_modulate_color = Color.DARK_ORANGE
			animated_sprite_2d.modulate = Color.DARK_ORANGE

func handle_debuff_end(debuff_type: EnemyDebuff.DebuffType) -> void:
	match(debuff_type):
		# recover speed and color
		EnemyDebuff.DebuffType.FROST: 
			_speed = base_speed
		EnemyDebuff.DebuffType.POISON: 
			poison_damage_timer.stop()
		EnemyDebuff.DebuffType.BURN: 
			burn_damage_timer.stop()
	
	if current_debuffs.size() == 0:
				_default_modulate_color = Color.WHITE
				animated_sprite_2d.modulate = Color.WHITE

func _on_poison_damage_timer_timeout() -> void:
	var poison_damage = current_debuffs[EnemyDebuff.DebuffType.POISON].value
	var poison_attack = Attack.new(poison_damage, false, [])
	_default_modulate_color = Color.CHARTREUSE
	animated_sprite_2d.modulate = Color.CHARTREUSE
	get_damage(poison_attack)
	
func _on_burn_damage_timer_timeout() -> void:
	var burn_damage = current_debuffs[EnemyDebuff.DebuffType.BURN].value
	var burn_attack = Attack.new(burn_damage, false, [])
	_default_modulate_color = Color.DARK_ORANGE
	animated_sprite_2d.modulate = Color.DARK_ORANGE
	get_damage(burn_attack)
