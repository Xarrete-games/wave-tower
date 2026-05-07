using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


[GlobalClass]
public partial class AnimationComponent : Node
{
    public event Action Entered;

    private sealed class AnimationSnapshot
    {
        public Vector2 Scale { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Size { get; set; }
        public Color SelfModulate { get; set; }
    }

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
    private AnimationSnapshot _hoverValues = new();
    private AnimationSnapshot _enterValues = new();
    private AnimationSnapshot _defaultValues = new();
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

        _defaultValues = new AnimationSnapshot
        {
            Scale = _target.Scale,
            Position = _target.Position,
            Rotation = _target.Rotation,
            Size = _target.Size,
            SelfModulate = _target.SelfModulate,
        };

        _hoverValues = new AnimationSnapshot
        {
            Scale = HoverScale,
            Position = _target.Position + HoverPosition,
            Rotation = _target.Rotation + Mathf.DegToRad(HoverRotation),
            Size = _target.Size * HoverSize,
            SelfModulate = HoverModulate,
        };

        _enterValues = new AnimationSnapshot
        {
            Scale = EnterScale,
            Position = _target.Position + EnterPosition,
            Rotation = _target.Rotation + Mathf.DegToRad(EnterRotation),
            Size = _target.Size * EnterSize,
            SelfModulate = EnterModulate,
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

    private async Task AddTween(AnimationSnapshot values, bool parallel, float seconds, float delay, Tween.TransitionType transition, Tween.EaseType easing, bool entering = false)
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
            PropertyTweener tweener = property switch
            {
                "rotation" => tween.TweenProperty(_target, property, values.Rotation, seconds),
                "scale" => tween.TweenProperty(_target, property, values.Scale, seconds),
                "position" => tween.TweenProperty(_target, property, values.Position, seconds),
                "size" => tween.TweenProperty(_target, property, values.Size, seconds),
                "self_modulate" => tween.TweenProperty(_target, property, values.SelfModulate, seconds),
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
        if (_target == null)
        {
            return;
        }

        Color defaultModulate = _defaultValues.SelfModulate;
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
