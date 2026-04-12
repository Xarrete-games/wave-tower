using Godot;

[GlobalClass]
public partial class Portal : AnimatedSprite2D
{
    public override void _Ready()
    {
        Play("default");
    }
}
