using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all button events in a scene.
/// </summary>
public class InputManager : MonoBehaviour {

    public SnakeMovement snakeMovement;
    public SoundManager soundManager;

    public void OnPauseButtonClicked() {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        GameManager.instance.GamePaused();
    }

    public void OnRetryButtonPressed() {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        GameManager.instance.SwitchScreen( ScreenType.GAME );
    }

    public void OnShopButtonPressed( bool inMainMenu ) {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        if( inMainMenu ) {
            MainMenuManager.instance.SwitchScreen( ScreenType.SHOP_MENU );
        } else {
            GameManager.instance.SwitchScreen( ScreenType.SHOP_MENU );
        }
    }

    public void OnMainMenuButtonPressed() {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        GameManager.instance.SwitchScreen( ScreenType.MAIN_MENU );
    }
    
    public void OnPlayButtonPressed( bool inShopScreen ) {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        if( inShopScreen ) {
            ShopScreen.instance.ChangeScreen( (int) ScreenType.GAME );
        } else {
            MainMenuManager.instance.SwitchScreen( ScreenType.GAME );
        }
    }

    public void OnPurchaseableButtonPressed( int index ) {
        soundManager.PlaySound( SoundEffectType.SOUND_BUTTON, false );
        ShopScreen.instance.PurchaseableObjectSelected( index );
    }
    
    public void OnBuySelectPurchaseable() {
        soundManager.PlaySound( SoundEffectType.SOUND_BUY_SELECT, false );
        ShopScreen.instance.BuySelectPurchaseable();
    }

    public void OnShopSectionButtonPressed( int index ) {
        soundManager.PlaySound( SoundEffectType.SOUND_SECTION_SELECT, false );
        ShopScreen.instance.ShowSection( index );
    }
}
