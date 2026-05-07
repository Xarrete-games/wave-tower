using Godot;
[GlobalClass]
public partial class RangePreview : Node2D
{
    [Export]
    public float radius
    {
        get => _radius;
        set
        {
            _radius = value;
            QueueRedraw();
        }
    }

    [Export] public float IsoScaleY = 0.5f;
    [Export] public Color ColorFill = new(1f, 1f, 1f, 0.3f);
    [Export] public Color ColorBorder = new(1f, 1f, 1f, 0.7f);
    [Export] public float LineWidth = 5f;
    [Export] public int segments = 64;

    private float _radius = 100.0f;

    public override void _Draw()
    {
        var points = new Vector2[segments];
        for (var i = 0; i < segments; i++)
        {
            var angle = Mathf.Tau * i / (float)segments;
            points[i] = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * IsoScaleY
            );
        }

        DrawColoredPolygon(points, ColorFill);

        for (var i = 0; i < segments; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % segments];
            DrawLine(a, b, ColorBorder, LineWidth);
        }
    }
}
