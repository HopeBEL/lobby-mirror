using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILobby : MonoBehaviour
{
    public static UILobby instance;

    //Header permet d'ajouter un header dans l'inspecteur, juste pour lisibilité
    [Header("Host Join")]

    //SerializeField = Rendre variables private visibles dans l'inspecteur
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] Canvas lobbyCanvas;
    
    [Header("Lobby")]
    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] TMP_Text matchID_text;
    [SerializeField] GameObject startGame_button;



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

            SpawnPlayerUIPrefab(Player.localPlayer);
            matchID_text.text = Player.localPlayer.MatchID;
            startGame_button.SetActive(true);

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

        //Le matchID = ce que tape l'utilisateur dans l'input field
        Player.localPlayer.JoinGame(joinInput.text);
    }

    public void JoinSuccess (bool success) {
        if (success) {
            lobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(Player.localPlayer);
            matchID_text.text = Player.localPlayer.MatchID;

        }
        else {
            joinInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }


    public void SpawnPlayerUIPrefab(Player player) {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);

        //Pour que les joueurs s'affichent dans le bon ordre dans le lobby
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
    }

    public void StartGame() {
        Player.localPlayer.StartGame();
    }
}
