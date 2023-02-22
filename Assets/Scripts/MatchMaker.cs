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
        //vérif que l'id n'existe pas déjà à faire
        matches.Add(new Match(matchID, player));
        return true;
        
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
