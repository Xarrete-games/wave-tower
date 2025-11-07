@tool
class_name RedTower extends Tower

@onready var red_projectil: RedProjectil = $RedProjectil

func _ready():
	super._ready()
	
func _process(_delta: float) -> void:
	if not _current_target:
		red_projectil.stop()
		return
	red_projectil.set_target(_current_target)

func _fire() -> void:
	red_projectil.hit_target()
