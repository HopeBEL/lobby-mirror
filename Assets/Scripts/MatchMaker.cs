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

    public bool HostGame(string matchID, GameObject player) {
        //On ajoute un nouveau match à la liste des matchs
        //Si cet ID n'est pas déjà dans la liste
        if (!matchIDs.Contains(matchID)) {
                matchIDs.Add(matchID);
                matches.Add(new Match(matchID, player));
                Debug.Log("Match Generated");
                return true; 
        }
        Debug.Log("Match ID already exists");
        return false;
    }

    public bool JoinGame(string matchID, GameObject player) {
        if (matchIDs.Contains(matchID)) {
            Debug.Log("Match joined");

            //On parcourt tous les match existant
            for (int i  = 0; i < matches.Count; i++) {
                //Quand on trouve notre match, on ajoute le joueur dedans et on break
                if (matches[i].matchID == matchID) {
                    matches[i].players.Add(player);
                    break;
                }
            }
            return true;
        }
        Debug.Log("Match ID does not exist");
        return false;
        
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
