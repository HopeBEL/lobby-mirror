using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;


//Serialization = pouvoir voir la classe et ses attributs dans l'inspecteur
[System.Serializable]
public class Match {
    public string matchID;
    //La liste contenant tous les joueurs d'un même match
    public List<GameObject> players = new List<GameObject>();

    //Constructeur, playerHost est le joueur qui crée le nouveau match et on l'ajoute à la liste
    public Match(string matchID, GameObject playerHost) {
        this.matchID = matchID;
        players.Add(playerHost);
    }

    //Besoin d'un constructeur vide pour que ce soit serializable
    public Match() {}
}

public class MatchMaker : NetworkBehaviour
{ 
    public static MatchMaker instance;
    //Liste contenant tous les matchs existants
    public SyncList<Match> matches = new SyncList<Match>();
    //Liste contenant uniquement les identifiants de tous les matchs existants
    public SyncList<string> matchIDs = new SyncList<string>();

    void Start() {
      instance = this;
    }

    //Static pour qu'on puisse y accéder dans d'autres fichiers, notamment Player.cs
    //Génère et renvoie un identifiant aléatoire
    public static string GetRandomMatchId() {
        string id_string = "";
        int random = UnityEngine.Random.Range(32, 122);
       
        id_string = random.ToString();

        Debug.Log("Match ID : " + id_string);
        return id_string;
    }


    //Ajoute un nouveau match
    public bool HostGame(string matchID, GameObject player, out int playerIndex) {
        playerIndex = -1;
        //Si l'identifiant en argument n'existe pas déjà dans la liste
        if (!matchIDs.Contains(matchID)) {
                //Alors on peut l'ajouter à la liste des matchs
                matchIDs.Add(matchID);
                matches.Add(new Match(matchID, player));
                Debug.Log("Match Generated");
                //Pour "Player 1"
                playerIndex = 1;
                return true; 
        }
        //Sinon c'est qu'il existe déjà
        Debug.Log("Match ID already exists");
        return false;
    }

    //Ajouter le joueur player au match existant avec l'identifiant matchId
    public bool JoinGame(string matchID, GameObject player, out int playerIndex) {
        playerIndex = -1;
        //On vérifie que le match existe bel et bien
        if (matchIDs.Contains(matchID)) {
            Debug.Log("Match joined");

            //Si oui on parcourt tous les match existant
            for (int i  = 0; i < matches.Count; i++) {
                //Quand on trouve notre match, on ajoute le joueur dedans et on sort
                if (matches[i].matchID == matchID) {
                    matches[i].players.Add(player);
                    playerIndex = matches[i].players.Count;
                    return true;
                }
            }
        }
        Debug.Log("Match ID does not exist");
        return false;
        
    }

    //Lancer la partie pour les joueurs d'un même match
    public void StartGame(string matchID) {
        //On parcourt la liste des matchs pour trouver celui en argument
        for (int i = 0; i < matches.Count; i++) {
            //Une fois trouvé
            if (matches[i].matchID == matchID) {
                //Pour chaque joueur de ce match, on leur demande de lancer la partie
                foreach (var player in matches[i].players) {
                    Player _player = player.GetComponent<Player>();
                    //Sur le serveur on dit à chacun de ses joueurs d'appeler
                    _player.BeginGame();
                }
                break;
            }
        }
    }

}


public static class MatchExtensions {
    public static Guid ToGuid (this string id) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
