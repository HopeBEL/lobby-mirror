using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{  
    public static Player localPlayer;

    //Syncvar bc will be displayed on all version of this client
    [SyncVar]public string MatchID;

    //Utilisée dans UIPlayer pour l'affichage des "Player .." dans le lobby
    [SyncVar] public int playerIndex;
    NetworkMatch networkMatch;


    //Quand un autre joueur rejoint le lobby, Start est rappelée
    //Si ce n'est pas le localplayer, il va quand même appelée Spawn..
    void Start() {
        networkMatch = GetComponent<NetworkMatch>();

        if (isLocalPlayer) {
            localPlayer = this;
        }
        else {
            UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    public void HostGame() {
        string matchID = MatchMaker.GetRandomMatchId();
        CmdHostGame(matchID);

    }

    //Player dit au serveur d'exécuter cette fonction
    //Le serveur va ajouter cet id à la liste des matchs existant
    [Command] 
    void CmdHostGame(string matchID) {
        MatchID = matchID;
        //out directement fait ref à playerIndex
        if(MatchMaker.instance.HostGame(matchID, gameObject, out playerIndex)) {
            Debug.Log("Game hosted successfully !");
            networkMatch.matchId = matchID.ToGuid();
            TargetHostGame(true, matchID, playerIndex);
        }
        else 
            Debug.Log("Game hosted failed");
            TargetHostGame(false, matchID, playerIndex);
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetHostGame(bool success, string matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        Debug.Log("Match ID : " + matchID + " == " + MatchID);
        UILobby.instance.HostSuccess(success);
    }

    public void JoinGame(string matchID) {
        CmdJoinGame(matchID);
    }

    //Player dit au serveur d'exécuter cette fonction
    //Le serveur va ajouter cet id à la liste des matchs existant
    [Command] 
    void CmdJoinGame(string matchID) {
        MatchID = matchID;
        if(MatchMaker.instance.JoinGame(matchID, gameObject, out playerIndex)) {
            Debug.Log("Game joined successfully !");
            networkMatch.matchId = matchID.ToGuid();
            TargetJoinGame(true, matchID, playerIndex);
        }
        else 
            Debug.Log("Game joined failed");
            TargetJoinGame(false, matchID, playerIndex);
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetJoinGame(bool success, string matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        Debug.Log("Match ID : " + matchID + " == " + MatchID);
        UILobby.instance.JoinSuccess(success);
    }


    /* -------- */

     public void StartGame() {
        CmdStartGame();
    }

    //Player dit au serveur d'exécuter cette fonction
    //Le serveur va ajouter cet id à la liste des matchs existant
    [Command] 
    void CmdStartGame() {
        MatchMaker.instance.StartGame(MatchID);
        Debug.Log("Game Starting");
    }

    public void BeginGame() {
        TargetStartGame();
    }

    //Le client concerné appelle cette fonction
    //Pas clientrpc parce qu'on veut pas tous les clients qui load la scène
    [TargetRpc]
    void TargetStartGame() {
        Debug.Log("Match ID : " + MatchID + "beginning");
        //Load game scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
}
