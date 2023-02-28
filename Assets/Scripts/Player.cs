using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{  
    public static Player localPlayer;

    //SyncVar : la variable va être mise à jour chez tous les clients
    [SyncVar]public string MatchID;

    //Utilisée dans UIPlayer pour l'affichage des "Player X" dans le lobby
    [SyncVar] public int playerIndex;
    NetworkMatch networkMatch;


    //Start est rappelée quand un autre joueur rejoint le lobby,
    //Si ce n'est pas le localplayer qui l'appelle, il va quand même appeler Spawn.. pour faire apparaître 
    //le prefab chez tous les joueurs
    void Start() {
        networkMatch = GetComponent<NetworkMatch>();

        if (isLocalPlayer) {
            localPlayer = this;
        }
        else {
            UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }


    /* ----- Héberger une partie ----- */

    //Récupère un identifiant random et le transmet au serveur
    public void HostGame() {
        string matchID = MatchMaker.GetRandomMatchId();
        CmdHostGame(matchID);

    }

    //Le client dit au serveur d'exécuter cette fonction
    //Le serveur va ajouter l'id en argument à la liste des matchs existant
    [Command] 
    void CmdHostGame(string matchID) {
        MatchID = matchID;
        //Si on a réussi à ajouter la nouvelle partie dans la liste de celles existant
        if(MatchMaker.instance.HostGame(matchID, gameObject, out playerIndex)) {
            Debug.Log("Game hosted successfully !");
            //Permet de grouper les joueurs dans des mêmes matchs selon l'identifiant
            networkMatch.matchId = matchID.ToGuid();
            //Le serveur dit au client concerné d'exécuter cette méthode
            TargetHostGame(true, matchID, playerIndex);
        }
        else 
            Debug.Log("Game hosted failed");
            TargetHostGame(false, matchID, playerIndex);
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetHostGame(bool success, string matchID, int _playerIndex) {
        //Mise à jour de l'index pour un bon affichage du nombre des joueurs
        playerIndex = _playerIndex;
        Debug.Log("Match ID : " + matchID + " == " + MatchID);
        UILobby.instance.HostSuccess(success);
    }

    /* ----- Rejoindre une partie ----- */

    //Le client dit au serveur d'exécuter cette méthode
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
            //Permet d'être groupé avec les joueurs ayant le même matchId
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
        //Mise à jour de l'index pour un bon affichage dans le lobby
        playerIndex = _playerIndex;
        Debug.Log("Match ID : " + matchID + " == " + MatchID);
        UILobby.instance.JoinSuccess(success);
    }


    /* ----- Lancer une partie ----- */

     public void StartGame() {
        CmdStartGame();
    }

    //Player dit au serveur d'exécuter cette fonction
    [Command] 
    void CmdStartGame() {
        MatchMaker.instance.StartGame(MatchID);
        Debug.Log("Game Starting");
    }

    //Fonction appelée par chaque joueur d'un même match
    public void BeginGame() {
        TargetStartGame();
    }

    //Le client concerné appelle cette fonction
    [TargetRpc]
    void TargetStartGame() {
        Debug.Log("Match ID : " + MatchID + "beginning");
        //Les joueurs concernés chargent la scène "Game"
        //La scène vient par dessus la scène Lobby, utile pour le retour au lobby plus tard
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
}
