using Godot;

[GlobalClass]
public partial class PassiveLight : PointLight2D
{
    [Export] public float velocidad = 6.0f;
    [Export] public float EscalaMin = 0.7f;
    [Export] public float EscalaMax = 1.0f;

    private float t = 0.0f;

    public override void _Process(double delta)
    {
        t += (float)delta * velocidad;
        float s = (Mathf.Sin(t) + 1.0f) * 0.5f;
        TextureScale = Mathf.Lerp(EscalaMin, EscalaMax, s);
    }
}
