using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using LitJson;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MultiMenuManager : MonoBehaviour
{
    public GameObject RoomWindow;
    public GameObject ChallengeRoomWindow;
    // public GameObject ChallengePopupWindow;
    public GameObject CreateRoomWindow;

    public GameObject room_contents;
    public GameObject roomPrefab;
    SocketIOController socket;

    public InputField c_RoomName;


    public GameObject userContent;
    public GameObject userPrefab;

    public InputField roomSearchField;
    public InputField challengeSearchField;
	RoomItem roomItem;
    RoomList roomList;
    ChallengeList challengeList;
	MenuUiController menuUiController;
	bool clickedBell = false;
	public int roomMatchID=1;
	public string matchID = "";
	private int currentPlayer=0;
    // Start is called before the first frame update
    void Start()
    {
        clickedBell = false;

       /* RoomWindow.SetActive(true);
        ChallengeRoomWindow.SetActive(false);
        CreateRoomWindow.SetActive(false);*/

        socket = SocketIOController.instance;
	    socket.Connect();


        socket.On("show room", GetRooms);
        socket.On("createdRoom", OnCreatedRoom);
        socket.On("show users", GetUsers);
        socket.On("show challenges", GetChallenges);
	    matchID = PlayerPrefs.GetInt("matchid").ToString();
	    //matchID = "34161";
       // Global.m_user = new User(101,"qqq",103);
	    menuUiController = new MenuUiController();
	    StartCoroutine(iShowRooms());
	  
	    Debug.Log(socket.socketIO);
	    roomMatchID = PlayerPrefs.GetInt("roomMatchID");
	    //int roomMatchID =1;
	    
	    if(roomMatchID == 1)
	    {
		    
		    PlayerPrefs.SetInt("currentPlayer",0);
		    PlayerPrefs.Save();
		    
		    Invoke("CreateNewRoom",4f);   
		    

	    }
	    
	    else if(roomMatchID == 0)
	    {
	    	
		    PlayerPrefs.SetInt("currentPlayer",1);
		    PlayerPrefs.Save();
		    
		    Invoke("joinCreatedRoom",12f);       

	    }
	    
        //socket.Emit("get room list");

    }
    
    
	public void connectSocketwithdelay(){
		socket.Connect();
	}
    
    
	public void joinCreatedRoom(){
		PlayerPrefs.SetString("RoomName", matchID);
		PlayerPrefs.SetString("RoomID", matchID);
		PlayerPrefs.SetInt("VsCPU", 0);
		PlayerPrefs.SetInt("Main", 0);

		//SceneManager.LoadScene("Game");	
		SceneManager.LoadScene("GameNew");
	}
    
    
	public void SetupMultiplayerSettings(){
		//	menuUiController.DoAction();
		//	menuUiController.InitMuliplayerOptionButtons();
		menuUiController.OnHvsHClicked();
		//SceneManager.LoadScene("Game");
		SceneManager.LoadScene("GameNew");

	}



    public void LoadMainScene()
    {
        //SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        SceneManager.LoadScene("MainMenu");
    }

    void DisplayChallenges(string searchKey = "")
    {
        if (challengeList == null || userContent == null)
        {
            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        int index = 1;

        string lowerName = "";
        string lowerKey = searchKey.ToLower();
        

        foreach (Challenge challenge in challengeList.challenges)
        {

            temp = Instantiate(userPrefab) as GameObject;

            temp.transform.name = index.ToString();

            if (challenge.status == -1)
            {
                //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                //{
                //    continue;
                //}
                lowerName = challenge.toUserName;

                if (!lowerName.Contains(lowerKey))
                {
                    continue;
                }

                temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName);
            }
            else
            {
                if (challenge.fromUserId == Global.m_user.id)
                {
                    lowerName = challenge.toUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }

                    //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    if (challenge.status == 0)
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, "WAITTING");
                    }
                    else
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, "START", challenge.roomId);
                    }
                }
                else
                {
                    lowerName = challenge.fromUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }
                    //if (searchKey != "" && challenge.fromUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.fromUserId, challenge.fromUserName, "ACCEPT");
                }
            }

            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            index++;
        }

        clickedBell = false;
    }

    private void GetChallenges(SocketIOEvent socketIOEvent)
    {

        if (ChallengeRoomWindow == null || (!clickedBell && !ChallengeRoomWindow.transform.gameObject.activeInHierarchy))
        {
            
            return;
        }

        challengeList = ChallengeList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayChallenges();
    }

    public void OnChangedChallengeKey()
    {
        DisplayChallenges(challengeSearchField.text);
    }

    public void OnClickBell()
    {
        clickedBell = true;
        socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
    }

    IEnumerator iShowRooms()
    {
        
        yield return new WaitForSeconds(1.0f);
        
        if (Global.socketConnected)
        {
            Debug.Log("get room list");
            socket.Emit("get room list", JsonUtility.ToJson(Global.m_user));
        }
        else
        {
            iShowRooms();
        }

    }

    public void ActiveRooms()
    {
        Debug.Log("Clicked Active Rooms");
        StartCoroutine(iShowRooms());
    }
    public void OnCreatedRoom(SocketIOEvent socketIOEvent)
    {
        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());
        print("create Room");
        //RoomWindow.SetActive(false);
        //CreateRoomWindow.SetActive(false);
        //CreatedRoomPopup.SetActive(true);
        //CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room.name,room.id);

	    PlayerPrefs.SetString("RoomName", matchID);
	    PlayerPrefs.SetString("RoomID", matchID);
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 1);
	    //roomItem = new RoomItem();
	    
	    //roomItem.OnclickButtonJoin();
	    // SceneManager.LoadScene("Game");
	    SceneManager.LoadScene("GameNew");

	    //Invoke("SetupMultiplayerSettings",2);

    }

    // Update is called once per frame
    void Update()
    {

    }


	//public void CheckRoom()
	//{
	//	if (room_contents == null)
	//	{
        	
	//		return;
	//	}

       
	//	//foreach (Transform child in room_contents.transform)
	//	//{
	//	//    Destroy(child.gameObject);
	//	//}

	//	//GameObject temp;
	//	int index = 0;

	//	foreach (Room room in roomList.rooms)
	//	{

	//		 string lowerName = room.name.ToLower();
	//		 string lowerKey = searchKey.ToLower();
	//		string lowerName = room.name;
	//		string lowerKey = "321";

	//		Debug.Log("Lower Name : " + lowerName);
	//		Debug.Log("Lower Key : " + lowerKey);

	//		if (!lowerName.Contains(lowerKey))
	//		{
			
	//			continue;
	//		}
 
	//		Debug.Log("Search Key : " + "321");
	//		Debug.Log("Result of indexof : " + room.name.IndexOf("321", StringComparison.CurrentCultureIgnoreCase));
	//		Debug.Log("Created Room Name : " + room.name);

	//		index++;
	//		//temp = Instantiate(roomPrefab) as GameObject;
	//		//temp.transform.name = index.ToString();
	//		//temp.GetComponent<RoomItem>().SetProps(room.name, room.id);
	//		//temp.transform.SetParent(room_contents.transform);
	//		//temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

	//	}
	//}

    void DisplayRooms(string searchKey = "")
    {
        if (room_contents == null)
        {
        	
            return;
        }

       
        //foreach (Transform child in room_contents.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //GameObject temp;
        int index = 0;

        foreach (Room room in roomList.rooms)
        {

            //string lowerName = room.name.ToLower();
            //string lowerKey = searchKey.ToLower();
            string lowerName = room.name;
            string lowerKey = searchKey;

            Debug.Log("Lower Name : " + lowerName);
            Debug.Log("Lower Key : " + lowerKey);

            if (!lowerName.Contains(lowerKey))
            {
	             
                continue;
            }
 
            Debug.Log("Search Key : " + searchKey);
            Debug.Log("Result of indexof : " + room.name.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase));
            Debug.Log("Created Room Name : " + room.name);

            index++;
            //temp = Instantiate(roomPrefab) as GameObject;
            //temp.transform.name = index.ToString();
            //temp.GetComponent<RoomItem>().SetProps(room.name, room.id);
            //temp.transform.SetParent(room_contents.transform);
            //temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }
    }

    public void OnChangedRoomSearchKey()
    {
	    DisplayRooms("");
    }


    void GetRooms(SocketIOEvent socketIOEvent)
    {
        roomList = RoomList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayRooms();

        Debug.Log(roomList.rooms);
    }

    void GetUsers(SocketIOEvent socketIOEvent)
    {
        UserList userList = UserList.CreateFromJSON(socketIOEvent.data.ToString());

        if (userList == null || userContent == null)
        {

            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }


        GameObject temp;
        int index = 0;
        foreach (User user in userList.users)
        {
            if (user.id == Global.m_user.id)
            {
                continue;
            }

            index++;
            temp = Instantiate(userPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), user.id, user.name);
            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

    }
    public void OnClickCreateRoomButton()
    {
        c_RoomName.text = "";


        // CreateRoomWindow.SetActive(true);
        // RoomWindow.SetActive(false);
    }
    
    
	public void CreateNewRoom()
	{
		//string randomRoomname = System.Guid.NewGuid().ToString();
	    
		//PlayerPrefs.SetString("randomRoomname",randomRoomname);

		//PlayerPrefs.SetInt("matchid",0);
		//PlayerPrefs.Save();
		
		Debug.Log("in Room creation");

		if (Global.socketConnected)
		{
			socket.Emit("createRoom", JsonUtility.ToJson(new Room(matchID, matchID)));
		}
	}
    
    public void OnClickCreateButton()
    {
        if (c_RoomName.text == "")
            return;
        //CreateRoomWindow.SetActive(false);
        //CreatedRoomPopup.SetActive(true);
        //RoomWindow.SetActive(false);

        if (Global.socketConnected)
        {
            socket.Emit("createRoom", JsonUtility.ToJson(new Room(matchID, matchID)));
        }
        
    }

    public void OnClickChallengeButton()
    {
        socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        //socket.Emit("get user list", JsonUtility.ToJson(Global.m_user));
    }
}

[Serializable]
public class Room
{
    public string id;
    public string name;

    public static Room CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Room>(data);
    }
    public Room(string name, string id)
    {
        this.name = name;
        this.id = id;
    }

}

[Serializable]
public class RoomList
{

    public List<Room> rooms;

    public static RoomList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<RoomList>(data);
    }
}

[Serializable]
public class UserList
{

    public List<User> users;

    public static UserList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<UserList>(data);
    }
}

[Serializable]
public class Challenge
{
    public int fromUserId;
    public string fromUserName;
    public int toUserId;
    public string toUserName;
    public int status;
    public string roomId;

    Challenge(int fromUserId, string fromUserName, int toUserId, string toUserName, int status, string roomId)
    {
        this.fromUserId = fromUserId;
        this.fromUserName = fromUserName;
        this.toUserId = toUserId;
        this.toUserName = toUserName;
        this.status = status;
        this.roomId = roomId;
    }
}

[Serializable]
public class ChallengeList
{

    public List<Challenge> challenges;

    public static ChallengeList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ChallengeList>(data);
    }
}