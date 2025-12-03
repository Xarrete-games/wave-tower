class_name EnemyGroup extends Resource

enum PATH { PATH1, PATH2, BOTH }


@export var enemy_type: Enemy.EnemyType =  Enemy.EnemyType.NORMAL
@export var amount: int = 5
@export var interval_spawn: float = 0.5
@export var time_to_start: float = 0
@export var paths: Array[int] = [0]
