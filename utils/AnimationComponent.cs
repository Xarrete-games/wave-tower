using Godot;
using System;
using System.Collections.Generic;


[GlobalClass]
public partial class AnimationComponent : Node
{
    public event Action entered;

    private const Tween.TransitionType IMMEDIATE_TRANSITION = Tween.TransitionType.Linear;

    [ExportGroup("Options")]
    [Export] public bool from_center = true;
    [Export] public bool parallel_animations = true;
    [Export] public bool enter_animation = false;
    [Export] public string[] properties = { "scale", "position", "rotation", "size", "self_modulate" };
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
    private Dictionary<string, Variant> hover_values = new();
    private Dictionary<string, Variant> enter_values = new();
    private Dictionary<string, Variant> default_values = new();
    private bool on_hover = false;

    public override void _Ready()
    {
        target = GetParent() as Control;
        CallDeferred(nameof(setup));
    }

    public void on_hover_entered()
    {
        on_hover = true;
        _ = add_tween(hover_values, parallel_animations, hover_time, hover_delay, hover_transition, hover_easing);
        if (play_hover_sound)
        {
            AudioManager audioManager = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
            audioManager?.play_button_hover();
        }
    }

    public void on_hover_exited()
    {
        on_hover = false;
        _ = add_tween(default_values, parallel_animations, hover_time, hover_delay, hover_transition, hover_easing);
    }

    public void on_entered_action()
    {
        _ = add_tween(default_values, parallel_animations, enter_time, enter_delay, enter_transition, enter_easing, true);
    }

    public void connect_signals()
    {
        target.MouseEntered += on_hover_entered;
        target.MouseExited += on_hover_exited;

        if (wait_for != null)
        {
            wait_for.entered += on_entered_action;
        }
    }

    public async void setup()
    {
        if (target == null)
        {
            return;
        }

        if (hover_position == Vector2.Zero && enter_position == Vector2.Zero)
        {
            var filtered = new List<string>();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] != "position")
                {
                    filtered.Add(properties[i]);
                }
            }
            properties = filtered.ToArray();
        }

        if (from_center)
        {
            target.PivotOffset = target.Size / 2.0f;
        }

        default_scale = target.Scale;
        default_values = new Dictionary<string, Variant>
        {
            { "scale", target.Scale },
            { "position", target.Position },
            { "rotation", target.Rotation },
            { "size", target.Size },
            { "self_modulate", target.SelfModulate },
        };

        hover_values = new Dictionary<string, Variant>
        {
            { "scale", hover_scale },
            { "position", target.Position + hover_position },
            { "rotation", target.Rotation + Mathf.DegToRad(hover_rotation) },
            { "size", target.Size * hover_size },
            { "self_modulate", hover_modulate },
        };

        enter_values = new Dictionary<string, Variant>
        {
            { "scale", enter_scale },
            { "position", target.Position + enter_position },
            { "rotation", target.Rotation + Mathf.DegToRad(enter_rotation) },
            { "size", target.Size * enter_size },
            { "self_modulate", enter_modulate },
        };

        connect_signals();

        if (flicked)
        {
            _ = flick_loop();
        }

        if (enter_animation)
        {
            on_enter();
        }
        else
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            entered?.Invoke();
        }
    }

    public void on_enter()
    {
        _ = add_tween(enter_values, true, 0.0f, 0.0f, IMMEDIATE_TRANSITION, Tween.EaseType.In);

        if (wait_for == null)
        {
            on_entered_action();
        }
    }

    public async System.Threading.Tasks.Task add_tween(Dictionary<string, Variant> values, bool parallel, float seconds, float delay, Tween.TransitionType transition, Tween.EaseType easing, bool entering = false)
    {
        if (!IsInsideTree() || target == null)
        {
            return;
        }

        Tween tween = GetTree().CreateTween();
        tween.SetParallel(parallel);
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.Pause();

        for (int i = 0; i < properties.Length; i++)
        {
            string property = properties[i];
            Variant value = values.TryGetValue(property, out Variant configuredValue) ? configuredValue : default;
            tween.TweenProperty(target, property, value, seconds).SetTrans(transition).SetEase(easing);
        }

        await ToSignal(GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
        tween.Play();

        if (entering)
        {
            await ToSignal(tween, Tween.SignalName.Finished);
            entered?.Invoke();
        }
    }

    public async System.Threading.Tasks.Task flick_loop()
    {
        if (default_values.Count == 0)
        {
            return;
        }

        Color defaultModulate = default_values["self_modulate"].AsColor();
        bool useFlick = true;

        while (flicked && IsInsideTree())
        {
            if (on_hover)
            {
                target.SelfModulate = defaultModulate;
                await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                continue;
            }

            target.SelfModulate = useFlick ? flicked_color : defaultModulate;
            useFlick = !useFlick;
            await ToSignal(GetTree().CreateTimer(flicked_time), Timer.SignalName.Timeout);
        }

        if (IsInsideTree())
        {
            target.SelfModulate = defaultModulate;
        }
    }
}
