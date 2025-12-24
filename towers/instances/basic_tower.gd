@abstract
class_name BasicTower extends Tower

var level: int = 1


func upgrade() -> void:
	level += 1
	experience_handler.level_up.emit(level)




