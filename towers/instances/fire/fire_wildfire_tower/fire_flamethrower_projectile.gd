

class_name FireFlamethrowerProjectile extends Node2D


const DAMAGE_TICK_INTERVAL: float = 0.5

var _target: Enemy = null
var _attack: Attack = null
var _tagers_in_area: Array[Enemy] = []

@onready var fire_particles: CPUParticles2D = %FireParticles
@onready var damage_timer: Timer = %DamageTimer
@onready var area_2d: Area2D = %Area2D
@onready var flamethrower: Node2D = %Flamethrower

func _ready() -> void:
	damage_timer.wait_time = DAMAGE_TICK_INTERVAL
	stop()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	if _target == null or not is_instance_valid(_target):
		return

	var dir: Vector2 = _target.target_position - global_position

	# Rotamos el emisor
	flamethrower.rotation = dir.angle()


func fire() -> void:
	damage_timer.start()
	fire_particles.emitting = true
	area_2d.monitoring = true

func set_target(enemy: Enemy, attack: Attack) -> void:
	_target = enemy
	_attack = attack

func stop() -> void:
	fire_particles.emitting = false
	area_2d.monitoring = false
	damage_timer.stop()
	_tagers_in_area.clear()

func is_throwing() -> bool:
	return fire_particles.emitting

func _on_area_2d_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	_tagers_in_area.erase(enemy)

func _on_area_2d_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.tree_exited.connect(
		func():
			_tagers_in_area.erase(enemy)
	)
	_tagers_in_area.append(enemy)

func _on_damage_timer_timeout() -> void:
	for target: Enemy in _tagers_in_area:
		if is_instance_valid(target):
			var debuff = RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.BURN)
			target.apply_damage(_attack)
			target.apply_debuff(debuff)
