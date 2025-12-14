class_name EnemyDataLoader extends RefCounted


const ENEMY_NORMAL = preload("uid://dmqbn2q5splor")
const ENEMY_BUBA = preload("uid://xk0wj86s8ddb")
const ENEMY_TANK = preload("uid://dvri0e4k4qwho")
const ENEMY_GOLEM = preload("uid://cdb5n1d4ubx72")
const ENEMY_SKELETON = preload("uid://bnpwdbi54cn00")
const BOSS_BLACK_GOLEM = preload("uid://bbwwsea7icfed")
const ENEMY_BLACK_SKELETON = preload("uid://d1p6gdwregh7v")
const BOSS_GOLD_SKELETON = preload("uid://qsxiwo0d27dq")
const ENEMY_INVOKER = preload("uid://crk2ly48vsxn4")

const ENEMIES_SCENES: Dictionary[Enemy.Type, PackedScene] = {
	Enemy.Type.NORMAL: ENEMY_NORMAL,
	Enemy.Type.BUBA: ENEMY_BUBA,
	Enemy.Type.TANK: ENEMY_TANK,
	Enemy.Type.GOLEM: ENEMY_GOLEM,
	Enemy.Type.SKELETON: ENEMY_SKELETON,
	Enemy.Type.BLACK_GOLEM: BOSS_BLACK_GOLEM,
	Enemy.Type.BLACK_SKELETON: ENEMY_BLACK_SKELETON,
	Enemy.Type.GOLD_SKELETON: BOSS_GOLD_SKELETON,
	Enemy.Type.INVOKER: ENEMY_INVOKER
}

func get_enemy_scene(enemy_type: Enemy.Type) -> PackedScene:
	return ENEMIES_SCENES[enemy_type]