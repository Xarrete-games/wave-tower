#EnemyGenerator.gd
extends Node

signal enemy_die(enemy: Enemy)
const ENEMY = preload("uid://diax8n45uy1cu")
var enemy_timer: Timer
var _level: Level
var count = 0

func generate_enemies(level: Level) -> void:
	_level = level
	enemy_timer = Timer.new()
	enemy_timer.wait_time = 3
	enemy_timer.one_shot = false
	enemy_timer.timeout.connect(_on_enemy_timer)
	level.add_child(enemy_timer)
	enemy_timer.start()
	
func _on_enemy_timer() -> void:
	# 1. Instanciar el enemigo
	var new_enemy: Enemy = ENEMY.instantiate()
	new_enemy.die.connect(_on_enemy_die)
	get_tree().root.add_child(new_enemy)
	
	new_enemy.set_path_follow(_level.get_new_path_follow())
	count += 1

func _on_enemy_die(enemy: Enemy) -> void:
	enemy_die.emit(enemy)
	
