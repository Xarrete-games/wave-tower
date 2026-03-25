class_name DebuffContext extends RefCounted

var debuff: EnemyDebuff
var stacks: int

func _init(p_debuff: EnemyDebuff, p_stacks: int):
	debuff = p_debuff
	stacks = p_stacks