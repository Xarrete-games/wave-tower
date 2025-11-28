# score.gd
extends Node

signal gold_change(amount: int)
signal extra_gold_dropped_change(amount: int)

var gold: int = 100:
	set(value):
		gold = value
		gold_change.emit(value)

var extra_gold_dropped: int = 0:
	set(value):
		extra_gold_dropped = value
		extra_gold_dropped_change.emit(extra_gold_dropped)

func _ready() -> void:
	EnemyManager.wave_finished.connect(func(wave: EnemyWave):
		add_gold(wave.gold_when_ends)
	)
	EnemyManager.last_wave_finished.connect(func(wave: EnemyWave):
		add_gold(wave.gold_when_ends)
	)
	
func add_gold(amount: int) -> void:
	gold += amount
	AudioManager.play_coins()
	gold_change.emit(gold)
	
func substract_gold(amount: int) -> void:
	gold -= amount
	gold_change.emit(gold)
