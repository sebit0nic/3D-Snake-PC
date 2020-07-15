using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the data that is being saved on disk
/// </summary>
[System.Serializable]
public class SavedData {
    
    public List<HatObject> hatObjectList = new List<HatObject>();
    public List<ColorObject> colorObjectList = new List<ColorObject>();
    public List<PowerupObject> powerupObjectList = new List<PowerupObject>();
    public int highscore;
    public int totalScore;
    public PlayerHatTypes currentHat;
    public PlayerColorTypes currentColor;

    private ShopSection currentShopSection;
    private const int highscoreLength = 3;
    private const int totalscoreLength = 5;

    public SavedData( List<HatObject> standardHatObjects, List<ColorObject> standardColorObjects, List<PowerupObject> standardPowerupObjects ) {
        hatObjectList = standardHatObjects;
        colorObjectList = standardColorObjects;
        powerupObjectList = standardPowerupObjects;
        highscore = 0;
        totalScore = 0;
        currentHat = PlayerHatTypes.TYPE_DEFAULT;
        currentColor = PlayerColorTypes.COLOR_CLASSIC;
    }

    /// <summary>
    /// Unlock a certain purchaseable by the players request.
    /// </summary>
    public void UnlockPurchaseable( int sectionIndex, int purchaseableIndex ) {
        currentShopSection = (ShopSection) sectionIndex;
        switch( currentShopSection ) {
            case ShopSection.HATS:
                totalScore -= hatObjectList[purchaseableIndex].GetPrice();
                hatObjectList[purchaseableIndex].Unlock();
                break;
            case ShopSection.COLORSCHEME:
                totalScore -= colorObjectList[purchaseableIndex].GetPrice();
                colorObjectList[purchaseableIndex].Unlock();
                break;
            case ShopSection.POWERUPS:
                totalScore -= powerupObjectList[purchaseableIndex].GetPrice();
                powerupObjectList[purchaseableIndex].Unlock();
                break;
        }
    }

    /// <summary>
    /// Change the currently selected purchaseable by the players request.
    /// </summary>
    public void SelectPurchaseable( int sectionIndex, int purchaseableIndex ) {
        currentShopSection = (ShopSection) sectionIndex;
        switch( currentShopSection ) {
            case ShopSection.HATS:
                currentHat = (PlayerHatTypes) purchaseableIndex;
                break;
            case ShopSection.COLORSCHEME:
                currentColor = (PlayerColorTypes) purchaseableIndex;
                break;
        }
    }

    public bool IsPurchaseableUnlocked( int sectionIndex, int purchaseableIndex ) {
        currentShopSection = (ShopSection) sectionIndex;
        switch( currentShopSection ) {
            case ShopSection.HATS:
                return hatObjectList[purchaseableIndex].IsUnlocked();
            case ShopSection.COLORSCHEME:
                return colorObjectList[purchaseableIndex].IsUnlocked();
            case ShopSection.POWERUPS:
                return powerupObjectList[purchaseableIndex].IsUnlocked();
            default:
                return false;
        }
    }

    public int GetPurchaseablePrice( int sectionIndex, int purchaseableIndex ) {
        currentShopSection = (ShopSection) sectionIndex;
        switch( currentShopSection ) {
            case ShopSection.HATS:
                return hatObjectList[purchaseableIndex].GetPrice();
            case ShopSection.COLORSCHEME:
                return colorObjectList[purchaseableIndex].GetPrice();
            case ShopSection.POWERUPS:
                return powerupObjectList[purchaseableIndex].GetPrice();
            default:
                return 0;
        }
    }

    public string GetPurchaseableName( int sectionIndex, int purchaseableIndex ) {
        currentShopSection = (ShopSection) sectionIndex;
        switch( currentShopSection ) {
            case ShopSection.HATS:
                return hatObjectList[purchaseableIndex].GetName();
            case ShopSection.COLORSCHEME:
                return colorObjectList[purchaseableIndex].GetName();
            case ShopSection.POWERUPS:
                return powerupObjectList[purchaseableIndex].GetName();
            default:
                return "DUMMY";
        }
    }

    public Color GetColorByPurchaseableColorType( PurchaseableColorType purchaseableColorType ) {
        return colorObjectList[(int) currentColor].GetColorByColorType( purchaseableColorType );
    }

    public Color GetColorByPurchaseableColorIndex( PurchaseableColorType purchaseableColorType, int index ) {
        return colorObjectList[index].GetColorByColorType( purchaseableColorType );
    }

    public int GetCurrentLevel( int purchaseableIndex ) {
        return powerupObjectList[purchaseableIndex].GetCurrentLevel();
    }

    public int GetMaxLevel( int purchaseableIndex ) {
        return powerupObjectList[purchaseableIndex].GetMaxLevel();
    }

    /// <summary>
    /// Check if there is any purchaseable that can be bought at the moment so that a notification
    /// can be shown on the game over screen.
    /// </summary>
    public bool IsSomethingPurchaseable() {
        for( int i = 1; i < hatObjectList.Count; i++ ) {
            if( hatObjectList[i].GetPrice() <= totalScore && !hatObjectList[i].IsUnlocked() ) {
                return true;
            }
        }

        for (int i = 1; i < colorObjectList.Count; i++) {
            if (colorObjectList[i].GetPrice() <= totalScore && !colorObjectList[i].IsUnlocked()) {
                return true;
            }
        }

        foreach( PowerupObject powerupObject in powerupObjectList ) {
            if( powerupObject.GetPrice() <= totalScore && !powerupObject.IsUnlocked() ) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Prepare the save string for PlayerPrefs saving.
    /// </summary>
    public string CreateSavedString() {
        string savedString = "";
        for (int i = 0; i < hatObjectList.Count; i++) {
            savedString += "H" + i + ( hatObjectList[i].IsUnlocked() ? 1 : 0 ) + ";";
        }

        for (int i = 0; i < colorObjectList.Count; i++) {
            savedString += "C" + i + ( colorObjectList[i].IsUnlocked() ? 1 : 0 ) + ";";
        }

        for (int i = 0; i < powerupObjectList.Count; i++) {
            savedString += "P" + i + powerupObjectList[i].GetCurrentLevel() + ";";
        }

        savedString += "S" + highscore.ToString().PadLeft(highscoreLength, '0') + ";";
        savedString += "T" + totalScore.ToString().PadLeft(totalscoreLength, '0') + ";";
        savedString += "A" + (int) currentHat + ";";
        savedString += "O" + (int) currentColor + ";";
        Debug.Log("Saved string: " + savedString);
        return savedString;
    }

    /// <summary>
    /// Parse the string from the PlayerPrefs saving and set the values accordingly.
    /// </summary>
    public void ParseSavedString(string savedString) {
        int index, value;
        for (int i = 0; i < savedString.Length; i++) {
            switch ( savedString[i] ) {
                case 'H':
                    i++;
                    index = int.Parse(savedString[i].ToString());
                    i++;
                    value = int.Parse(savedString[i].ToString());
                    hatObjectList[index].SetUnlocked(value == 1 ? true : false);
                    i++;
                    break;
                case 'C':
                    i++;
                    index = int.Parse(savedString[i].ToString());
                    i++;
                    value = int.Parse(savedString[i].ToString());
                    colorObjectList[index].SetUnlocked(value == 1 ? true : false);
                    i++;
                    break;
                case 'P':
                    i++;
                    index = int.Parse(savedString[i].ToString());
                    i++;
                    value = int.Parse(savedString[i].ToString());
                    powerupObjectList[index].SetCurrentLevel(value);
                    break;
                case 'S':
                    i++;
                    highscore = int.Parse(savedString.Substring(i, highscoreLength));
                    i += highscoreLength;
                    break;
                case 'T':
                    i++;
                    totalScore = int.Parse(savedString.Substring(i, totalscoreLength));
                    i += totalscoreLength;
                    break;
                case 'A':
                    i++;
                    currentHat = (PlayerHatTypes) int.Parse(savedString[i].ToString());
                    break;
                case 'O':
                    i++;
                    currentColor = (PlayerColorTypes) int.Parse(savedString[i].ToString());
                    break;
            }
        }

        Debug.Log("Loaded string: " + savedString);
    }

    public void SetTotalScore( int totalScore ) {
        this.totalScore = totalScore;
    }

    public void SetHighscore( int highscore ) {
        this.highscore = highscore;
    }

    public PlayerHatTypes GetSelectedHatType() {
        return currentHat;
    }

    public PlayerColorTypes GetSelectedColorType() {
        return currentColor;
    }

    public List<PowerupObject> GetUnlockedPowerups() {
        return powerupObjectList;
    }

    public int GetTotalScore() {
        return totalScore;
    }
}
