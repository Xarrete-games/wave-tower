using Godot;
using System.Collections.Generic;

public static class FloodFill
{
    public static bool can_escape_from(
        Vector2I start,
        HashSet<Vector2I> occupied,
        IReadOnlyDictionary<int, Vector2I> gridOffsets)
    {
        var visited = new HashSet<Vector2I>();
        var stack = new Stack<Vector2I>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2I current = stack.Pop();

            if (!visited.Add(current))
            {
                continue;
            }

            foreach (Vector2I offset in gridOffsets.Values)
            {
                Vector2I next = current + offset;

                if (occupied.Contains(next))
                {
                    continue;
                }

                if (visited.Contains(next))
                {
                    continue;
                }

                if (visited.Count > occupied.Count)
                {
                    return true;
                }

                stack.Push(next);
            }
        }

        return false;
    }
}
