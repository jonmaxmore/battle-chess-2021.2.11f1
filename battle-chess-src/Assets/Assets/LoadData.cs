using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
 using Models;
using Proyecto26;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;using TMPro;
using UnityEngine.SceneManagement;
using LitJson;
using System.Text;
using Newtonsoft.Json;
public class LoadData : MonoBehaviour
{
 	private RequestHelper currentRequest;



	public TextMeshProUGUI matchResult;
	//string userDetailsApi ="http://188.166.228.186:1337/api/user/comman-details/";
	string userDetailsApi ="http://13.228.113.125:1337/api/user/comman-details/";
	public string accessTokenfetched="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjozLCJlbWFpbCI6InVzZXIxQG1haWxpbmF0b3IuY29tIiwidXNlcl90eXBlIjoiZW5kdXNlciIsImlhdCI6MTY1MjMzNTc0OCwiZXhwIjoxNjgzODcxNzQ4fQ.1I4kUs9-7DRJ_mEEaVe6BmO42BvlL7idPDFjp3SWjhw";
	public int user1ID=0;
	public int user2ID=0				;
 
	// Start is called before the first frame update
	void Start()
	{
		accessTokenfetched=PlayerPrefs.GetString("AccessToken","a");
		user1ID=PlayerPrefs.GetInt("user1id",0);
		user2ID=PlayerPrefs.GetInt("user2id",0);
		Invoke("CurrentPlayerData", 2);
 	}

 

	public void CurrentPlayerData()
	{
			
  
	 
		//	SceneManager.LoadScene("MainMenu");


 
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer "+accessTokenfetched;

		RestClient.Get(userDetailsApi+user1ID).Then(response => {
			//EditorUtility.DisplayDialog("Response", response.Text, "Ok");
	
			Debug.Log(response.Text.ToString());
 			JsonData json = JsonMapper.ToObject(response.Text);
			String player1Name =	json["user"]["name"].ToString();
			String player1Avatar =	json["user"]["avatar_image"].ToString();
			String player1Email =	json["user"]["email"].ToString();
			
			Debug.Log("Player 1 Name: "+player1Name);
			
			PlayerPrefs.SetString("Player1Name",player1Name);
			PlayerPrefs.SetString("Player1Avatar",player1Avatar);
			PlayerPrefs.SetString("Player1Email",player1Email);
			PlayerPrefs.Save();
			
			
			RestClient.DefaultRequestHeaders["Authorization"] = "Bearer "+accessTokenfetched;

			RestClient.Get(userDetailsApi+user2ID).Then(response => {
				//EditorUtility.DisplayDialog("Response", response.Text, "Ok");
	
				Debug.Log(response.Text.ToString());
 				JsonData json2 = JsonMapper.ToObject(response.Text);

				String player2Name =	json2["user"]["name"].ToString();
				String player2Avatar =	json2["user"]["avatar_image"].ToString();
				String player2Email =	json2["user"]["email"].ToString();
				Debug.Log("Player 2 Name: "+player2Name);

				PlayerPrefs.SetString("Player2Name",player2Name);
				PlayerPrefs.SetString("Player2Avatar",player2Avatar);
				PlayerPrefs.SetString("Player2Email",player2Email);
				PlayerPrefs.Save();
				SceneManager.LoadScene("MainMenu");

			
			}).Catch(err => {
				var error = err as RequestException;
				//	EditorUtility.DisplayDialog("Error Response", error.Response, "Ok");
				Debug.Log(error.ToString());
 
			});
			
			
		}).Catch(err => {
			var error = err as RequestException;
			//	EditorUtility.DisplayDialog("Error Response", error.Response, "Ok");
			Debug.Log(error.ToString());
 
		});
			

	}
	
}
  






 
 


	
	
	 

