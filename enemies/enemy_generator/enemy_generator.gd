class_name EnemyGenerator extends Node
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
# CONSTANTS
# ---------------------------------------------------------

## Placeholder — will be replaced by dynamic logic later.
const TOTAL_WAVES: int = 30

# ---------------------------------------------------------
# DEPENDENCIES (assign in the editor)
# ---------------------------------------------------------

## The WaveSpawner node that physically places enemies on the map.
@export var wave_spawner: WaveSpawner = null

## Optional: override the default wave scaling parameters.
@export var wave_config: WaveConfig = null

@export var enemies_container: Node2D

# ---------------------------------------------------------
# INTERNAL STATE
# ---------------------------------------------------------

var _wave_number: int = 0
var _composer: WaveComposer = null
var _enemies_left: int = 0

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

	# Wire RunContext progress
	RunContext.progress.total_waves = TOTAL_WAVES

	# Forward spawner signals
	if wave_spawner:
		wave_spawner.enemies_container = enemies_container
		wave_spawner.wave_started.connect(_on_wave_started)
		wave_spawner.wave_finished.connect(_on_wave_finished)
		wave_spawner.enemy_spawned.connect(_on_enemy_spawned)

	ClickEvents.next_wave_pressed.connect(start_next_wave)

# ---------------------------------------------------------
# PUBLIC API
# ---------------------------------------------------------

## Composes the next wave and hands it to the spawner.
func start_next_wave() -> void:
	if wave_spawner == null:
		push_warning("[EnemyGeneratorProcedural] No WaveSpawner assigned")
		return

	_wave_number += 1
	RunContext.progress.current_wave = _wave_number

	var groups: Array[WaveComposer.WaveGroup] = _composer.compose_wave(_wave_number)

	# Debug summary
	var total_enemies: int = 0
	for g in groups:
		total_enemies += g.enemies.size()
	var budget: int = _composer.get_budget_for_wave(_wave_number)
	print("[Wave %d] Budget: %d | Groups: %d | Total enemies: %d" % [
		_wave_number, budget, groups.size(), total_enemies
	])

	wave_spawner.start_wave(
		_wave_number,
		groups,
		wave_config
	)

# ---------------------------------------------------------
# SIGNAL CALLBACKS
# ---------------------------------------------------------

func _on_wave_started(wave_number: int) -> void:
	print("[EnemyGeneratorProcedural] Wave %d started" % wave_number)


## Called when the spawner has placed the LAST enemy of the wave.
## From this point we start tracking how many enemies remain alive.
func _on_wave_finished(wave_number: int) -> void:
	print("[EnemyGeneratorProcedural] Wave %d finished spawning" % wave_number)
	_enemies_left = get_tree().get_nodes_in_group("enemy").size()
	if _enemies_left == 0:
		_report_finished()
		return
	enemies_container.child_exiting_tree.connect(_on_enemy_left)


## Called each time the spawner creates an enemy — wire up its signals.
func _on_enemy_spawned(enemy: Enemy) -> void:
	enemy.die.connect(_on_enemy_die)
	enemy.target_reached.connect(_on_enemy_target_reached)


## When an enemy is eliminated (death or reaching the end),
## check if there are more enemies left to finish the wave.
func _on_enemy_left(node: Node) -> void:
	if node.is_in_group("enemy"):
		_enemies_left -= 1
	if _enemies_left <= 0:
		if enemies_container.child_exiting_tree.is_connected(_on_enemy_left):
			enemies_container.child_exiting_tree.disconnect(_on_enemy_left)
		# Defer to avoid tree-lock: child_exiting_tree fires while the tree is busy
		_report_finished.call_deferred()


## Init the next wave or end the level if it's the last wave.
func _report_finished() -> void:
	if RunContext.is_on_restarting or RunContext.status.health <= 0 or GameState.is_on_main_menu():
		return

	if _wave_number >= TOTAL_WAVES:
		RunContext.progress.last_wave_finished.emit()
	else:
		Hooks.on_wave_finished()
		RunContext.progress.current_wave_finished.emit()


func _on_enemy_target_reached(enemy: Enemy) -> void:
	RunContext.status.apply_damage(enemy.damage)
	RunContext.enemy_manager.enemy_target_reached.emit(enemy)


func _on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	RunContext.enemy_manager.enemy_die.emit(enemy, attack)
