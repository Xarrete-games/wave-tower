using Godot;
using System.Collections.Generic;

public partial class MusicHandler : Node
{
    private const int MAX_PLAYERS = 8;
    private const int TOWER_TYPE_FIRE = 0;
    private const int TOWER_TYPE_LIGHTNING = 1;
    private const int TOWER_TYPE_FROST = 2;

    private Node red_players;
    private Node blue_players;
    private Node green_players;
    private AudioStreamPlayer base_player;
    private Status _status;
    private TowersManager _towersManager;

    private readonly Dictionary<int, Node> tower_players = new();

    public override void _Ready()
    {
        red_players = GetNodeOrNull<Node>("RedPlayers");
        blue_players = GetNodeOrNull<Node>("BluePlayers");
        green_players = GetNodeOrNull<Node>("GreenPlayers");
        base_player = GetNodeOrNull<AudioStreamPlayer>("BasePlayer");

        tower_players[TOWER_TYPE_FIRE] = red_players;
        tower_players[TOWER_TYPE_FROST] = blue_players;
        tower_players[TOWER_TYPE_LIGHTNING] = green_players;

        stop_music();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        _towersManager = runContext?.towers_manager;
        if (_towersManager != null)
        {
            _towersManager.tower_count_change += OnTowerCountChange;
        }
        _status = runContext?.status;
        if (_status != null)
        {
            _status.PlayerDied += stop_music;
        }
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.tower_count_change -= OnTowerCountChange;
            _towersManager = null;
        }

        if (_status != null)
        {
            _status.PlayerDied -= stop_music;
            _status = null;
        }
    }

    public void play_music()
    {
        base_player?.Play();
        StartPlayers(red_players);
        StartPlayers(blue_players);
        StartPlayers(green_players);
    }

    public void stop_music()
    {
        StopPlayers(red_players);
        StopPlayers(blue_players);
        StopPlayers(green_players);
        base_player?.Stop();
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

    private void OnTowerCountChange(int tower_type, int amount)
    {
        if (amount > MAX_PLAYERS || amount == 0)
        {
            return;
        }

        if (!tower_players.TryGetValue(tower_type, out Node playersNode) || playersNode == null)
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
