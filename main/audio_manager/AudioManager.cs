using Godot;

public partial class AudioManager : Node
{
    private AudioStreamPlayer _buttonClick;
    private AudioStreamPlayer _buttonHover;
    private AudioStreamPlayer _coins;
    private AudioStreamPlayer _mainPianoPlayer;
    private AudioStreamPlayer _purchasePlayer;
    private AudioStreamPlayer _waveClear;
    private AudioStreamPlayer _defeatedSound;
    private AudioStreamPlayer _relicObtain;
    private AudioStreamPlayer _towerObtain;
    private AudioStreamPlayer _placeTower;
    private AudioStreamPlayer _armor;
    private AudioStreamPlayer _lossHp;
    private AudioStreamPlayer _lossArmor;

    public override void _Ready()
    {
        _buttonClick = GetNodeOrNull<AudioStreamPlayer>("ButtonClick");
        _buttonHover = GetNodeOrNull<AudioStreamPlayer>("ButtonHover");
        _coins = GetNodeOrNull<AudioStreamPlayer>("Coins");
        _mainPianoPlayer = GetNodeOrNull<AudioStreamPlayer>("MainPianoPlayer");
        _purchasePlayer = GetNodeOrNull<AudioStreamPlayer>("PurchasePlayer");
        _waveClear = GetNodeOrNull<AudioStreamPlayer>("WaveClear");
        _defeatedSound = GetNodeOrNull<AudioStreamPlayer>("DefeatedSound");
        _relicObtain = GetNodeOrNull<AudioStreamPlayer>("Relic_obtain");
        _towerObtain = GetNodeOrNull<AudioStreamPlayer>("Tower_obtain");
        _placeTower = GetNodeOrNull<AudioStreamPlayer>("Place_tower");
        _armor = GetNodeOrNull<AudioStreamPlayer>("Armor");
        _lossHp = GetNodeOrNull<AudioStreamPlayer>("Loss_HP");
        _lossArmor = GetNodeOrNull<AudioStreamPlayer>("Loss_Armor");
    }

    public void PlayCoins() => _coins?.Play();
    public void PlayRelicObtain() => _relicObtain?.Play();
    public void PlayTowerObtain() => _towerObtain?.Play();
    public void PlayButtonHover() => _buttonHover?.Play();
    public void PlayButtonClick() => _buttonClick?.Play();
    public void PlayPurchase() => _purchasePlayer?.Play();
    public void StopMainPiano() => _mainPianoPlayer?.Stop();
    public void PlayWaveClear() => _waveClear?.Play();
    public void PlayDefeatedSound() => _defeatedSound?.Play();
    public void PlayPlaceTower() => _placeTower?.Play();
    public void PlayPlayerHurt() => _lossHp?.Play();
    public void PlayArmorBlock() => _lossArmor?.Play();
    public void PlayGainArmor() => _armor?.Play();

    public void PlayMainPiano()
    {
        if (_mainPianoPlayer == null || _mainPianoPlayer.Playing)
        {
            return;
        }

        _mainPianoPlayer.Play();
    }
}
