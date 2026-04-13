using Godot;

public static class FloodFill
{
    public static bool can_escape_from(
        Vector2I start,
        Godot.Collections.Dictionary grid,
        Godot.Collections.Dictionary grid_offsets)
    {
        var visited = new Godot.Collections.Dictionary();
        var stack = new Godot.Collections.Array<Vector2I> { start };

        while (stack.Count > 0)
        {
            Vector2I current = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (visited.ContainsKey(current))
            {
                continue;
            }

            visited[current] = true;

            foreach (Variant value in grid_offsets.Values)
            {
                Vector2I offset = value.AsVector2I();
                Vector2I next = current + offset;

                if (grid.ContainsKey(next))
                {
                    continue;
                }

                if (visited.ContainsKey(next))
                {
                    continue;
                }

                if (visited.Count > grid.Count)
                {
                    return true;
                }

                stack.Add(next);
            }
        }

        return false;
    }
}
