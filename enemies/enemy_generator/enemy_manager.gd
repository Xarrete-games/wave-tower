#EnemyGenerator 
extends Node

@warning_ignore("unused_signal")
signal enemy_die(enemy: Enemy)
@warning_ignore("unused_signal")
signal new_level_loaded(total_waves: int)
@warning_ignore("unused_signal")
signal last_wave_finished(wave: EnemyWave)
@warning_ignore("unused_signal")
signal wave_finished(wave: EnemyWave)
@warning_ignore("unused_signal")
signal wave_init(num: int)

const ENEMY_NORMAL = preload("uid://dmqbn2q5splor")
const ENEMY_BUBA = preload("uid://xk0wj86s8ddb")
const ENEMY_TANK = preload("uid://dvri0e4k4qwho")
const ENEMY_GOLEM = preload("uid://cdb5n1d4ubx72")
const ENEMY_SKELETON = preload("uid://bnpwdbi54cn00")
const BOSS_BLACK_GOLEM = preload("uid://bbwwsea7icfed")
const ENEMY_BLACK_SKELETON = preload("uid://d1p6gdwregh7v")
const BOSS_GOLD_SKELETON = preload("uid://qsxiwo0d27dq")
const ENEMY_INVOKER = preload("uid://crk2ly48vsxn4")
const ENEMY_SKULL = preload("uid://daxk1aepm2gdi")


const ENEMIES_SCENES: Dictionary[Enemy.EnemyType, PackedScene] = {
	Enemy.EnemyType.NORMAL: ENEMY_NORMAL,
	Enemy.EnemyType.BUBA: ENEMY_BUBA,
	Enemy.EnemyType.TANK: ENEMY_TANK,
	Enemy.EnemyType.GOLEM: ENEMY_GOLEM,
	Enemy.EnemyType.SKELETON: ENEMY_SKELETON,
	Enemy.EnemyType.BLACK_GOLEM: BOSS_BLACK_GOLEM,
	Enemy.EnemyType.BLACK_SKELETON: ENEMY_BLACK_SKELETON,
	Enemy.EnemyType.GOLD_SKELETON: BOSS_GOLD_SKELETON,
	Enemy.EnemyType.INVOKER: ENEMY_INVOKER,
	Enemy.EnemyType.SKULL: ENEMY_SKULL
}

func get_enemy_scene(enemy_type: Enemy.EnemyType) -> PackedScene:
	return ENEMIES_SCENES[enemy_type]
