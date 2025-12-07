# EnemyDebuffManager
extends Node

signal debuff_change(enemy_debuff: EnemyDebuff)


var burn_debuff: BurnDebuff
var frost_debuff: FrostDebuff

func _ready() -> void:
	reset()
	_bind_signals()

func get_debuff(type: EnemyDebuff.Type) -> EnemyDebuff:
	match type:
		EnemyDebuff.Type.BURN:
			return burn_debuff.clone()
		EnemyDebuff.Type.FROST:
			return frost_debuff.clone()
	
	push_error("[EnemyDebuffManager] invalid get debuff")
	return null

func reset() -> void:
	frost_debuff = FrostDebuff.new()
	burn_debuff = BurnDebuff.new()
	
func _bind_signals():
	burn_debuff.changed.connect(
		func(): 
			debuff_change.emit(burn_debuff))
	frost_debuff.changed.connect(func(): debuff_change.emit(frost_debuff))
