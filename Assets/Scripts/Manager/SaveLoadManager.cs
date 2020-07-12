using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Handles persistent saving and loading of all relevant data.
/// </summary>
public class SaveLoadManager : MonoBehaviour {

    [SerializeField]
    public List<HatObject> standardHatObjects = new List<HatObject>();
    public List<ColorObject> standardColorObjects = new List<ColorObject>();
    public List<PowerupObject> standardPowerupObjects = new List<PowerupObject>();
    
    private const string soundStatusKey = "SoundStatus";
    private const string tutorialStatusKey = "TutorialStatus";
    private const string lastRewardTimeKey = "LastRewardTime";
    private const string defaultLastRewardTime = "01.01.2000 00:00:00";

    // PC only keys for PlayerPrefs
    private const string saveFileExistsKey = "SaveFileExists";
    private const string saveFileKey = "SaveFile";

    /// <summary>
    /// Save data persistently on device.
    /// </summary>
    public void SaveData( SavedData savedData ) {
        PlayerPrefs.SetInt(saveFileExistsKey, 1);

    }

    /// <summary>
    /// Load data from device or create new data if no save file exists yet.
    /// </summary>
    public SavedData LoadData() {
        if (PlayerPrefs.GetInt(saveFileExistsKey, 0) == 0) {
            SavedData savedData = new SavedData(standardHatObjects, standardColorObjects, standardPowerupObjects);
            SaveData(savedData);
            return savedData;
        } else {
            SavedData savedData = new SavedData(standardHatObjects, standardColorObjects, standardPowerupObjects);
            savedData.ParseSavedString(PlayerPrefs.GetString(saveFileKey, ""));
            return savedData;
        }
    }

    public int GetSoundStatus() {
        return PlayerPrefs.GetInt( soundStatusKey, (int) SoundStatus.SOUND_ON );
    }

    public void SetSoundStatus( int value ) {
        PlayerPrefs.SetInt( soundStatusKey, value );
    }

    public bool GetTutorialStatus() {
        return PlayerPrefs.GetInt( tutorialStatusKey, (int) TutorialStatus.TUTORIAL_OPEN ) != 0;
    }

    public void SetTutorialStatus( int value ) {
        PlayerPrefs.SetInt( tutorialStatusKey, value );
    }

    public System.DateTime GetLastRewardTime() {
        return System.DateTime.Parse( PlayerPrefs.GetString( lastRewardTimeKey, defaultLastRewardTime ) );
    }

    public void SetLastRewardTime( string lastRewardTime ) {
        PlayerPrefs.SetString( lastRewardTimeKey, lastRewardTime );
    }
}
