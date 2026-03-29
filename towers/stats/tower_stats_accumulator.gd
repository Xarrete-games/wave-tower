class_name TowerStatsAccumulator extends RefCounted

# commons
# damage
var flat_damage: float = 0.0
var damage_mult: float = 0.0
# range
var flat_attack_range: float = 0.0
var attack_range_mult: float = 0.0
var attack_range_mult_frost: float = 0.0
# attack speed
var flat_attack_speed: float = 0.0
var attack_speed_mult: float = 0.0
var attack_speed_mult_lightning: float = 0.0
# critic
var flat_critic_chance: float = 0.0
var critic_chance_mult: float = 0.0
# critic damage
var flat_critic_damage: float = 0.0
var critic_damage_mult: float = 0.0

# laser tower
var flat_execute_threshold: float = 0.0
# fire tower
var all_fire_apply_burn: bool = false

# nova tower
var flat_double_shot_chance: float = 0.0

func merge(other: TowerStatsAccumulator) -> TowerStatsAccumulator:
    var result :TowerStatsAccumulator = TowerStatsAccumulator.new()
    # commons
    # damage
    result.flat_damage = flat_damage + other.flat_damage
    result.damage_mult = damage_mult + other.damage_mult
    
    # range
    result.flat_attack_range = flat_attack_range + other.flat_attack_range
    result.attack_range_mult = attack_range_mult + other.attack_range_mult
    result.attack_range_mult_frost = attack_range_mult_frost + other.attack_range_mult_frost

    # attack speed  
    result.flat_attack_speed = flat_attack_speed + other.flat_attack_speed
    result.attack_speed_mult = attack_speed_mult + other.attack_speed_mult
    result.attack_speed_mult_lightning = attack_speed_mult_lightning + other.attack_speed_mult_lightning
    
    # critic
    result.flat_critic_chance = flat_critic_chance + other.flat_critic_chance
    result.critic_chance_mult = critic_chance_mult + other.critic_chance_mult

    # critic damage
    result.flat_critic_damage = flat_critic_damage + other.flat_critic_damage
    result.critic_damage_mult = critic_damage_mult + other.critic_damage_mult

    # fire
    result.flat_execute_threshold = flat_execute_threshold + other.flat_execute_threshold
    result.all_fire_apply_burn = all_fire_apply_burn or other.all_fire_apply_burn

    # frost
    result.flat_double_shot_chance = flat_double_shot_chance + other.flat_double_shot_chance
    

    return result