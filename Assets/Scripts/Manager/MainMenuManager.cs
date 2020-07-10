using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles communication between different managers in the main menu.
/// </summary>
public class MainMenuManager : MonoBehaviour {

    public static MainMenuManager instance = null;
    public ScreenTransition screenTransition;
    public Toggle soundButton;
    
    private SaveLoadManager saveLoadManager;
    private StyleManager styleManager;
    private SavedData savedData;
    private SoundManager soundManager;

    private void Awake() {
        if( instance == null ) {
            instance = this;
        }
    }

    private void Start() {
        saveLoadManager = GetComponentInChildren<SaveLoadManager>();
        savedData = saveLoadManager.LoadData();
        styleManager = GetComponentInChildren<StyleManager>();
        styleManager.Init( savedData );
        soundManager = GetComponentInChildren<SoundManager>();
        soundManager.Init( saveLoadManager );

        soundButton.isOn = saveLoadManager.GetSoundStatus() != 0;
    }

    /// <summary>
    /// Change the scene to something else.
    /// </summary>
    public void SwitchScreen( ScreenType screenType ) {
        screenTransition.StartScreenTransition( (int) screenType );
    }

    /// <summary>
    /// Toggle the sound on or off.
    /// </summary>
    public void ToggleButtonSoundPressed() {
        saveLoadManager.SetSoundStatus(soundButton.isOn ? 1 : 0);
        
        soundManager.Init( saveLoadManager );
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );

        if( (SoundStatus) saveLoadManager.GetSoundStatus() == SoundStatus.SOUND_OFF ) {
            soundManager.StopAllSound();
        } else {
            soundManager.ResumeMusicLoop();
        }
    }
}
