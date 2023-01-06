using Assets.Project.Chess3D;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;
using System.Collections;

public class EventManager : MonoBehaviour
{


    //public Transform black_cam_pos;
    //public Transform white_cam_pos;

    //public enum GameType
    //{
    //    NONE,
    //    VSCPU,
    //    VSPLAYERS
    //}
	//public GameObject mainUI;

    //public GameObject animCamera;

    //public GameController gc;

    //private bool blocked = false;

    //SocketIOController socket;
    //public int myTurn = 0;

    //public GameType gameType = GameType.NONE;
    //public string turnOrder = "WB";

    //public GameObject waitingWindow;
    //public GameObject leftWindow;
    //public GameObject rightWindow;
    //public GameObject mainWindow;


    //string roomName;
    //string roomID;

    //public bool isEnableInput = false;


    //bool selectedAni = false;
    //void Start()
    //{
        
    //    gameType = (PlayerPrefs.GetInt("VsCPU", 1) != 1 ? GameType.VSPLAYERS : GameType.VSCPU);

    //    if (gameType == GameType.VSPLAYERS)
    //    {
    //        Debug.Log("MultiPlayer Mode");

    //        roomName = PlayerPrefs.GetString("RoomName");
    //        roomID = PlayerPrefs.GetString("RoomID");

    //        socket = SocketIOController.instance;
    //        socket.On("gameTurn", OnGetGameTurn);
    //        socket.On("other player turned", OnOtherPlayerTurned);
    //        socket.On("gave up", GaveUp);

	//        socket.Emit("joinRoom", JsonUtility.ToJson(new Room(roomID, roomID)));

	//        waitingWindow.SetActive(true);
	//        mainUI.SetActive(false);
	//        //leftWindow.SetActive(false);
    //        //rightWindow.SetActive(false);
    //        //mainWindow.SetActive(false);
    //        BlockEvents();
    //    }
        

	//    gc = GameObject.Find("GameController").transform.GetComponent<GameController>();
	//    //gc = GameObject.Find("GameController2").transform.GetComponent<GameController>();
    //    isEnableInput = true;
    //}

    //public void EnableInput()
    //{
    //    isEnableInput = true;
    //}

    //public void DisableInput()
    //{
    //    isEnableInput = false;
    //}
    //void FixedUpdate()
    //{
    //    if (blocked) return;
    //    if (!(gc.OnTurn is Human)) return;

    //    string strCurTurn = gc.Board.OnTurn.ToString();
    //    if (gameType == GameType.VSPLAYERS)
    //    {
    //        if (turnOrder[myTurn] != strCurTurn[0])
    //        {
    //            return;
    //        }
    //    }

    //    if (gc.SelectedPiece != null && !selectedAni)
    //    {
    //        gc.SelectedPieceAni();
    //        selectedAni = true;
    //    }

    //    if (Input.GetMouseButtonDown(0) && isEnableInput)
    //    {

    //        Debug.Log("Turn : " + gc.OnTurn.Id);
    //        Debug.Log(gc.Board.OnTurn.ToString());

            

    //        RaycastHit hit;
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        LayerMask mask = LayerMask.GetMask("Pieces");
    //        int sq64, sq120;
    //        print(gc.SelectedPiece);
    //        print(Physics.Raycast(ray, out hit, 100, mask) && gc.SelectedPiece == null);

    //        if (Physics.Raycast(ray, out hit, 100, mask) && gc.SelectedPiece == null)
    //        {
    //            sq64 = RaycastCell(ray);
    //            sq120 = Board.Sq120(sq64);
    //            gc.SelectPiece(sq120);

    //            PlayerTurn playerTurn = new PlayerTurn(0, sq120, myTurn);
    //            if (gameType == GameType.VSPLAYERS)
    //            {
    //                socket.Emit("click", JsonUtility.ToJson(playerTurn));
    //            }

    //            print(sq64);
    //            print(sq120);
    //            return;
    //        }
    //        else if (gc.SelectedPiece != null)
    //        {
    //            gc.DeselectedPieceAni();
    //            selectedAni = false;

    //            sq64 = RaycastCell(ray);
    //            if (gc.IsValidCell(sq64))
    //            {
    //                sq120 = Board.Sq120(sq64);
    //                gc.DoMove(sq120);


    //                PlayerTurn playerTurn = new PlayerTurn(1, sq120, myTurn);
    //                if (gameType == GameType.VSPLAYERS)
    //                {
    //                    socket.Emit("click", JsonUtility.ToJson(playerTurn));
    //                }

    //                Debug.Log(sq120);
    //            }
    //        }
    //    }
    //}

    //private int RaycastCell(Ray ray)
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 100))
    //    {
    //        Debug.Log(hit.point.ToString());
    //        Vector3 point = hit.point + new Vector3(-30f, 0, 30f);
    //        int i = (int)(-point.x / 7.5f);
    //        int j = (int)(point.z / 7.5f);
    //        return i * 8 + j;
    //    }
    //    return -1;
    //}

    //public void BlockEvents()
    //{
    //    blocked = true;
    //}

    //public void EnableEvents()
    //{
    //    blocked = false;
    //}


    //public void OnClickTest()
    //{
    //    gc.SelectPiece(34);
    //    //  gc.DoMove(44);
    //}

    //public void OnGetGameTurn(SocketIOEvent socketIOEvent)
    //{
    //    GameTurn turn = GameTurn.CreateFromJSON(socketIOEvent.data);
    //    Debug.Log("===================================");
    //    myTurn = turn.turn - 1;

    //    if (turnOrder[myTurn] == 'B')
    //    {
	//        //animCamera.SetTrigger("SwitchPlayerView");
	//         animCamera.transform.position = black_cam_pos.position;
	//        animCamera.transform.rotation = black_cam_pos.rotation;
	//        //animCamera.GetComponent<CameraMovement>().enabled = true;
        
    //        // StartCoroutine(iEndCam());
    //    }
    //    else
    //    {  
	//        //  animCamera.GetComponent<CameraMovement>().enabled = true;
    //    }

    //    if (turn.playing == 2)
    //    {
    //        waitingWindow.SetActive(false);
	//        mainUI.SetActive(true);

    //        //leftWindow.SetActive(true);
    //        //rightWindow.SetActive(true);
    //        //mainWindow.SetActive(true);

    //        EnableEvents();
    //        //obj_waiting.SetActive(false);
    //        //objTimer.SetActive(true);
    //        //isPlaying = true;
    //    }


    //    /* PlayerPrefs.SetInt("VsCPU", 0);
    //     PlayerPrefs.SetInt("GameTurn", turn.turn);*/

    //}


    //IEnumerator iEndCam()
    //{
    //    yield return new WaitForSeconds(1f);
    //    //animCamera.enabled = false;
    //}

    //void OnOtherPlayerTurned(SocketIOEvent socketIOEvent)
    //{

    //    string data = socketIOEvent.data.ToString();
    //    PlayerTurn turnJson = PlayerTurn.CreateFromJson(data);
    //    Debug.Log("other player turned = " + turnJson.index);

    //    if (turnJson.type == 0)
    //    {
    //        gc.SelectPiece(turnJson.index);
    //    }
    //    else
    //    {
    //        gc.DoMove(turnJson.index);
    //    }
        
    //}

    //public void GiveUp()
    //{
        
    //    if (gameType == GameType.VSPLAYERS)
    //    {
    //        socket.Emit("give up", JsonUtility.ToJson(new Room(myTurn.ToString(), roomID)));
    //    }

    //    string id = "";
    //    if (turnOrder[myTurn] == 'B')
    //    {
    //        id = "WHITE ";
    //    }
    //    else
    //    {
    //        id = "BLACK ";
    //    }
    //    gc.UiController.EndGame(id + " WINS.");
    //    BlockEvents();
    //}

    //void GaveUp(SocketIOEvent socketIOEvent)
    //{
    //    string res = socketIOEvent.data;
    //    int gavedTurn = 1;

    //    if (res.Contains("0"))
    //    {
    //        gavedTurn = 0;
    //    }

    //    string id = "";
    //    if (turnOrder[gavedTurn] == 'B')
    //    {
    //        id = "WHITE ";
    //    }
    //    else
    //    {
    //        id = "BLACK ";
    //    }
    //    gc.UiController.EndGame(id + " WINS.");
    //    BlockEvents();

    //}


    //public void CancelWaiting()
    //{
    //    if (PlayerPrefs.GetInt("Main") == 1)
    //    {
    //        socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID)));
    //        Destroy(socket.gameObject);
    //        SceneManager.LoadScene("MainMenu");
    //    }
    //    else
    //    {
    //        socket.Emit("leaveRoom");
    //        Destroy(socket.gameObject);
    //        SceneManager.LoadScene("MainMenu");
    //    }
    //}

}

//[Serializable]
//public class GameTurn
//{
//    public int turn;
//    public int playing;

//    public static GameTurn CreateFromJSON(string data)
//    {
//        return JsonUtility.FromJson<GameTurn>(data);
//    }
//}


//[Serializable]
//public class PlayerTurn
//{
//    public int type;
//    public int index;
//    public int turn;
//    public string roomId;

//    public PlayerTurn(int type, int index, int turn)
//    {
//        this.type = type;
//        this.index = index;
//        this.turn = turn;
//        this.roomId = PlayerPrefs.GetString("RoomID", "");
//    }
//    public static PlayerTurn CreateFromJson(string data)
//    {
//        return JsonUtility.FromJson<PlayerTurn>(data);
//    }
//}