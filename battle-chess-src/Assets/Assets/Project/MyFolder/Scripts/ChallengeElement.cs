using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;

public class ChallengeElement : MonoBehaviour
{
    // public Text no;
    public Text name;
    public long userId;
    public Text btnName;
    public Button btn;
    public string roomId;

    SocketIOController socket;

    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
    }

    public void SetProps(string no, long userId, string userName, string type = "CHALLENGE", string roomId = "")
    {
        //this.no.text = no;
        this.name.text = userName;
        this.userId = userId;
        this.btnName.text = type;
        this.roomId = roomId;

        // btn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = type;

        switch (type)
        {
            case "CHALLENGE":
                btn.onClick.AddListener(OnClickChallenge);
                break;
            case "ACCEPT":
                btn.onClick.AddListener(OnClickAccept);
                break;
            case "START":
                btn.onClick.AddListener(OnClickStart);
                break;
            case "WAITTING":
                btn.transform.gameObject.SetActive(false);
                this.name.text = userName + "(waitting...)";
                break;
        }

    }

    public void OnClickChallenge()
    {
        UserList userList = new UserList();
        userList.users = new List<User>();

        userList.users.Add(Global.m_user);
        userList.users.Add(new User(userId, name.text));

        socket.Emit("invite a challenge", JsonUtility.ToJson(userList));
        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        // socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID)));
    }

    public void OnClickAccept()
    {
        UserList userList = new UserList();
        userList.users = new List<User>();

        userList.users.Add(new User(userId, name.text));
        userList.users.Add(Global.m_user);

        socket.Emit("createChallenge", JsonUtility.ToJson(userList));
        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
    }

    public void OnClickStart()
    {
        PlayerPrefs.SetString("RoomName", "chllenge room");
        PlayerPrefs.SetString("RoomID", roomId);
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 0);

        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
