class_name SpawnPositionsHandler
extends RefCounted

const ORANGE_PORTAL: PackedScene = preload("uid://b8g0wp8j02vu4")
const PORTAL_GROUP: String = "orange_portal"

var portals_container: Node2D = null
var portal_entries: Array[Dictionary] = [] # {"pos": Vector2, "dir": MapPiece.Dir}
var portal_positions: Array[Vector2] = []
var portal_nodes: Array[Node2D] = []

func _init(portals_container_p: Node2D) -> void:
    portals_container = portals_container_p

func update(entries: Array[Dictionary]) -> void:
    # remove previous visuals
    for n in portal_nodes:
        if is_instance_valid(n):
            n.queue_free()
    portal_nodes.clear()

    # copy entries into typed array
    portal_entries.clear()
    portal_positions.clear()
    for e in entries:
        if typeof(e) != TYPE_DICTIONARY:
            continue
        portal_entries.append(e)
        portal_positions.append(e["pos"])
        var portal = ORANGE_PORTAL.instantiate()
        if portals_container:
            portals_container.add_child(portal)
        else:
           push_error("[SpawnPositionsHandler] Portals container is null, cannot add portal instance")
        portal.global_position = e["pos"]
        portal.add_to_group(PORTAL_GROUP)

        if e["dir"] == MapPiece.Dir.NE or e["dir"] == MapPiece.Dir.SE:
            var sprite_node = portal.get_node_or_null("AnimatedSprite2D")
            if sprite_node and sprite_node is AnimatedSprite2D:
                sprite_node.flip_h = true
            else:
                for c in portal.get_children():
                    if c is AnimatedSprite2D:
                        c.flip_h = true
                        break

        portal_nodes.append(portal)

func get_positions() -> Array[Vector2]:
    return portal_positions.duplicate()

func get_entries() -> Array[Dictionary]:
    return portal_entries.duplicate()

func clear_visuals() -> void:
    for n in portal_nodes:
        if is_instance_valid(n):
            n.queue_free()
    portal_nodes.clear()
