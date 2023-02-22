using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class Match {
    public string matchID;
    public List<GameObject> players = new List<GameObject>();
    //Constructeur, playerHost est le joueur qui crée le nouveau match et on l'ajoute à la liste
    public Match(string matchID, GameObject playerHost) {
        this.matchID = matchID;
        players.Add(playerHost);
    }

    //Besoin d'un blank constructor pour que ce soit serializable ?
    public Match() {}
}

/* --- Deprecated
[System.Serializable]
//SyncLists are array based lists similar to C# List that synchronize 
//their contents from the server to the clients
//Liste les joueurs d'un même match
public class SyncListGameObject : SyncList<GameObject> {

}*/

[System.Serializable]
//Liste tous les matchs qui ont lieu en parallèle
public class SyncListMatch : SyncList<Match> {

}

public class MatchMaker : NetworkBehaviour
{ 
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();
    public SyncList<string> matchIDs = new SyncList<string>();

    [SerializeField] GameObject turnManagerPrefab;

    void Start() {
      instance = this;
    }

    //Static pour qu'on puisse y accéder dans Player.cs
    public static string GetRandomMatchId() {
        string id_string = "";
        int random = UnityEngine.Random.Range(32, 122);
       
        id_string = random.ToString();

        Debug.Log("Match ID : " + id_string);
        return id_string;
    }


    //out pe
    public bool HostGame(string matchID, GameObject player, out int playerIndex) {
        //On ajoute un nouveau match à la liste des matchs
        //Si cet ID n'est pas déjà dans la liste

        playerIndex = -1;
        if (!matchIDs.Contains(matchID)) {
                matchIDs.Add(matchID);
                matches.Add(new Match(matchID, player));
                Debug.Log("Match Generated");
                playerIndex = 1;
                return true; 
        }
        Debug.Log("Match ID already exists");
        return false;
    }

    //out permet de faire ref à une variable partout où cette fonction est appelée
    public bool JoinGame(string matchID, GameObject player, out int playerIndex) {
        playerIndex = -1;
        if (matchIDs.Contains(matchID)) {
            Debug.Log("Match joined");

            //On parcourt tous les match existant
            for (int i  = 0; i < matches.Count; i++) {
                //Quand on trouve notre match, on ajoute le joueur dedans et on break
                if (matches[i].matchID == matchID) {
                    matches[i].players.Add(player);
                    playerIndex = matches[i].players.Count;
                    break;
                }
            }
            return true;
        }
        Debug.Log("Match ID does not exist");
        return false;
        
    }

    public void StartGame(string matchID) {
        GameObject newTurnManager = Instantiate(turnManagerPrefab);
        NetworkServer.Spawn(newTurnManager);
        //Turnmanager will only apply to players who have the same matchId
        newTurnManager.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].matchID == matchID) {

                foreach (var player in matches[i].players) {
                    Player _player = player.GetComponent<Player>();
                    //TurnManager aura la liste de tous ses joueurs
                    turnManager.AddPlayer(_player);
                    //Sur le serveur on dit à chacun de ses joueurs d'appeler
                    _player.BeginGame();
                }
                break;
            }
        }
    }

}

 //wtf mais ok
    public static class MatchExtensions {
        public static Guid ToGuid (this string id) {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }
