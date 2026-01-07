class_name LightningOverchargeTower extends Tower

@export_group("Scenes")
@export var projectile_scene: PackedScene
@export var overcharge_particle_scene: PackedScene


var towers_in_range: Array[Tower] = []
var buff = TowerBuff.new(TowerBuff.SourceType.TOWER, name, DamageMultModifier.new(0.1))
var particles_dict: Dictionary[String, Node] = {}

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos
@onready var buff_area_shape: CollisionShape2D = $BuffArea/BuffAreaShape
@onready var buff_area: Area2D = $BuffArea

func _fire() -> void:
	var projectile = projectile_scene.instantiate() as SingleTargetProjectile
	get_parent().add_child(projectile)

	projectile.global_position = projectile_spawn_pos.global_position
	projectile.set_target(_current_target, _get_attack())

func placement_mode() -> void:
	super.placement_mode()
	buff_area.monitoring = false

func enable() -> void:
	super.enable()
	buff_area.monitoring = true
	RunContext.towers_manager.tower_placed.connect(_on_tower_placed)
	
func _on_tower_placed(tower: Tower) -> void:
	if tower == self:
		return
	buff_area.monitoring = false
	await get_tree().process_frame
	buff_area.monitoring = true

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass

func _apply_stats_changes() -> void:
	super._apply_stats_changes()
	(buff_area_shape.shape as CircleShape2D).radius = stats.attack_range

func _on_buff_area_area_entered(area: Area2D) -> void:
	var tower = area.get_parent() as Tower
	_apply_buff(tower)

func _on_buff_area_area_exited(area: Area2D) -> void:
	var tower = area.get_parent() as Tower
	_remove_buff(tower)
	
func _apply_buff(tower: Tower) -> void:
	if tower in towers_in_range or tower == self:
		return
	
	var buff_particle = overcharge_particle_scene.instantiate()
	tower.add_local_buff(buff)
	towers_in_range.append(tower)
	tower.add_child(buff_particle)
	particles_dict[tower.name] = buff_particle

func _remove_buff(tower: Tower) -> void:
	if tower not in towers_in_range:
		return
	
	tower.remove_local_buff(buff)
	towers_in_range.erase(tower)
	
	var particle = particles_dict.get(tower.name, null)
	if particle != null:
		particle.queue_free()
		particles_dict.erase(tower.name)
