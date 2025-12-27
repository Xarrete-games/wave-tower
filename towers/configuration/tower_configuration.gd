class_name TowerConfiguration extends Resource

@export_group("Definition")
@export var id: String
@export var display_name: String
@export_multiline var description: String
@export var icon: Texture2D
@export var type: Tower.Type

@export_group("Price")
@export var base_price: int = 50

@export_group("Initial Stats")
@export var base_damage: float = 5
@export var base_attack_range: float = 200
@export var base_attack_speed: float = 1.0
@export var base_critic_chance: float = 0
@export var base_critic_damage: float = 50

@export_group("Stats Per Level")
@export var damage_per_level: float = 0
@export var attack_range_level: float = 0
@export var attack_speed_per_level: float = 0.0
@export var critic_chance_per_level: float = 0
@export var critic_damage_per_level: float = 0

@export_group("Upgrades")
@export var upgradeable_towers: Array[TowerConfigurationWithInstance] = []

var stats: TowerStats = TowerStats.new()
var stats_on_level: TowerStats = TowerStats.new()


func build() -> void:
    stats.damage = base_damage
    stats.attack_range = base_attack_range
    stats.attack_speed = base_attack_speed
    stats.critic_chance = base_critic_chance
    stats.critic_damage = base_critic_damage

    stats_on_level.damage = damage_per_level
    stats_on_level.attack_range = attack_range_level
    stats_on_level.attack_speed = attack_speed_per_level
    stats_on_level.critic_chance = critic_chance_per_level
    stats_on_level.critic_damage = critic_damage_per_level
 