using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILobby : MonoBehaviour
{
    public static UILobby instance;
    //Rendre variables private visibles dans l'inspecteur
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] Canvas lobbyCanvas;
    

    void Start() {
        instance = this;
    }

    public void Host() {
        //Quand on a appuyé sur le bouton on rend tous ces champs non interagissables
        joinInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.HostGame();
    }

    public void HostSuccess(bool success) {
        if (success) {
            //On fait juste apparaitre un canva blanc pour l'instant -> ne fonctionne pas
            lobbyCanvas.enabled = true;
        }
        else {
            joinInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void Join() {
        //Quand on a appuyé sur le bouton on rend tous ces champs non interagissables
        
        joinInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
    }

    public void JoinSuccess (bool success) {
        if (success) {

        }
        else {
            joinInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }
}
