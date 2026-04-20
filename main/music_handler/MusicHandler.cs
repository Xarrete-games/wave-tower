using Godot;
using System.Collections.Generic;

public partial class MusicHandler : Node
{
    private const int MAX_PLAYERS = 8;
    private const int TOWER_TYPE_FIRE = 0;
    private const int TOWER_TYPE_LIGHTNING = 1;
    private const int TOWER_TYPE_FROST = 2;

    private Node _redPlayers;
    private Node _bluePlayers;
    private Node _greenPlayers;
    private AudioStreamPlayer _basePlayer;
    private Status _status;
    private TowersManager _towersManager;

    private readonly Dictionary<int, Node> _towerPlayers = new();

    public override void _Ready()
    {
        _redPlayers = GetNodeOrNull<Node>("RedPlayers");
        _bluePlayers = GetNodeOrNull<Node>("BluePlayers");
        _greenPlayers = GetNodeOrNull<Node>("GreenPlayers");
        _basePlayer = GetNodeOrNull<AudioStreamPlayer>("BasePlayer");

        _towerPlayers[TOWER_TYPE_FIRE] = _redPlayers;
        _towerPlayers[TOWER_TYPE_FROST] = _bluePlayers;
        _towerPlayers[TOWER_TYPE_LIGHTNING] = _greenPlayers;

        StopMusic();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        _towersManager = runContext?.TowersManager;
        if (_towersManager != null)
        {
            _towersManager.TowerCountChanged += OnTowerCountChange;
        }
        _status = runContext?.Status;
        if (_status != null)
        {
            _status.PlayerDied += StopMusic;
        }
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.TowerCountChanged -= OnTowerCountChange;
            _towersManager = null;
        }

        if (_status != null)
        {
            _status.PlayerDied -= StopMusic;
            _status = null;
        }
    }

    public void PlayMusic()
    {
        _basePlayer?.Play();
        StartPlayers(_redPlayers);
        StartPlayers(_bluePlayers);
        StartPlayers(_greenPlayers);
    }

    public void StopMusic()
    {
        StopPlayers(_redPlayers);
        StopPlayers(_bluePlayers);
        StopPlayers(_greenPlayers);
        _basePlayer?.Stop();
    }

    private void StartPlayers(Node node)
    {
        if (node == null)
        {
            return;
        }

        foreach (Node child in node.GetChildren())
        {
            if (child is AudioStreamPlayer player)
            {
                player.Play();
            }
        }
    }

    private void StopPlayers(Node node)
    {
        if (node == null)
        {
            return;
        }

        foreach (Node child in node.GetChildren())
        {
            if (child is AudioStreamPlayer player)
            {
                StopPlayer(player);
            }
        }
    }

    private void OnTowerCountChange(int towerType, int amount)
    {
        if (amount > MAX_PLAYERS || amount == 0)
        {
            return;
        }

        if (!_towerPlayers.TryGetValue(towerType, out Node playersNode) || playersNode == null)
        {
            return;
        }

        Godot.Collections.Array<Node> playerList = playersNode.GetChildren();
        int playerIndex = amount - 1;
        if (playerIndex < 0 || playerIndex >= playerList.Count)
        {
            return;
        }

        AudioStreamPlayer player = playerList[playerIndex] as AudioStreamPlayer;
        PlayPlayer(player);
    }

    private void PlayPlayer(AudioStreamPlayer player)
    {
        if (player == null)
        {
            return;
        }

        Tween tween = CreateTween();
        tween.TweenProperty(player, "volume_db", 0, 1);
    }

    private void StopPlayer(AudioStreamPlayer player)
    {
        if (player == null)
        {
            return;
        }

        player.VolumeDb = -80;
    }
}
