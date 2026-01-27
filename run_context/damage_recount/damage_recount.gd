class_name DamageRecount extends RefCounted

signal damage_recount_change(data: DamageRecountData)

var damage_per_wave: Dictionary[int, Dictionary] = {}
var tower_manager: TowersManager
var progress: RunProgress

func _init(p_tower_manager: TowersManager, p_progress: RunProgress) -> void:
	tower_manager = p_tower_manager
	progress = p_progress
	progress.current_level_changed.connect(func(_level_num: int) -> void:
		# Initialize damage recount for the new wave
		damage_per_wave.clear()
	)
	tower_manager.tower_placed.connect(func(tower: Tower) -> void:
		record_damage(
			RunContext.progress.current_wave,
			tower.id,
			0.0)
	)

func record_damage(wave: int, id: String, damage: float) -> void:
	if not damage_per_wave.has(wave):
		damage_per_wave[wave] = {}
	if not damage_per_wave[wave].has(id):
		damage_per_wave[wave][id] = DamageRecountData.new()
		damage_per_wave[wave][id].id = id
		damage_per_wave[wave][id].wave = wave
	damage_per_wave[wave][id].damage += damage
	damage_recount_change.emit(damage_per_wave[wave][id])
