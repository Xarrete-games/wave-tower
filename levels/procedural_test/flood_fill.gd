class_name FloodFill extends RefCounted

static func can_escape_from(
	start: Vector2i,                # posición desde la que crecería el mapa
	grid: Dictionary,               # celdas ocupadas (cuerpo)
	grid_offsets: Dictionary        # direcciones posibles
) -> bool:

	var visited = {}                # celdas libres alcanzables
	var stack = [start]             # DFS desde la "cabeza"

	while stack.size() > 0:
		var current = stack.pop_back()

		# ya visitado → no repetir
		if visited.has(current):
			continue

		# marcamos celda como alcanzable
		visited[current] = true

		# exploramos vecinos
		for offset in grid_offsets.values():
			var next = current + offset

			# no podemos atravesar el cuerpo
			if grid.has(next):
				continue

			# ya explorado
			if visited.has(next):
				continue

			# 🔥 CRITERIO DE ESCAPE
			# si tengo más espacio libre accesible que cuerpo,
			# entonces puedo seguir creciendo sin encerrarme
			if visited.size() > grid.size():
				return true

			stack.append(next)

	# si agotamos el espacio accesible sin superar el tamaño del cuerpo,
	# estamos encerrados
	return false
