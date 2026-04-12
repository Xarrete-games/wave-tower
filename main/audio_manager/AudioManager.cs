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
        this.button_click = GetNodeOrNull<AudioStreamPlayer>("ButtonClick");
        this.button_hover = GetNodeOrNull<AudioStreamPlayer>("ButtonHover");
        this.coins = GetNodeOrNull<AudioStreamPlayer>("Coins");
        this.main_piano_player = GetNodeOrNull<AudioStreamPlayer>("MainPianoPlayer");
        this.purchase_player = GetNodeOrNull<AudioStreamPlayer>("PurchasePlayer");
        this.wave_clear = GetNodeOrNull<AudioStreamPlayer>("WaveClear");
        this.defeated_sound = GetNodeOrNull<AudioStreamPlayer>("DefeatedSound");
        this.relic_obtain = GetNodeOrNull<AudioStreamPlayer>("Relic_obtain");
        this.tower_obtain = GetNodeOrNull<AudioStreamPlayer>("Tower_obtain");
        this.place_tower = GetNodeOrNull<AudioStreamPlayer>("Place_tower");
        this.armor = GetNodeOrNull<AudioStreamPlayer>("Armor");
        this.loss_hp = GetNodeOrNull<AudioStreamPlayer>("Loss_HP");
        this.loss_armor = GetNodeOrNull<AudioStreamPlayer>("Loss_Armor");
    }

    public void play_coins() => this.coins?.Play();
    public void play_relic_obtain() => this.relic_obtain?.Play();
    public void play_tower_obtain() => this.tower_obtain?.Play();
    public void play_button_hover() => this.button_hover?.Play();
    public void play_button_click() => this.button_click?.Play();
    public void play_purchase() => this.purchase_player?.Play();
    public void stop_main_piano() => this.main_piano_player?.Stop();
    public void play_wave_clear() => this.wave_clear?.Play();
    public void play_defeated_sound() => this.defeated_sound?.Play();
    public void play_place_tower() => this.place_tower?.Play();
    public void play_player_hurt() => this.loss_hp?.Play();
    public void play_armor_block() => this.loss_armor?.Play();
    public void play_gain_armor() => this.armor?.Play();

    public void play_main_piano()
    {
        if (this.main_piano_player == null || this.main_piano_player.Playing)
        {
            return;
        }

        this.main_piano_player.Play();
    }
}
