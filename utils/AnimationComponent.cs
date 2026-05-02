using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


[GlobalClass]
public partial class AnimationComponent : Node
{
    public event Action Entered;

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

    private Control _target;
    private Dictionary<string, object> _hoverValues = new();
    private Dictionary<string, object> _enterValues = new();
    private Dictionary<string, object> _defaultValues = new();
    private bool _onHover = false;
    private bool _signalsConnected;

    public override void _Ready()
    {
        _target = GetParent() as Control;
        CallDeferred(nameof(Setup));
    }

    public override void _ExitTree()
    {
        DisconnectSignals();
    }

    public void OnHoverEntered()
    {
        _onHover = true;
        _ = AddTween(_hoverValues, ParallelAnimations, HoverTime, HoverDelay, HoverTransition, HoverEasing);
        if (PlayHoverSound)
        {
            AudioManager audioManager = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
            audioManager?.PlayButtonHover();
        }
    }

    public void OnHoverExited()
    {
        _onHover = false;
        _ = AddTween(_defaultValues, ParallelAnimations, HoverTime, HoverDelay, HoverTransition, HoverEasing);
    }

    public void OnEnteredAction()
    {
        _ = AddTween(_defaultValues, ParallelAnimations, EnterTime, EnterDelay, EnterTransition, EnterEasing, true);
    }

    public void ConnectSignals()
    {
        if (_signalsConnected || _target == null)
        {
            return;
        }

        _target.MouseEntered += OnHoverEntered;
        _target.MouseExited += OnHoverExited;

        if (WaitFor != null)
        {
            WaitFor.Entered += OnEnteredAction;
        }

        _signalsConnected = true;
    }

    private void DisconnectSignals()
    {
        if (!_signalsConnected)
        {
            return;
        }

        if (_target != null)
        {
            _target.MouseEntered -= OnHoverEntered;
            _target.MouseExited -= OnHoverExited;
        }

        if (WaitFor != null)
        {
            WaitFor.Entered -= OnEnteredAction;
        }

        _signalsConnected = false;
    }

    private void Setup()
    {
        AsyncTaskHelper.FireAndForget(SetupAsync(), "AnimationComponent.SetupAsync");
    }

    private async Task SetupAsync()
    {
        if (_target == null)
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
            _target.PivotOffset = _target.Size / 2.0f;
        }

        _defaultValues = new Dictionary<string, object>
        {
            { "scale", _target.Scale },
            { "position", _target.Position },
            { "rotation", _target.Rotation },
            { "size", _target.Size },
            { "self_modulate", _target.SelfModulate },
        };

        _hoverValues = new Dictionary<string, object>
        {
            { "scale", HoverScale },
            { "position", _target.Position + HoverPosition },
            { "rotation", _target.Rotation + Mathf.DegToRad(HoverRotation) },
            { "size", _target.Size * HoverSize },
            { "self_modulate", HoverModulate },
        };

        _enterValues = new Dictionary<string, object>
        {
            { "scale", EnterScale },
            { "position", _target.Position + EnterPosition },
            { "rotation", _target.Rotation + Mathf.DegToRad(EnterRotation) },
            { "size", _target.Size * EnterSize },
            { "self_modulate", EnterModulate },
        };

        ConnectSignals();

        if (flicked)
        {
            AsyncTaskHelper.FireAndForget(FlickLoop(), "AnimationComponent.FlickLoop");
        }

        if (EnterAnimation)
        {
            OnEnter();
        }
        else
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            Entered?.Invoke();
        }
    }

    public void OnEnter()
    {
        _ = AddTween(_enterValues, true, 0.0f, 0.0f, IMMEDIATE_TRANSITION, Tween.EaseType.In);

        if (WaitFor == null)
        {
            OnEnteredAction();
        }
    }

    private async Task AddTween(Dictionary<string, object> values, bool parallel, float seconds, float delay, Tween.TransitionType transition, Tween.EaseType easing, bool entering = false)
    {
        if (!IsInsideTree() || _target == null)
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
            if (!values.TryGetValue(property, out object configuredValue))
            {
                continue;
            }

            PropertyTweener tweener = configuredValue switch
            {
                float floatValue => tween.TweenProperty(_target, property, floatValue, seconds),
                Vector2 vector2Value => tween.TweenProperty(_target, property, vector2Value, seconds),
                Color colorValue => tween.TweenProperty(_target, property, colorValue, seconds),
                _ => null,
            };

            tweener?.SetTrans(transition).SetEase(easing);
        }

        await ToSignal(GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
        tween.Play();

        if (entering)
        {
            await ToSignal(tween, Tween.SignalName.Finished);
            Entered?.Invoke();
        }
    }

    private async Task FlickLoop()
    {
        if (_defaultValues.Count == 0)
        {
            return;
        }

        Color defaultModulate = _defaultValues["self_modulate"] is Color color ? color : Colors.White;
        bool useFlick = true;

        while (flicked && IsInsideTree())
        {
            if (_onHover)
            {
                _target.SelfModulate = defaultModulate;
                await ToSignal(GetTree().CreateTimer(0.05f), Timer.SignalName.Timeout);
                continue;
            }

            _target.SelfModulate = useFlick ? FlickedColor : defaultModulate;
            useFlick = !useFlick;
            await ToSignal(GetTree().CreateTimer(FlickedTime), Timer.SignalName.Timeout);
        }

        if (IsInsideTree())
        {
            _target.SelfModulate = defaultModulate;
        }
    }
}
