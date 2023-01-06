using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Exceptions;
using Chess3D.Dependency;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
namespace Assets.Project.Chess3D
{
	using Proyecto26;
	using System.Text;

    public class GameUiController : MonoBehaviour
	{
		private readonly string updateMatchResultURL = "http://188.166.228.186:1337/api/user/update-match-status";

		public RectTransform WinnerFrame;
		public RectTransform LooserFrame;
		public RectTransform ChatPanel;
        public Text ErrorText;
        public Text InputInfoText;
        public Text SearchInfoText;
        public Button EndButton;

        public GameObject btnSave;
		bool chatpanelShow = true;
		public RectTransform backToBattlelabBtn;
        public GameObject gameOverWindow;
        public Text winnerText;
		public TextMeshProUGUI playerName;
		public TextMeshProUGUI OpponentName;
 		public TextMeshProUGUI winnerName;
		public TextMeshProUGUI looserName;
		public Image playerAvatar;
		public Image opponentAvatar;
		public Image winnerAvatar;
		public Image looserAvatar;
        //public GameObject btn_home;
        //public GameObject btn_restart;
        //public GameObject btn_resume;
		public GameObject btn_Settings;
		public GameObject btn_Chat;
		public GameObject PlayerPanel;
		public GameObject OpponentPanel;
	    
		public GameObject btn_chattab;
		public GameObject btn_emojitab;
		public GameObject chattabDetail;
		public GameObject emojiTabDetail;
        private void Start()
		{
        	
        	
            gameOverWindow.SetActive(false);

			EndButton.onClick.AddListener(ToMenu);
			playerName.text=PlayerPrefs.GetString("Player1Name","user1");
			OpponentName.text=PlayerPrefs.GetString("Player2Name","user2");
			StartCoroutine(LoadPlayerAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
			StartCoroutine(LoadOpponentAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));

            if (PlayerPrefs.GetInt("VsCPU", 1) == 1)
            {
                btnSave.SetActive(true);
               /* btn_home.SetActive(true);
                btn_restart.SetActive(true);
                btn_resume.SetActive(true);*/
            }
            else
            {
                btnSave.SetActive(false);
             /*   btn_home.SetActive(false);
                btn_restart.SetActive(false);
                btn_resume.SetActive(false);*/
            }
        }
        
        
        
		IEnumerator LoadPlayerAvatar(string url)
		{   
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

			DownloadHandler handle = www.downloadHandler;

			//Send Request and wait
			yield return www.SendWebRequest();

			if (www.isHttpError || www.isNetworkError)
			{
				UnityEngine.Debug.Log("Error while Receiving: " + www.error);
			}
			else
			{
				UnityEngine.Debug.Log("Success");

				//Load Image
				Texture2D texture2d = DownloadHandlerTexture.GetContent(www);

				Sprite sprite = null;
				sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);

				if (sprite != null)
				{
					playerAvatar.sprite = sprite;
				}
			}
		} 
		
		
		IEnumerator LoadLooserAvatar(string url)
		{   
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

			DownloadHandler handle = www.downloadHandler;

			//Send Request and wait
			yield return www.SendWebRequest();

			if (www.isHttpError || www.isNetworkError)
			{
				UnityEngine.Debug.Log("Error while Receiving: " + www.error);
			}
			else
			{
				UnityEngine.Debug.Log("Success");

				//Load Image
				Texture2D texture2d = DownloadHandlerTexture.GetContent(www);

				Sprite sprite = null;
				sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);

				if (sprite != null)
				{
					looserAvatar.sprite = sprite;
				}
			}
		} 
		
		
		
		IEnumerator LoadWinnerAvatar(string url)
		{   
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

			DownloadHandler handle = www.downloadHandler;

			//Send Request and wait
			yield return www.SendWebRequest();

			if (www.isHttpError || www.isNetworkError)
			{
				UnityEngine.Debug.Log("Error while Receiving: " + www.error);
			}
			else
			{
				UnityEngine.Debug.Log("Success");

				//Load Image
				Texture2D texture2d = DownloadHandlerTexture.GetContent(www);

				Sprite sprite = null;
				sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);

				if (sprite != null)
				{
					winnerAvatar.sprite = sprite;
				}
			}
		} 
		
		
		
		
		IEnumerator LoadOpponentAvatar(string url)
		{   
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

			DownloadHandler handle = www.downloadHandler;

			//Send Request and wait
			yield return www.SendWebRequest();

			if (www.isHttpError || www.isNetworkError)
			{
				UnityEngine.Debug.Log("Error while Receiving: " + www.error);
			}
			else
			{
				UnityEngine.Debug.Log("Success");

				//Load Image
				Texture2D texture2d = DownloadHandlerTexture.GetContent(www);

				Sprite sprite = null;
				sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);

				if (sprite != null)
				{
					opponentAvatar.sprite = sprite;
				}
			}
		} 
	
        

        public void ShowErrorText(string text)
        {
            ErrorText.text = text;
        }

        public void HideErrorText()
        {
            ErrorText.text = string.Empty;
        }

        public void ShowInputInfoText(string text)
        {
            InputInfoText.text = text;
        }

        public void HideInputInfoText()
        {
            InputInfoText.text = string.Empty;
        }

        public void ShowSearchInfoText(string text)
        {
            SearchInfoText.text = text;
        }

        public void HideSearchInfoText()
        {
            SearchInfoText.text = string.Empty;
        }

		public void ShowChatTabDetails()
		{
	    	
			emojiTabDetail.SetActive(false);	    
			chattabDetail.SetActive(true);	    
			btn_chattab.GetComponent<Image>().color = new UnityEngine.Color(btn_chattab.GetComponent<Image>().color.r, btn_chattab.GetComponent<Image>().color.g, btn_chattab.GetComponent<Image>().color.b, 1f);
			btn_emojitab.GetComponent<Image>().color = new UnityEngine.Color(btn_emojitab.GetComponent<Image>().color.r, btn_emojitab.GetComponent<Image>().color.g, btn_emojitab.GetComponent<Image>().color.b, .3f);

		}

		public void ShowEmojiTabDetails()
		{
			chattabDetail.SetActive(false);
			emojiTabDetail.SetActive(true);	  
		   
			btn_chattab.GetComponent<Image>().color = new UnityEngine.Color(btn_chattab.GetComponent<Image>().color.r, btn_chattab.GetComponent<Image>().color.g, btn_chattab.GetComponent<Image>().color.b, .3f);
			btn_emojitab.GetComponent<Image>().color = new UnityEngine.Color(btn_emojitab.GetComponent<Image>().color.r, btn_emojitab.GetComponent<Image>().color.g, btn_emojitab.GetComponent<Image>().color.b, 1f);

		}
		
		public class MatchResultdata
		{
			public int match_id;
			public int win_team_id;
			public int lose_team_id;

			public MatchResultdata(int match_id,int win_team_id,int lose_team_id)
			{
				this.match_id=match_id;
				this.win_team_id=win_team_id;
				this.lose_team_id=lose_team_id;
			}
		}
		
		IEnumerator MatchResultAPICAll(int matchID,int winnerId,int looserID)
		{
			MatchResultdata matchresultData=new MatchResultdata(matchID,winnerId,looserID);
			string json2=JsonUtility.ToJson(matchresultData);
			var request = new UnityWebRequest(updateMatchResultURL, "POST");
			byte[] bodyRaw = Encoding.UTF8.GetBytes(json2);
				
			request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();

			//        Debug.LogError("Data: "+request.downloadHandler.text);
			string data=request.downloadHandler.text;
			Debug.Log(data);

			if (request.isNetworkError)
			{
				Debug.Log("Error While Sending: " + request.error);
			}
			else
			{
				Debug.Log("Received: " + request.downloadHandler.text);
			}
		}

		public void EndGame(string winner)
		{
	    	
			OpponentPanel.SetActive(false);
			PlayerPanel.SetActive(false);

			btn_Settings.SetActive(false);
			btn_Chat.SetActive(false);
			if(winner=="WHITE")
			{
				StartCoroutine(LoadWinnerAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
				StartCoroutine(LoadLooserAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));
				winnerName.text = PlayerPrefs.GetString("Player1Name","winner");
				looserName.text = PlayerPrefs.GetString("Player2Name","winner");
			
				
				gameOverWindow.SetActive(true);
				winnerText.text = winner.ToUpper();
				LooserFrame.DOAnchorPos(new Vector2(330,45),.7f);
				WinnerFrame.DOAnchorPos(new Vector2(-280,52),.7f);
				backToBattlelabBtn.DOAnchorPos(new Vector2(0,100),.7f);
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team1id",0),PlayerPrefs.GetInt("team2id",0)));

			}
			
			else{
				
				StartCoroutine(LoadWinnerAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));
				StartCoroutine(LoadLooserAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
				winnerName.text = PlayerPrefs.GetString("Player2Name","winner");
				looserName.text = PlayerPrefs.GetString("Player1Name","winner");
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team2id",0),PlayerPrefs.GetInt("team1id",0)));

				
				gameOverWindow.SetActive(true);
				winnerText.text = winner.ToUpper();
				LooserFrame.DOAnchorPos(new Vector2(330,45),.7f);
				WinnerFrame.DOAnchorPos(new Vector2(-280,52),.7f);
				backToBattlelabBtn.DOAnchorPos(new Vector2(0,100),.7f);

			}
	
		 
			//InputInfoText.text = winner;
			//ErrorText.text = string.Empty;
			//SearchInfoText.text = string.Empty;
		}
		
		public void ShowChatPanel()
		{
	    	
			if(chatpanelShow)
			{
				ChatPanel.DOAnchorPos(new Vector2(187,0),.70f);
				chatpanelShow=false;
			}
			else{
	    		
	    		
				ChatPanel.DOAnchorPos(new Vector2(-207,0),.70f);
				chatpanelShow=true;

			}
		}

        public void ClearAll()
        {
            InputInfoText.text = string.Empty;
            ErrorText.text = string.Empty;
        }

        public void ToMenu()
        {
           
            SceneManager.LoadScene("MainMenu");


        }

		public void QuitGame(){
			Application.Quit();
		}
		
        public void Restart()
        {
          
        }
    }
}
