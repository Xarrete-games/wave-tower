using Godot;

public partial class AudioManager : Node
{
    private AudioStreamPlayer button_click;
    private AudioStreamPlayer button_hover;
    private AudioStreamPlayer coins;
    private AudioStreamPlayer main_piano_player;
    private AudioStreamPlayer purchase_player;
    private AudioStreamPlayer wave_clear;
    private AudioStreamPlayer defeated_sound;
    private AudioStreamPlayer relic_obtain;
    private AudioStreamPlayer tower_obtain;
    private AudioStreamPlayer place_tower;
    private AudioStreamPlayer armor;
    private AudioStreamPlayer loss_hp;
    private AudioStreamPlayer loss_armor;

    public override void _Ready()
    {
        button_click = GetNodeOrNull<AudioStreamPlayer>("ButtonClick");
        button_hover = GetNodeOrNull<AudioStreamPlayer>("ButtonHover");
        coins = GetNodeOrNull<AudioStreamPlayer>("Coins");
        main_piano_player = GetNodeOrNull<AudioStreamPlayer>("MainPianoPlayer");
        purchase_player = GetNodeOrNull<AudioStreamPlayer>("PurchasePlayer");
        wave_clear = GetNodeOrNull<AudioStreamPlayer>("WaveClear");
        defeated_sound = GetNodeOrNull<AudioStreamPlayer>("DefeatedSound");
        relic_obtain = GetNodeOrNull<AudioStreamPlayer>("Relic_obtain");
        tower_obtain = GetNodeOrNull<AudioStreamPlayer>("Tower_obtain");
        place_tower = GetNodeOrNull<AudioStreamPlayer>("Place_tower");
        armor = GetNodeOrNull<AudioStreamPlayer>("Armor");
        loss_hp = GetNodeOrNull<AudioStreamPlayer>("Loss_HP");
        loss_armor = GetNodeOrNull<AudioStreamPlayer>("Loss_Armor");
    }

    public void play_coins() => coins?.Play();
    public void play_relic_obtain() => relic_obtain?.Play();
    public void play_tower_obtain() => tower_obtain?.Play();
    public void play_button_hover() => button_hover?.Play();
    public void play_button_click() => button_click?.Play();
    public void play_purchase() => purchase_player?.Play();
    public void stop_main_piano() => main_piano_player?.Stop();
    public void play_wave_clear() => wave_clear?.Play();
    public void play_defeated_sound() => defeated_sound?.Play();
    public void play_place_tower() => place_tower?.Play();
    public void play_player_hurt() => loss_hp?.Play();
    public void play_armor_block() => loss_armor?.Play();
    public void play_gain_armor() => armor?.Play();

    public void play_main_piano()
    {
        if (main_piano_player == null || main_piano_player.Playing)
        {
            return;
        }

        main_piano_player.Play();
    }
}
