using Godot;
using System;
using System.Collections.Generic;


[GlobalClass]
public partial class AnimationComponent : Node
{
    public event Action entered;

    private const Tween.TransitionType IMMEDIATE_TRANSITION = Tween.TransitionType.Linear;

    [ExportGroup("Options")]
    [Export] public bool FromCenter = true;
    [Export] public bool ParallelAnimations = true;
    [Export] public bool EnterAnimation = false;
    [Export] public string[] properties = { "scale", "position", "rotation", "size", "self_modulate" };
    [Export] public bool flicked = false;

    [ExportGroup("Hover Settings")]
    [Export] public float HoverTime = 0.2f;
    [Export] public float HoverDelay = 0.0f;
    [Export] public Tween.TransitionType HoverTransition = Tween.TransitionType.Linear;
    [Export] public Tween.EaseType HoverEasing = Tween.EaseType.InOut;
    [Export] public Vector2 HoverScale = Vector2.One;
    [Export] public Vector2 HoverPosition = Vector2.Zero;
    [Export] public float HoverRotation = 0.0f;
    [Export] public Vector2 HoverSize = Vector2.One;
    [Export] public Color HoverModulate = Colors.White;
    [Export] public bool PlayHoverSound = false;

    [ExportGroup("Enter Settings")]
    [Export] public AnimationComponent WaitFor;
    [Export] public float EnterTime = 0.2f;
    [Export] public float EnterDelay = 0.0f;
    [Export] public Tween.TransitionType EnterTransition = Tween.TransitionType.Linear;
    [Export] public Tween.EaseType EnterEasing = Tween.EaseType.InOut;
    [Export] public Vector2 EnterScale = Vector2.One;
    [Export] public Vector2 EnterPosition = Vector2.Zero;
    [Export] public float EnterRotation = 0.0f;
    [Export] public Vector2 EnterSize = Vector2.One;
    [Export] public Color EnterModulate = Colors.White;

    [ExportGroup("Flicked Settings")]
    [Export] public float FlickedTime = 0.1f;
    [Export] public Color FlickedColor = new(1, 1, 1, 0.5f);

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
        _ = add_tween(hover_values, ParallelAnimations, HoverTime, HoverDelay, HoverTransition, HoverEasing);
        if (PlayHoverSound)
        {
            AudioManager audioManager = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
            audioManager?.PlayButtonHover();
        }
    }

    public void on_hover_exited()
    {
        on_hover = false;
        _ = add_tween(default_values, ParallelAnimations, HoverTime, HoverDelay, HoverTransition, HoverEasing);
    }

    public void on_entered_action()
    {
        _ = add_tween(default_values, ParallelAnimations, EnterTime, EnterDelay, EnterTransition, EnterEasing, true);
    }

    public void connect_signals()
    {
        target.MouseEntered += on_hover_entered;
        target.MouseExited += on_hover_exited;

        if (WaitFor != null)
        {
            WaitFor.entered += on_entered_action;
        }
    }

    public async void setup()
    {
        if (target == null)
        {
            return;
        }

        if (HoverPosition == Vector2.Zero && EnterPosition == Vector2.Zero)
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

        if (FromCenter)
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
            { "scale", HoverScale },
            { "position", target.Position + HoverPosition },
            { "rotation", target.Rotation + Mathf.DegToRad(HoverRotation) },
            { "size", target.Size * HoverSize },
            { "self_modulate", HoverModulate },
        };

        enter_values = new Dictionary<string, Variant>
        {
            { "scale", EnterScale },
            { "position", target.Position + EnterPosition },
            { "rotation", target.Rotation + Mathf.DegToRad(EnterRotation) },
            { "size", target.Size * EnterSize },
            { "self_modulate", EnterModulate },
        };

        connect_signals();

        if (flicked)
        {
            _ = flick_loop();
        }

        if (EnterAnimation)
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

        if (WaitFor == null)
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

            target.SelfModulate = useFlick ? FlickedColor : defaultModulate;
            useFlick = !useFlick;
            await ToSignal(GetTree().CreateTimer(FlickedTime), Timer.SignalName.Timeout);
        }

        if (IsInsideTree())
        {
            target.SelfModulate = defaultModulate;
        }
    }
}
