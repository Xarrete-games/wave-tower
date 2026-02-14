class_name SpawnPositionsHandler
extends RefCounted

const ORANGE_PORTAL: PackedScene = preload("uid://b8g0wp8j02vu4")
const PORTAL_GROUP: String = "orange_portal"

var portals_container: Node2D = null
var portal_entries_map: Dictionary = {} # key -> {"pos": Vector2, "dir": Edge.Dir}
var portal_positions: Array[Vector2] = []
var portal_nodes: Dictionary = {} # key -> Node2D

func _init(portals_container_p: Node2D) -> void:
    portals_container = portals_container_p

func update(entries: Array[Dictionary]) -> void:
    # Build a keyed map for incoming entries
    var new_map: Dictionary = {}
    for e in entries:
        if typeof(e) != TYPE_DICTIONARY:
            continue
        # prefer explicit tile key if provided, otherwise tile, otherwise pos
        var key: String = ""
        if e.has("key"):
            key = str(e["key"])
        elif e.has("tile"):
            key = _tile_key(e["tile"])
        else:
            var pos: Vector2 = e.get("pos", null)
            if pos == null:
                continue
            key = _pos_key(pos)
        new_map[key] = e
    
    # purge any invalid nodes left in the map (in case they were freed externally)
    var to_purge: Array = []
    for key in portal_nodes.keys():
        var node = portal_nodes[key]
        if not is_instance_valid(node):
            to_purge.append(key)
    for k in to_purge:
        portal_nodes.erase(k)

    # Free visuals that are no longer present
    var keys_to_remove: Array = []
    for key in portal_nodes.keys():
        if not new_map.has(key):
            keys_to_remove.append(key)
    for key in keys_to_remove:
        var node = portal_nodes[key]
        if is_instance_valid(node):
            node.queue_free()
        portal_nodes.erase(key)
    # Create visuals for new entries
    var keys_created: Array = []
    for key in new_map.keys():
        var needs_create = true
        if portal_nodes.has(key):
            var existing = portal_nodes[key]
            if is_instance_valid(existing):
                needs_create = false
            else:
                portal_nodes.erase(key)
        if needs_create:
            keys_created.append(key)
            var e = new_map[key]
            var portal = ORANGE_PORTAL.instantiate()
            if portals_container:
                portals_container.add_child(portal)
            else:
                push_error("[SpawnPositionsHandler]: No container to add portals to!")
            portal.global_position = e["pos"]
            portal.add_to_group(PORTAL_GROUP)

            if e.has("dir") and (e["dir"] == Edge.Dir.NE or e["dir"] == Edge.Dir.SE):
                var sprite_node = portal.get_node_or_null("AnimatedSprite2D")
                if sprite_node and sprite_node is AnimatedSprite2D:
                    sprite_node.flip_h = true
                else:
                    for c in portal.get_children():
                        if c is AnimatedSprite2D:
                            c.flip_h = true
                            break

            portal_nodes[key] = portal
    
    # Replace entries map and rebuild positions list
    portal_entries_map = new_map
    portal_positions.clear()
    for k in portal_entries_map.keys():
        portal_positions.append(portal_entries_map[k]["pos"])

func get_positions() -> Array[Vector2]:
    return portal_positions.duplicate()

func get_entries() -> Array[Dictionary]:
    var arr: Array[Dictionary] = []
    for k in portal_entries_map.keys():
        arr.append(portal_entries_map[k])
    return arr

func clear_visuals() -> void:
    for key in portal_nodes.keys():
        var n = portal_nodes[key]
        if is_instance_valid(n):
            n.queue_free()
    portal_nodes.clear()

func _pos_key(pos: Vector2) -> String:
    return "%f,%f" % [pos.x, pos.y]

func _tile_key(tile: Vector2i) -> String:
    return "%d,%d" % [tile.x, tile.y]