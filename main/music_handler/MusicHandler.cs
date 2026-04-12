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

    private readonly Dictionary<int, Node> tower_players = new();

    public override void _Ready()
    {
        this.red_players = GetNodeOrNull<Node>("RedPlayers");
        this.blue_players = GetNodeOrNull<Node>("BluePlayers");
        this.green_players = GetNodeOrNull<Node>("GreenPlayers");
        this.base_player = GetNodeOrNull<AudioStreamPlayer>("BasePlayer");

        this.tower_players[TOWER_TYPE_FIRE] = this.red_players;
        this.tower_players[TOWER_TYPE_FROST] = this.blue_players;
        this.tower_players[TOWER_TYPE_LIGHTNING] = this.green_players;

        this.stop_music();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        runContext?.towers_manager?.Connect("tower_count_change", Callable.From<int, int>(this._on_tower_count_change));
        runContext?.status?.Connect("player_died", Callable.From(this.stop_music));
    }

    public void play_music()
    {
        this.base_player?.Play();
        this._start_players(this.red_players);
        this._start_players(this.blue_players);
        this._start_players(this.green_players);
    }

    public void stop_music()
    {
        this._stop_players(this.red_players);
        this._stop_players(this.blue_players);
        this._stop_players(this.green_players);
        this.base_player?.Stop();
    }

    private void _start_players(Node node)
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

    private void _stop_players(Node node)
    {
        if (node == null)
        {
            return;
        }

        foreach (Node child in node.GetChildren())
        {
            if (child is AudioStreamPlayer player)
            {
                this._stop_player(player);
            }
        }
    }

    private void _on_tower_count_change(int tower_type, int amount)
    {
        if (amount > MAX_PLAYERS || amount == 0)
        {
            return;
        }

        if (!this.tower_players.TryGetValue(tower_type, out Node playersNode) || playersNode == null)
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
        this._play_player(player);
    }

    private void _play_player(AudioStreamPlayer player)
    {
        if (player == null)
        {
            return;
        }

        Tween tween = CreateTween();
        tween.TweenProperty(player, "volume_db", 0, 1);
    }

    private void _stop_player(AudioStreamPlayer player)
    {
        if (player == null)
        {
            return;
        }

        player.VolumeDb = -80;
    }
}
