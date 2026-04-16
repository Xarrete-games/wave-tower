using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class AnimationComponent : Node
{
    public event Action entered;

    private const Tween.TransitionType IMMEDIATE_TRANSITION = Tween.TransitionType.Linear;

    [ExportGroup("Options")]
    [Export] public bool from_center = true;
    [Export] public bool parallel_animations = true;
    [Export] public bool enter_animation = false;
    [Export] public Array<string> properties = new() { "scale", "position", "rotation", "size", "self_modulate" };
    [Export] public bool flicked = false;

    [ExportGroup("Hover Settings")]
    [Export] public float hover_time = 0.2f;
    [Export] public float hover_delay = 0.0f;
    [Export] public Tween.TransitionType hover_transition = Tween.TransitionType.Linear;
    [Export] public Tween.EaseType hover_easing = Tween.EaseType.InOut;
    [Export] public Vector2 hover_scale = Vector2.One;
    [Export] public Vector2 hover_position = Vector2.Zero;
    [Export] public float hover_rotation = 0.0f;
    [Export] public Vector2 hover_size = Vector2.One;
    [Export] public Color hover_modulate = Colors.White;
    [Export] public bool play_hover_sound = false;

    [ExportGroup("Enter Settings")]
    [Export] public AnimationComponent wait_for;
    [Export] public float enter_time = 0.2f;
    [Export] public float enter_delay = 0.0f;
    [Export] public Tween.TransitionType enter_transition = Tween.TransitionType.Linear;
    [Export] public Tween.EaseType enter_easing = Tween.EaseType.InOut;
    [Export] public Vector2 enter_scale = Vector2.One;
    [Export] public Vector2 enter_position = Vector2.Zero;
    [Export] public float enter_rotation = 0.0f;
    [Export] public Vector2 enter_size = Vector2.One;
    [Export] public Color enter_modulate = Colors.White;

    [ExportGroup("Flicked Settings")]
    [Export] public float flicked_time = 0.1f;
    [Export] public Color flicked_color = new(1, 1, 1, 0.5f);

    private Control target;
    private Vector2 default_scale;
    private Dictionary hover_values = new();
    private Dictionary enter_values = new();
    private Dictionary default_values = new();
    private bool on_hover = false;

    public override void _Ready()
    {
        this.target = GetParent() as Control;
        CallDeferred(nameof(setup));
    }

    public void on_hover_entered()
    {
        this.on_hover = true;
        _ = add_tween(this.hover_values, this.parallel_animations, this.hover_time, this.hover_delay, this.hover_transition, this.hover_easing);
        if (this.play_hover_sound)
        {
            AudioManager audioManager = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
            audioManager?.play_button_hover();
        }
    }

    public void on_hover_exited()
    {
        this.on_hover = false;
        _ = add_tween(this.default_values, this.parallel_animations, this.hover_time, this.hover_delay, this.hover_transition, this.hover_easing);
    }

    public void on_entered_action()
    {
        _ = add_tween(this.default_values, this.parallel_animations, this.enter_time, this.enter_delay, this.enter_transition, this.enter_easing, true);
    }

    public void connect_signals()
    {
        this.target.MouseEntered += this.on_hover_entered;
        this.target.MouseExited += this.on_hover_exited;

        if (this.wait_for != null)
        {
            this.wait_for.entered += this.on_entered_action;
        }
    }

    public async void setup()
    {
        if (this.target == null)
        {
            return;
        }

        if (this.hover_position == Vector2.Zero && this.enter_position == Vector2.Zero)
        {
            var filtered = new Array<string>();
            for (int i = 0; i < this.properties.Count; i++)
            {
                if (this.properties[i] != "position")
                {
                    filtered.Add(this.properties[i]);
                }
            }
            this.properties = filtered;
        }

        if (this.from_center)
        {
            this.target.PivotOffset = this.target.Size / 2.0f;
        }

        this.default_scale = this.target.Scale;
        this.default_values = new Dictionary
        {
            { "scale", this.target.Scale },
            { "position", this.target.Position },
            { "rotation", this.target.Rotation },
            { "size", this.target.Size },
            { "self_modulate", this.target.SelfModulate },
        };

        this.hover_values = new Dictionary
        {
            { "scale", this.hover_scale },
            { "position", this.target.Position + this.hover_position },
            { "rotation", this.target.Rotation + Mathf.DegToRad(this.hover_rotation) },
            { "size", this.target.Size * this.hover_size },
            { "self_modulate", this.hover_modulate },
        };

        this.enter_values = new Dictionary
        {
            { "scale", this.enter_scale },
            { "position", this.target.Position + this.enter_position },
            { "rotation", this.target.Rotation + Mathf.DegToRad(this.enter_rotation) },
            { "size", this.target.Size * this.enter_size },
            { "self_modulate", this.enter_modulate },
        };

        this.connect_signals();

        if (this.flicked)
        {
            _ = flick_loop();
        }

        if (this.enter_animation)
        {
            this.on_enter();
        }
        else
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            this.entered?.Invoke();
        }
    }

    public void on_enter()
    {
        _ = add_tween(this.enter_values, true, 0.0f, 0.0f, IMMEDIATE_TRANSITION, Tween.EaseType.In);

        if (this.wait_for == null)
        {
            this.on_entered_action();
        }
    }

    public async System.Threading.Tasks.Task add_tween(Dictionary values, bool parallel, float seconds, float delay, Tween.TransitionType transition, Tween.EaseType easing, bool entering = false)
    {
        if (!IsInsideTree() || this.target == null)
        {
            return;
        }

        Tween tween = GetTree().CreateTween();
        tween.SetParallel(parallel);
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.Pause();

        for (int i = 0; i < this.properties.Count; i++)
        {
            string property = this.properties[i];
            Variant value = values.ContainsKey(property) ? values[property] : default;
            tween.TweenProperty(this.target, property, value, seconds).SetTrans(transition).SetEase(easing);
        }

        await ToSignal(GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
        tween.Play();

        if (entering)
        {
            await ToSignal(tween, Tween.SignalName.Finished);
            this.entered?.Invoke();
        }
    }

    public async System.Threading.Tasks.Task flick_loop()
    {
        if (this.default_values.Count == 0)
        {
            return;
        }

        Color defaultModulate = this.default_values["self_modulate"].AsColor();
        bool useFlick = true;

        while (this.flicked && IsInsideTree())
        {
            if (this.on_hover)
            {
                this.target.SelfModulate = defaultModulate;
                await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                continue;
            }

            this.target.SelfModulate = useFlick ? this.flicked_color : defaultModulate;
            useFlick = !useFlick;
            await ToSignal(GetTree().CreateTimer(this.flicked_time), Timer.SignalName.Timeout);
        }

        if (IsInsideTree())
        {
            this.target.SelfModulate = defaultModulate;
        }
    }
}
