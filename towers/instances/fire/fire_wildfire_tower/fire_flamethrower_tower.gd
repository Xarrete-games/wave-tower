class_name FireFlamethrowerTower extends Tower

@onready var flame_thrower_duration_timer: Timer = %FlameThrowerDurationTimer
@onready var fire_flamethrower_projectile: FireFlamethrowerProjectile = %FireFlamethrowerProjectile

func _ready() -> void:
	super._ready()
	on_target_change.connect(_on_new_target_change)
	flame_thrower_duration_timer.timeout.connect(_on_flame_thrower_duration_timer_timeout)

func _fire() -> void:
	fire_flamethrower_projectile.fire()
	var attack: Attack = _get_attack()
	fire_flamethrower_projectile.set_target(_current_target, attack)
	flame_thrower_duration_timer.start()

func _on_new_target_change(enemy: Enemy) -> void:
	if fire_flamethrower_projectile.is_throwing():
		if enemy == null:
			fire_flamethrower_projectile.stop()
		else:
			var attack: Attack = _get_attack()
			fire_flamethrower_projectile.set_target(enemy, attack)


func _on_flame_thrower_duration_timer_timeout() -> void:
	fire_flamethrower_projectile.stop()
