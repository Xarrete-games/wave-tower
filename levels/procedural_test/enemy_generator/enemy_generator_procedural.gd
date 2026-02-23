class_name EnemyGeneratorProcedural extends Node
## Orchestrates procedural wave generation.
##
## This node is intentionally thin — it delegates:
##  - Wave composition → WaveComposer (decides WHAT to spawn)
##  - Wave spawning    → WaveSpawner  (handles HOW to spawn)
##  - Wave scaling     → WaveConfig   (defines the difficulty curve)
##
## The only responsibility here is to wire input → composer → spawner
## and track the current wave number.

# ---------------------------------------------------------
# DEPENDENCIES (assign in the editor)
# ---------------------------------------------------------

## The WaveSpawner node that physically places enemies on the map.
@export var wave_spawner: WaveSpawner = null

## Optional: override the default wave scaling parameters.
@export var wave_config: WaveConfig = null

# ---------------------------------------------------------
# INTERNAL STATE
# ---------------------------------------------------------

var _wave_number: int = 0
var _composer: WaveComposer = null

# ---------------------------------------------------------
# LIFECYCLE
# ---------------------------------------------------------

func _ready() -> void:
	# Build the enemy catalog from the global DataLoader
	var enemy_catalog: Array[EnemyData] = DataLoader.enemy_data.get_all_enemies()

	# Fall back to a default config if none was provided in the inspector
	if wave_config == null:
		wave_config = WaveConfig.new()

	_composer = WaveComposer.new(wave_config, enemy_catalog)

	# Forward spawner signals for debugging / future UI hooks
	if wave_spawner:
		wave_spawner.wave_started.connect(_on_wave_started)
		wave_spawner.wave_finished.connect(_on_wave_finished)

# ---------------------------------------------------------
# INPUT  (temporary — will be replaced by proper UI later)
# ---------------------------------------------------------

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_accept"):
		await get_tree().create_timer(0.1).timeout
		start_next_wave()

# ---------------------------------------------------------
# PUBLIC API
# ---------------------------------------------------------

## Composes the next wave and hands it to the spawner.
func start_next_wave() -> void:
	if wave_spawner == null:
		push_warning("[EnemyGeneratorProcedural] No WaveSpawner assigned")
		return

	_wave_number += 1
	var groups: Array[WaveComposer.WaveGroup] = _composer.compose_wave(_wave_number)

	# Debug summary
	var total_enemies: int = 0
	for g in groups:
		total_enemies += g.enemies.size()
	var budget: int = wave_config.base_budget + (_wave_number - 1) * wave_config.budget_per_wave
	print("[Wave %d] Budget: %d | Groups: %d | Total enemies: %d" % [
		_wave_number, budget, groups.size(), total_enemies
	])

	wave_spawner.start_wave(
		_wave_number,
		groups,
		wave_config.spawn_interval,
		wave_config.group_delay
	)

# ---------------------------------------------------------
# SIGNAL CALLBACKS
# ---------------------------------------------------------

func _on_wave_started(wave_number: int) -> void:
	print("[EnemyGeneratorProcedural] Wave %d started" % wave_number)

func _on_wave_finished(wave_number: int) -> void:
	print("[EnemyGeneratorProcedural] Wave %d finished spawning" % wave_number)
