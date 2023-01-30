using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using TMPro;
using UnityEngine.SceneManagement;
public class MatchMakingHook : MonoBehaviour
{
	 
    // Start is called before the first frame update
    void Start()
    {
	    Invoke("LoadScene", 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
	
	public void LoadScene()
	{
		SceneManager.LoadScene("LoadData");

	}
	
 
	
	
	public void SetMatchID(int matchid){
		
		Debug.Log("Match ID: "+matchid.ToString());
		PlayerPrefs.SetInt("matchid", matchid);  
		PlayerPrefs.Save();  
	}
	
	public void SetTeam1ID(int team1id){
		Debug.Log("Team1ID: "+ team1id.ToString());
		PlayerPrefs.SetInt("team1id", team1id);  
		PlayerPrefs.Save();  

	}
	
	public void SetTeam2ID(int team2id){   
		Debug.Log("Team2ID: " + team2id.ToString());
		PlayerPrefs.SetInt("team2id", team2id);  
		PlayerPrefs.Save();  

	}
	
	
	public void SetUser1ID(int user1id){   
		Debug.Log("User1ID: " + user1id.ToString());
		PlayerPrefs.SetInt("user1id", user1id);  
		PlayerPrefs.Save();  

	}
	
	public void SetUser2ID(int user2id){   
		Debug.Log("User2ID: " + user2id.ToString());
		PlayerPrefs.SetInt("user2id", user2id);  
		PlayerPrefs.Save();  

	}
	
	
	public void SetCurrentPlayerTeamID(int currentplayerteamid){
		Debug.Log("CurrentPlayer Team ID: " + currentplayerteamid.ToString());
		PlayerPrefs.SetInt("currentplayerteamid", currentplayerteamid);  
		PlayerPrefs.Save();  

	}
	
	public void SetAccessToken(string accessToken){
		Debug.Log("Access Token: " + accessToken);
		PlayerPrefs.SetString("AccessToken", accessToken);  
		PlayerPrefs.Save();  

	}
	public void SetRoom(int roomMatchID){
		Debug.Log("Room: " + roomMatchID);
		PlayerPrefs.SetInt("roomMatchID", roomMatchID);  
		PlayerPrefs.Save();  

	}
}
