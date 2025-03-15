extends Node


func _ready() -> void:
	if !OS.is_debug_build():
		get_parent().remove_child(self)
		queue_free()
