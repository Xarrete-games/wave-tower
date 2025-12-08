class_name ArticCube extends Relic

func _init():
	super(preload("uid://br0igrelenl0f"))

func apply_effect() -> void:
	EnemyDebuffManager.frost_debuff.value += 10

