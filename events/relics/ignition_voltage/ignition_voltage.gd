class_name IgnitionVoltage extends Relic

func _init():
	super(preload("uid://dqejgch5dthnb"))

func apply_effect() -> void:
	EnemyDebuffManager.burn_debuff.value += 1
