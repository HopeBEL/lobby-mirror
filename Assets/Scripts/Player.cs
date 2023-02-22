using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{  
    public static Player localPlayer;

    //Syncvar bc will be displayed on all version of this client
    [SyncVar]public string MatchID;
    NetworkMatch networkMatch;
    void Start() {
        if (isLocalPlayer) {
            localPlayer = this;
        }

        networkMatch = GetComponent<NetworkMatch>();
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
        if(MatchMaker.instance.HostGame(matchID, gameObject)) {
            Debug.Log("Game hosted successfully !");
            networkMatch.matchId = matchID.ToGuid();
            TargetHostGame(true, matchID);
        }
        else 
            Debug.Log("Game hosted failed");
            TargetHostGame(false, matchID);
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetHostGame(bool success, string matchID) {
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
        if(MatchMaker.instance.JoinGame(matchID, gameObject)) {
            Debug.Log("Game joined successfully !");
            networkMatch.matchId = matchID.ToGuid();
            TargetJoinGame(true, matchID);
        }
        else 
            Debug.Log("Game joined failed");
            TargetJoinGame(false, matchID);
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetJoinGame(bool success, string matchID) {
        Debug.Log("Match ID : " + matchID + " == " + MatchID);
        UILobby.instance.JoinSuccess(success);
    }

}
