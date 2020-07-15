using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles communication between different managers in the game scene.
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public Snake snake;
    public CameraController cameraController;

    private FruitSpawner fruitSpawner;
    private ScoreManager scoreManager;
    private GuiManager guiManager;
    private PowerupSpawner powerupSpawner;
    private StyleManager styleManager;
    private SoundManager soundManager;
    private SaveLoadManager saveLoadManager;
    private SavedData savedData;

    private bool paused;

    private void Awake() {
        if( instance == null ) {
            instance = this;
        }
    }

    private void Start() {
        saveLoadManager = GetComponentInChildren<SaveLoadManager>();
        savedData = saveLoadManager.LoadData();

        fruitSpawner = GetComponentInChildren<FruitSpawner>();
        if( saveLoadManager.GetTutorialStatus() ) {
            fruitSpawner.Init();
        }
        scoreManager = GetComponentInChildren<ScoreManager>();
        scoreManager.Init( savedData );
        guiManager = GetComponentInChildren<GuiManager>();
        guiManager.Init( saveLoadManager );
        powerupSpawner = GetComponentInChildren<PowerupSpawner>();
        powerupSpawner.Init( savedData );
        cameraController.Init();
        styleManager = GetComponentInChildren<StyleManager>();
        styleManager.Init( savedData );
        soundManager = GetComponentInChildren<SoundManager>();
        soundManager.Init( saveLoadManager );
        soundManager.PlaySound( SoundEffectType.SOUND_SLITHER, false );
    }

    /// <summary>
    /// Tutorial on first play has been completed so start spawning fruit.
    /// </summary>
    public void TutorialDone() {
        fruitSpawner.Init();
        fruitSpawner.SpawnNewFruit( false );
    }

    /// <summary>
    /// Player has collected fruit so do all logic based on it.
    /// </summary>
    public void PlayerCollectedFruit() {
        fruitSpawner.SpawnNewFruit( false );
        scoreManager.IncreaseScore();
        powerupSpawner.UpdateActualCollectedFruit( scoreManager.GetCurrentScore() );
        fruitSpawner.SetMoveFruitTowardsPlayer( false );
        guiManager.FruitCollected( scoreManager.GetCurrentScore() );
        soundManager.PlaySound( SoundEffectType.SOUND_EAT, true );
        soundManager.CheckMusicLoopLevelIncrease( scoreManager.GetCurrentScore() );
    }

    /// <summary>
    /// Player collected powerup so find out which random one it is.
    /// </summary>
    public PlayerPowerupTypes PlayerCollectedPowerup() {
        soundManager.StopSound( SoundEffectType.SOUND_SLITHER );
        soundManager.PlaySound( SoundEffectType.SOUND_POWERUP_COLLECT, false );

        PlayerPowerupTypes collectedType = powerupSpawner.CollectPowerup( soundManager );
        guiManager.ShowPowerupIcon( collectedType );
        guiManager.SetPowerupDuration( powerupSpawner.GetPowerupDuration() );
        return collectedType;
    }

    /// <summary>
    /// Magnet collider of snake touched a fruit so move towards snake.
    /// </summary>
    public void PlayerMagnetTouchedFruit() {
        fruitSpawner.SetMoveFruitTowardsPlayer( true );
    }

    /// <summary>
    /// Any type of powerup has worn off so reset everything necessary. If "resumeSpawning" is set, then new powerups should be spawned after some time.
    /// </summary>
    public void PowerupWoreOff( bool resumeSpawning ) {
        guiManager.HidePowerupText();

        soundManager.StopSound( SoundEffectType.SOUND_THIN );
        soundManager.StopSound( SoundEffectType.SOUND_MAGNET );
        soundManager.StopSound( SoundEffectType.SOUND_INVINCIBILITY );
        soundManager.PlaySound( SoundEffectType.SOUND_SLITHER, false );
        soundManager.PlaySound( SoundEffectType.SOUND_POWERUP_WORE_OFF, false );

        if( resumeSpawning ) {
            powerupSpawner.ResumeSpawning();
        }
        fruitSpawner.SetMoveFruitTowardsPlayer( false );
    }

    /// <summary>
    /// Snake touched tail, so do everything for game over.
    /// </summary>
    public void PlayerTouchedTail() {
        fruitSpawner.Stop();
        powerupSpawner.Stop();
        snake.Stop();
        guiManager.ToggleHUD(false);

        soundManager.StopSound( SoundEffectType.SOUND_SLITHER );
        soundManager.PlaySound( SoundEffectType.SOUND_TAIL_EAT, false );
        soundManager.StopMusicLoop();

        if ( scoreManager.CheckDailyPlayReward(saveLoadManager) ) {
            cameraController.Stop();
            scoreManager.ClaimDailyPlayReward(saveLoadManager, savedData);
            scoreManager.FinalizeScore(savedData);
            guiManager.ShowGameOverScreen(soundManager, scoreManager.GetCurrentScore(), scoreManager.GetTotalScore(), scoreManager.IsNewHighscore(), true, savedData.IsSomethingPurchaseable());
        } else {
            cameraController.Stop();
            scoreManager.FinalizeScore(savedData);
            guiManager.ShowGameOverScreen(soundManager, scoreManager.GetCurrentScore(), scoreManager.GetTotalScore(), scoreManager.IsNewHighscore(), false, savedData.IsSomethingPurchaseable());
        }
    }

    /// <summary>
    /// Game over screen has been shown, so do all the heavy post processing (like posting scores) for it.
    /// </summary>
    public void GameOverScreenShown() {
        saveLoadManager.SetTutorialStatus( (int) TutorialStatus.TUTORIAL_DONE );
        saveLoadManager.SaveData( savedData );
    }

    /// <summary>
    /// Change the scene to something else (or reset current scene).
    /// </summary>
    public void SwitchScreen( ScreenType screenType ) {
        guiManager.ShowScreenTransition( (int) screenType );
    }

    /// <summary>
    /// Pause game.
    /// </summary>
    public void GamePaused() {
        paused = !paused;
        if( paused ) {
            Time.timeScale = 0;
            guiManager.TogglePauseMenu( true );
            soundManager.PlaySound( SoundEffectType.SOUND_PAUSE_START, false );
        } else {
            Time.timeScale = 1;
            guiManager.TogglePauseMenu( false );
            soundManager.PlaySound( SoundEffectType.SOUND_PAUSE_END, false );
        }
    }

    public Transform GetCurrentSnakePosition() {
        return snake.GetCurrentPosition();
    }

    public Transform GetLastTailTransform() {
        return snake.GetLastTailTransform();
    }

    public float GetPowerupDuration() {
        return powerupSpawner.GetPowerupDuration();
    }

    public SavedData GetSavedData() {
        return savedData;
    }
}
