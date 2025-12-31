class_name LightningChainProjectile extends Node2D

var enemies_in_range: Array[Enemy] = []

@onready var thunder: Line2D = $VFX_thunder/Thunder

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_area_2d_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	enemies_in_range.erase(enemy)

func _on_area_2d_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemies_in_range.append(enemy)
	enemy.tree_exited.connect(
		func():
			enemies_in_range.erase(enemy)
	)
