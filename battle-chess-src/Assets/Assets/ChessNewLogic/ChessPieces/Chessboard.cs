using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Project.Chess3D;
using UnityEngine.UI;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DG.Tweening;
using System.Text;

public enum SpecialMove
{
	None=0,
	EnPassant,
	Castling,
	Promotion
}
public class Chessboard : MonoBehaviour
{
	[Header("Art Stuff")]
	[SerializeField] private Material tileMaterial;
	[SerializeField] private float tileSize = 1.0f;
	[SerializeField] private float yOffset = 0.2f;
	[SerializeField] private float dragOffset = 1f;
	[SerializeField] private Vector3 boardcenter = Vector3.zero;
	[SerializeField] private float deathSize = 0.3f;
	[SerializeField] private float deathSpacing = 0.3f;
	[SerializeField] public GameObject victoryScreen;
	[SerializeField] public GameObject directionLight;

	
	[Header("Prefabs & Materials")]
	[SerializeField]private GameObject[] prefabs;
	[SerializeField]private Material[] teamMaterials;

	//Logic
	
	private ChessPiece[,] chessPieces;
	private List<Vector2Int> availableMoves = new List<Vector2Int>();

	private ChessPiece currentlyDragging;
	private List<ChessPiece> deadWhites = new List<ChessPiece>();
	private List<ChessPiece> deadBlacks = new List<ChessPiece>();
	private const int TILE_COUNT_X = 8;
	private const int TILE_COUNT_Y = 8;
	private GameObject[,] tiles;
	private Camera currentCamera;
	private Vector2Int currentHover;
	private Vector3 bounds;
	private bool isWhiteTurn;
	private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
	private SpecialMove specialMove;
	
	//Multiplayer logic
	private int playerCount = -1;
	private int currentTeam = -1;
	
	
	
	//Old Game Logic
	public EventManager EventManager;
	SocketIOController socket;
	public int myTurn = 0;

	public GameType gameType = GameType.NONE;
	public string turnOrder = "WB";
	public GameObject mainUI;
	public GameObject waitingWindow;
	//public GameUiController UiController;

 

	//public IPlayer[] Players { get; private set; }
	//public IPlayer OnTurn { get; private set; }

	private bool blocked = false;

	string roomName;
	string roomID;

	public bool isEnableInput = false;
	bool isEnded;
	string msg = "";
	
	public GameObject white_turn_frame;
	public GameObject black_turn_frame;
	public TextMeshProUGUI white_time;
	public TextMeshProUGUI black_time;
        
 	    
	private float f_white_time;
	private float f_black_time;
	//private readonly string updateMatchResultURL = "http://188.166.228.186:1337/api/user/update-match-status";
	private readonly string updateMatchResultURL = "http://13.228.113.125:1337/api/user/update-match-status";

	
	public RectTransform WinnerFrame;
	public RectTransform LooserFrame;
	float dt = 0f;
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
        
	public enum GameType
	{
		NONE,
		VSCPU,
		VSPLAYERS
	}
 	    
	//private float f_white_time;
	//private float f_black_time;

	//float dt = 0f;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	private void Start()
	{
		isEnded = false;
		currentTeam=PlayerPrefs.GetInt("currentPlayer",0);
		//currentTeam=1;
		if (currentTeam==0)
		{
		
			playerName.text=PlayerPrefs.GetString("Player1Name","user1");
			StartCoroutine(LoadPlayerAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
			OpponentName.text=PlayerPrefs.GetString("Player2Name","user2");
			StartCoroutine(LoadOpponentAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));

		}
		
		else
		{   playerName.text=PlayerPrefs.GetString("Player1Name","user1");
			StartCoroutine(LoadPlayerAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
			OpponentName.text=PlayerPrefs.GetString("Player2Name","user2");
			StartCoroutine(LoadOpponentAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));
		}
		
		
		gameType = (PlayerPrefs.GetInt("VsCPU", 1) != 1 ? GameType.VSPLAYERS : GameType.VSCPU);

		if (gameType == GameType.VSPLAYERS)
		{
			Debug.Log("MultiPlayer Mode");

			roomName = PlayerPrefs.GetString("RoomName");
			roomID = PlayerPrefs.GetString("RoomID");

			socket = SocketIOController.instance;
			socket.On("gameTurn", OnGetGameTurn);
			socket.On("other player turned", OnOtherPlayerTurned);
			socket.On("gave up", GaveUp);

			socket.Emit("joinRoom", JsonUtility.ToJson(new Room(roomID, roomID)));

			waitingWindow.SetActive(true);
			mainUI.SetActive(false);
			//leftWindow.SetActive(false);
			//rightWindow.SetActive(false);
			//mainWindow.SetActive(false);
			BlockEvents();
		}
	 

		isWhiteTurn=true;
		GameUIControl.instance.ChangeCamera((currentTeam==0)? CameraAngle.whiteTeam : CameraAngle.blackTeam);
		if (currentTeam==1)
		{
			directionLight.gameObject.transform.transform.Rotate(55,138,50);
		}
		
		GenerateAllTiles(tileSize,TILE_COUNT_X,TILE_COUNT_Y);
		SpawnAllPieces();
		PositionAllPieces();

		white_turn_frame.GetComponent<Image>().enabled = false;
		black_turn_frame.GetComponent<Image>().enabled = false;

		////StartGame();
		f_black_time = 0f;
		f_white_time = 0f;
	}
	
	// Awake is called when the script instance is being loaded.
	public  void Awake()
	{
	
	}
	
	
	
	// Update is called once per frame
 private void Update()
	{
		if (!currentCamera)
		{
			currentCamera = Camera.main;
			return;
		}
		
	
		
		RaycastHit info;
		Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile","Hover","Highlight")))
		{
			//Get the indexes of tile we hit
			Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

			//If we are hovering any tile after not hovering any tile
			if (currentHover == -Vector2Int.one)
			{
				currentHover = hitPosition;
				tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}
			//if we were already hovernig a tile, change prevoius one
			if (currentHover != hitPosition)
			{
				tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves,currentHover))?LayerMask.NameToLayer("Highlight"):LayerMask.NameToLayer("Tile");;
				currentHover = hitPosition;
				tiles[currentHover.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}
			
			// if we press down the Mouse button
			if(Input.GetMouseButtonDown(0))
			{
				if (chessPieces[hitPosition.x,hitPosition.y]!=null)
				{
					
					//Is it our turn
					if ((chessPieces[hitPosition.x,hitPosition.y].team == 0 && isWhiteTurn && currentTeam==0)||(chessPieces[hitPosition.x,hitPosition.y].team == 1 && !isWhiteTurn&& currentTeam==1))
					{
					
						
						currentlyDragging = chessPieces[hitPosition.x,hitPosition.y];
						//Get List of where I can go and highlight the tiles
						availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces,TILE_COUNT_X,TILE_COUNT_Y);
						
						//Get the list of Special Moves
						specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces,ref moveList,ref availableMoves);
						
						//	PreventCheck();
						HighlightTiles();
						
					
						
						
						
					}
					//if ((chessPieces[hitPosition.x,hitPosition.y].team == 0 && isWhiteTurn && currentTeam==0))
					//{
					//	f_white_time += Time.deltaTime;
					//	white_time.text = GetTime((int)f_white_time);
					//	white_turn_frame.GetComponent<Image>().enabled = true;
					//	black_turn_frame.GetComponent<Image>().enabled = false;
						
					//	currentlyDragging = chessPieces[hitPosition.x,hitPosition.y];
					//	//Get List of where I can go and highlight the tiles
					//	availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces,TILE_COUNT_X,TILE_COUNT_Y);
						
					//	//Get the list of Special Moves
					//	specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces,ref moveList,ref availableMoves);
						
					//	HighlightTiles();
					//}
					
					//else if ((chessPieces[hitPosition.x,hitPosition.y].team == 1 && !isWhiteTurn&& currentTeam==1))
					//{
					//	f_black_time += Time.deltaTime;
					//	black_time.text = GetTime((int)f_black_time);
					//	white_turn_frame.GetComponent<Image>().enabled = false;
					//	black_turn_frame.GetComponent<Image>().enabled = true;
					//	currentlyDragging = chessPieces[hitPosition.x,hitPosition.y];
					//	//Get List of where I can go and highlight the tiles
					//	availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces,TILE_COUNT_X,TILE_COUNT_Y);
						
					//	//Get the list of Special Moves
					//	specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces,ref moveList,ref availableMoves);
						
					//	HighlightTiles();
					//}
					
					//dt = dt + Time.deltaTime;

				}
			}
			
			
			//if we releasing the mouse button
			if(currentlyDragging!=null && Input.GetMouseButtonUp(0))
			{
				Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

				if(ContainsValidMove(ref availableMoves, new Vector2Int(hitPosition.x,hitPosition.y)))
				{
				 
				 MoveTo(previousPosition.x,previousPosition.y,hitPosition.x,hitPosition.y);
				 //NodejS Server APIs calls here
				 
				 Vector2Int movedPosition = new Vector2Int(hitPosition.x, hitPosition.y);
					if (isWhiteTurn)
					{
						f_white_time += Time.deltaTime;
						white_time.text = GetTime((int)f_white_time);
						white_turn_frame.GetComponent<Image>().enabled = true;
						black_turn_frame.GetComponent<Image>().enabled = false;
					}
					else{
						f_white_time += Time.deltaTime;
						white_time.text = GetTime((int)f_white_time);
						white_turn_frame.GetComponent<Image>().enabled = false;
						black_turn_frame.GetComponent<Image>().enabled = true;
					}
				 PlayerTurn playerTurn = new PlayerTurn(1, previousPosition.x,previousPosition.y,hitPosition.x,hitPosition.y,currentTeam);
				 
					if (gameType == GameType.VSPLAYERS)
					{
						socket.Emit("click", JsonUtility.ToJson(playerTurn));
					}

				}
				
				
				else
				{
					currentlyDragging.SetPosition(GetTileCenter(previousPosition.x,previousPosition.y));
				
					currentlyDragging = null;
					RemoveHighlightTiles();
 
				}
				
 				
				
			}
		}
		else
		{
			if(currentHover != -Vector2Int.one)
			{
				tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves,currentHover))?LayerMask.NameToLayer("Highlight"):LayerMask.NameToLayer("Tile");
				currentHover = -Vector2Int.one;
			}
			
			if(currentlyDragging && Input.GetMouseButtonUp(0))
			{
				currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX,currentlyDragging.currentY));

				currentlyDragging = null;

				RemoveHighlightTiles();
				 
			}
		}
		
		//if we're dragging 
		
		if (currentlyDragging)
		{
			Plane horizontalPlane = new Plane(Vector3.up,Vector3.up*yOffset);
			float distance = 0.0f;
			if (horizontalPlane.Raycast(ray,out distance))
			{
				currentlyDragging.SetPosition(ray.GetPoint(distance)+Vector3.up * dragOffset);
			}
			
		}
	}

	private void MoveTo(int orignalX,int orignalY, int x, int y)
    {
		
	   
	    
	    Vector2Int previousPosition = new Vector2Int(orignalX, orignalY);
	    
	    ChessPiece cp = chessPieces[orignalX,orignalY];
		//if there is another piece on target position

		if(chessPieces[x,y]!=null)
        {
			ChessPiece ocp = chessPieces[x, y];
			if(cp.team == ocp.team)
				return;
            	
			//if its the enemy team
			if (ocp.team==0)
			{
				
				if (ocp.type == ChessPieceType.King2)
				{
					CheckMate(1);
				}
				deadWhites.Add(ocp);
				ocp.SetScale(Vector3.one*deathSize);
				ocp.SetPosition(new Vector3(8*tileSize,yOffset,-1*tileSize)
					-bounds
					+new Vector3(tileSize/2,0,tileSize/2)
					+(Vector3.forward*deathSpacing)*deadWhites.Count);
				
			}
			else
			
			{
				
				if (ocp.type == ChessPieceType.King2)
				{
					CheckMate(0);
				}
				
				deadBlacks.Add(ocp);
				ocp.SetScale(Vector3.one*deathSize);
				ocp.SetPosition(new Vector3(-1*tileSize,yOffset,8*tileSize)
					-bounds
					+new Vector3(tileSize/2,0,tileSize/2)
					+(Vector3.back*deathSpacing)*deadBlacks.Count);
			}
            
        }
        
	 
	   
		chessPieces[x, y] = cp;
		chessPieces[previousPosition.x, previousPosition.y] = null;
	    PositionSinglePiece(x, y);
		
	    isWhiteTurn = !isWhiteTurn;
	    
	    moveList.Add(new Vector2Int[]{previousPosition,new Vector2Int(x,y)});
		
	    ProcessSpecialMove();
	    
	    //if (CheckForCheckMate())
	    //{
	    //	CheckMate(cp.team);
	    //}
	    
	    if (currentlyDragging)
	    {
		    currentlyDragging = null;
	    }
	    RemoveHighlightTiles();
		
		return;

    }
	
	
	//*******CheckMate**************
	private void CheckMate(int team){
		PlayerPrefs.SetInt("winningTeam",team);
		PlayerPrefs.Save();
		EndGame(team);
	}
	
	private void DisplayVictory(int Winningteam)
	{
		victoryScreen.SetActive(true);
		victoryScreen.transform.GetChild(Winningteam).gameObject.SetActive(true);
	}
	
	public void OnExitbutton()
	
	{
		int resultUpdationStatus = PlayerPrefs.GetInt("matchResultUpdated",0);
		
		
		if (resultUpdationStatus == 1)
		{
			
			Application.Quit();
		
		}
		
		else
		{
			
			
			int winningTeam = 	PlayerPrefs.GetInt("winningTeam",0);
			if(winningTeam==0)
			{
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team1id",0),PlayerPrefs.GetInt("team2id",0)));
				Application.Quit();

			}
			
			else{
				
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team2id",0),PlayerPrefs.GetInt("team1id",0)));
				Application.Quit();


			}
			
			

		}

	}

	public void OnResetButton()
	{
		victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
		victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
		victoryScreen.SetActive(false);
		
		
		//Clear Fields
		
		currentlyDragging = null;
		availableMoves.Clear();
		moveList.Clear();
		//Clean up
		
		for (int x = 0; x < TILE_COUNT_X; x++) {
			for (int y = 0; y < TILE_COUNT_Y; y++) {
				
				if (chessPieces[x,y] !=null)
				{
					Destroy(chessPieces[x,y].gameObject);
				}
				chessPieces[x,y] = null;
			}
		}
		
		for (int i = 0; i < deadWhites.Count; i++) {
			Destroy(deadWhites[i].gameObject);
		}
		
		
		for (int i = 0; i < deadBlacks.Count; i++) {
			Destroy(deadBlacks[i].gameObject);
		}
		
		deadBlacks.Clear();
		deadWhites.Clear();
		
		SpawnAllPieces();
		PositionAllPieces();
		isWhiteTurn = true;

	}
	
	

	//************Highlight Tiles******************
	
	private void HighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++) {
			tiles[availableMoves[i].x,availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
		}
		
	}

	//************Remove Highlighted Tiles******************
	
	private void RemoveHighlightTiles()
	{
		for (int i = 0; i < availableMoves.Count; i++) 
			tiles[availableMoves[i].x,availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
			
			
		availableMoves.Clear();	
			
		
		
	}

	//**********Generating the Board*******************

	public void GenerateAllTiles(float tileSize,int tileCountX,int tileCountY){
		
		yOffset+= transform.position.y;
		bounds = new Vector3((tileCountX/2)*tileSize,0,(tileCountY/2)*tileSize)+boardcenter;
		tiles = new GameObject[tileCountX,tileCountY];
		for(int x=0; x<tileCountX;x++)
		
		for (int y = 0; y < tileCountY; y++) 
	   
				
			tiles[x,y]=GenerateSingleTile(tileSize,x,y);
		
		
	}
	
	public GameObject GenerateSingleTile(float tileSize,int x,int y){
	 
	 GameObject tileObject = new GameObject(string.Format("X:{0},Y:{1}",x,y));
		
		tileObject.transform.parent = transform;
		
		Mesh mesh = new Mesh();
		tileObject.AddComponent<MeshFilter>().mesh = mesh; 
		tileObject.AddComponent<MeshRenderer>().material = tileMaterial;
		
		Vector3[] vertices = new Vector3[4];
		
		vertices[0] =new Vector3(x*tileSize,yOffset,y*tileSize)-bounds;
		vertices[1] =new Vector3(x*tileSize,yOffset,(y+1)*tileSize)-bounds;
		vertices[2] =new Vector3((x+1)*tileSize,yOffset,y*tileSize)-bounds;
		vertices[3] =new Vector3((x+1)*tileSize,yOffset,(y+1)*tileSize)-bounds;
		
		int[] trices = new int[]{0,1,2,1,3,2};
		mesh.vertices = vertices;
		mesh.triangles= trices;
		mesh.RecalculateNormals();
		
		tileObject.layer = LayerMask.NameToLayer("Tile");
		tileObject.AddComponent<BoxCollider>();
		
		return tileObject;
	}


	//Special Moves
	private void ProcessSpecialMove()
	{
		
		//En Passant Special Move logic here**********************
		if (specialMove == SpecialMove.EnPassant)
		{
			var newMove = moveList[moveList.Count-1];
			ChessPiece myPawn = chessPieces[newMove[1].x,newMove[1].y];

			var targetPawnPostion = moveList[moveList.Count-2];
			ChessPiece enemyPawn = chessPieces[targetPawnPostion[1].x,targetPawnPostion[1].y];
			if (myPawn.currentX == enemyPawn.currentX)
			{
				if (myPawn.currentY == enemyPawn.currentY-1 || myPawn.currentY == enemyPawn.currentY+1)
				{
					if (enemyPawn.team==0)
					{
						deadWhites.Add(enemyPawn);
						enemyPawn.SetScale(Vector3.one*deathSize);
						enemyPawn.SetPosition(new Vector3(8*tileSize,yOffset,-1*tileSize)
							-bounds
							+new Vector3(tileSize/2,0,tileSize/2)
							+(Vector3.forward*deathSpacing)*deadWhites.Count);
					
					}
				
					else
					{
						deadBlacks.Add(enemyPawn);
						enemyPawn.SetScale(Vector3.one*deathSize);
						enemyPawn.SetPosition(new Vector3(8*tileSize,yOffset,-1*tileSize)
							-bounds
							+new Vector3(tileSize/2,0,tileSize/2)
							+(Vector3.forward*deathSpacing)*deadBlacks.Count);
					
					}
					
					chessPieces[enemyPawn.currentX,enemyPawn.currentY] = null;
				}
			}
		}
		
		//En Passant Special Move logic ends here**********************
		
		
		
		//Promotion Special Move logic here**********************
		if (specialMove == SpecialMove.Promotion)
		{
			
			Vector2Int[] lastMove = moveList[moveList.Count-1];
			ChessPiece targetPawn = chessPieces[lastMove[1].x,lastMove[1].y];
			
			if (targetPawn.type == ChessPieceType.Pawn2)
			{
				if (targetPawn.team==0 && lastMove[1].y == 7)
				{
					ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen2,0);
					
					newQueen.transform.position = chessPieces[lastMove[1].x,lastMove[1].y].transform.position;
					
					Destroy(chessPieces[lastMove[1].x,lastMove[1].y].gameObject);
					chessPieces[lastMove[1].x,lastMove[1].y] = newQueen;
					PositionSinglePiece(lastMove[1].x,lastMove[1].y);

				}
				
				if (targetPawn.team==1 && lastMove[1].y == 0)
				{
					ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen2,1);
					Destroy(chessPieces[lastMove[1].x,lastMove[1].y].gameObject);
					chessPieces[lastMove[1].x,lastMove[1].y] = newQueen;
					PositionSinglePiece(lastMove[1].x,lastMove[1].y,true);

				}
			}

		
		}
		//Promotion Special Move logic ends here**********************
		
		
		//Castling Special Move logic here**********************
		if (specialMove == SpecialMove.Castling)
		{
			
			Vector2Int[] lastMove = moveList[moveList.Count-1];
 
			//Left Rook
			
			if (lastMove[1].x==2)
			{
				if (lastMove[1].y==0)//White side
				{
					ChessPiece rook = chessPieces[0,0];
					chessPieces[3,0] =rook;
					PositionSinglePiece(3,0);
					chessPieces[0,0] =null;
				}
				else if (lastMove[1].y==7)//Black Side
				{
					ChessPiece rook = chessPieces[0,7];
					chessPieces[3,7] =rook;
					PositionSinglePiece(3,7);
					chessPieces[0,7] =null;
				}
			}
			
			//Right Rook
			else if (lastMove[1].x == 6)
			{
				if (lastMove[1].y==0)//White side
				{
					ChessPiece rook = chessPieces[7,0];
					chessPieces[5,0] =rook;
					PositionSinglePiece(5,0);
					chessPieces[7,0] =null;
				}
				else if (lastMove[1].y==7)//Black Side
				{
					ChessPiece rook = chessPieces[7,7];
					chessPieces[5,7] =rook;
					PositionSinglePiece(5,7);
					chessPieces[7,7] =null;
				}
			}
 
		}
		
		
		//Castling Special Move logic ends here**********************
	}
	
	private void PreventCheck()
	{
		ChessPiece targetKing = null;
		for (int x = 0; x < TILE_COUNT_X; x++)
			for (int y = 0; y < TILE_COUNT_Y; y++)
				if (chessPieces[x, y] != null)
					if (chessPieces[x, y].type == ChessPieceType.King2)
						if (chessPieces[x, y].team == currentlyDragging.team)
							targetKing = chessPieces[x, y];

                
					// Since we're sending ref asailablesMoves, we will be deleting moves that are putting us in check
		SimulateMoveForSinglePiece(currentlyDragging,
			ref availableMoves,
			targetKing);
	}
	
	
	private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
	
	{
		
		// Save the Current values, to reset the function call
		int actualX = cp.currentX;
		int actualY = cp.currentY;
		List<Vector2Int> movesToRemove = new List<Vector2Int>();

		// Going through all the moves, simualte them and check if we're in check
		for (int i = 0; i < moves.Count; i++)
		{
			int simX = moves[i].x;
			int simY = moves[i].y;

			Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
			// Did we simulate the king's move
			if (cp.type == ChessPieceType.King2)
				kingPositionThisSim = new Vector2Int(simX, simY);

			// Copy the [,] and not a reference
			ChessPiece[,] simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
			List<ChessPiece> simAttackingPieces = new List<ChessPiece>();
			for (int x = 0; x < TILE_COUNT_X; x++)
			{
				for (int y = 0; y < TILE_COUNT_Y; y++)
				{
					if (chessPieces[x, y] != null)
					{
						simulation[x, y] = chessPieces[x, y];
						if (simulation[x, y].team != cp.team)
							simAttackingPieces.Add(simulation[x, y]);
					}
				}
			}

			// Simulate that move
			simulation[actualX, actualY] = null;
			cp.currentX = simX;
			cp.currentX = simY;
			simulation[simX, simY] = cp;

			// Did one of the piece got taken down during our simulation
			var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simX);
			if (deadPiece != null)
				simAttackingPieces.Remove(deadPiece);

			//Get all the Ssimulated attacking pieces moves
			List<Vector2Int> simMoves = new List<Vector2Int>();
			for (int a = 0; a < simAttackingPieces.Count; a++)
			{
				var pieceMoves = simAttackingPieces[a].GetAvailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
				for (int b = 0; b < pieceMoves.Count; b++)
					simMoves.Add(pieceMoves[b]);
			}

			// Is the king in trouble? if so, remove the move
			if (ContainsValidMove(ref simMoves, kingPositionThisSim))
			{
				movesToRemove.Add(moves[i]);
			}

			// Restore the actual CP data
			cp.currentX = actualX;
			cp.currentY = actualY;
		}

		// Remove from the current available move list
		for (int i = 0; i < movesToRemove.Count; i++)
			moves.Remove(movesToRemove[i]);
	}


	private bool CheckForCheckMate()
	{
		
		var lastMove = moveList[moveList.Count-1];
		int targetTeam = (chessPieces[lastMove[0].x,lastMove[0].y].team==0)?1:0;
		
		List<ChessPiece> attackingPieces = new List<ChessPiece>();
		List<ChessPiece> defendingPieces = new List<ChessPiece>();
		
		ChessPiece targetKing = null;
		for (int x = 0; x < TILE_COUNT_X; x++)
			for (int y = 0; y < TILE_COUNT_Y; y++)
				if (chessPieces[x, y] != null)
				{
					if (chessPieces[x, y].team == targetTeam)
					{
						defendingPieces.Add(chessPieces[x,y]);
						if (chessPieces[x,y].type==ChessPieceType.King2)
						{
							targetKing=chessPieces[x,y];
						}
					}
					else
					{
						attackingPieces.Add(chessPieces[x,y]);

					}

				}
				
		
		
		//Is  the King Attacked right now?
		
		List<Vector2Int> currentlyAvailableMoves = new List<Vector2Int>();
		for (int i = 0; i <attackingPieces.Count; i++) {
			
			var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
			for (int b = 0; b < pieceMoves.Count; b++)
				currentlyAvailableMoves.Add(pieceMoves[b]);
		}
		
		
		//Are we in Check Right now
		if (ContainsValidMove(ref currentlyAvailableMoves,new Vector2Int(targetKing.currentX,targetKing.currentY)))
		{
			
			//King is under attack can we move something to help him
			
			for (int i = 0; i < defendingPieces.Count; i++) {
				
				List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
				SimulateMoveForSinglePiece(defendingPieces[i],
					ref defendingMoves,
					targetKing);
					
				if (defendingMoves.Count!=0)
				{
					return false;
				}	
				
			}
			
			return true;//CheckMate Exit here
			
		}
		
		return false;
					 
		
	}
	
	
	
	

	//Spawning of Pieces
	
	private void SpawnAllPieces(){
		
		chessPieces = new ChessPiece[TILE_COUNT_X,TILE_COUNT_Y];
		
		int whiteTeam = 0; int blackTeam = 1;
		
		
		//White Team
		chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook2,whiteTeam);
		chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight2,whiteTeam);
		chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop2,whiteTeam);
		chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen2,whiteTeam);
		chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King2,whiteTeam);
		chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop2,whiteTeam);
		chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight2,whiteTeam);
		chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook2,whiteTeam);
		
		for (int i = 0; i < TILE_COUNT_X; i++) 
		chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn2,whiteTeam);
			
			
		//Black Team
		chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook2,blackTeam);
		chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight2,blackTeam);
		chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop2,blackTeam);
		chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen2,blackTeam);
		chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King2,blackTeam);
		chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop2,blackTeam);
		chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight2,blackTeam);
		chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook2,blackTeam);
		
		for (int i = 0; i < TILE_COUNT_X; i++) 
		chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Pawn2,blackTeam);
			
			
	}
	
	
	//**************Positioning All Pieces******************


	private void PositionAllPieces(){
		
		for (int x = 0; x < TILE_COUNT_X; x++) 
			for (int y = 0; y < TILE_COUNT_Y; y++) 
				if (chessPieces[x,y]!=null)
					PositionSinglePiece(x,y,true);
					
				
			
		
	}
	
	
	private void PositionSinglePiece(int x,int y, bool force= false)
	{
		
		chessPieces[x,y].currentX = x;
		chessPieces[x,y].currentY = y;
		chessPieces[x,y].SetPosition(GetTileCenter(x,y),force);
	}
	
	
	private bool ContainsValidMove(ref List<Vector2Int> moves , Vector2Int pos)
	{
		for (int i = 0; i < moves.Count; i++) {
			if (moves[i].x == pos.x && moves[i].y == pos.y)
			{
				return true;
			}
			
		}
		
		return false;

	}
	
	private Vector3 GetTileCenter(int x,int y)
	{
		return new Vector3(x*tileSize,0,y*tileSize)-bounds+new Vector3(tileSize/2,0,tileSize/2);
	}
	
	
	
	private ChessPiece SpawnSinglePiece(ChessPieceType type,int team){
		
		ChessPiece cp = Instantiate(prefabs[(int) type -1],transform).GetComponent<ChessPiece>();
		
		cp.type = type;
		cp.team = team;
		cp.GetComponent<MeshRenderer>().material = teamMaterials[((team==0)?0:6)+((int)type -1)];
		return cp;
	}


	

	private Vector2Int LookupTileIndex(GameObject hitInfo)
	{
		for (int x = 0; x < TILE_COUNT_X; x++) 
			for (int y = 0; y < TILE_COUNT_Y; y++) 
				if (tiles[x,y]==hitInfo)
				 	return new Vector2Int(x,y);
				 
		 
			
		return -Vector2Int.one;//invalid
	 
	}
	
	
	//Old Chess Logic 
	
	public void BlockEvents()
	{
		blocked = true;
	}

	public void EnableEvents()
	{
		blocked = false;
	}
	
	public void OnGetGameTurn(SocketIOEvent socketIOEvent)
	{
		GameTurn turn = GameTurn.CreateFromJSON(socketIOEvent.data);
		Debug.Log("===================================");
		myTurn = currentTeam;
		//myTurn = turn.turn - 1;

		if (turnOrder[myTurn] == 'B')
		{
			//animCamera.SetTrigger("SwitchPlayerView");
			//	animCamera.transform.position = black_cam_pos.position;
			//animCamera.transform.rotation = black_cam_pos.rotation;
			//animCamera.GetComponent<CameraMovement>().enabled = true;
        
			// StartCoroutine(iEndCam());
		}
		else
		{  
			//  animCamera.GetComponent<CameraMovement>().enabled = true;
		}

		if (turn.playing == 2)
		{
			waitingWindow.SetActive(false);
			mainUI.SetActive(true);

			//leftWindow.SetActive(true);
			//rightWindow.SetActive(true);
			//mainWindow.SetActive(true);

			EnableEvents();
			//obj_waiting.SetActive(false);
			//objTimer.SetActive(true);
			//isPlaying = true;
		}


		/* PlayerPrefs.SetInt("VsCPU", 0);
		PlayerPrefs.SetInt("GameTurn", turn.turn);*/

	}
	
	void OnOtherPlayerTurned(SocketIOEvent socketIOEvent)
	{

		string data = socketIOEvent.data.ToString();
		PlayerTurn turnJson = PlayerTurn.CreateFromJson(data);
		Debug.Log("other player turned = " + turnJson.destinationX+ turnJson.destinationY);

	 

			if (turnJson.turn!=currentTeam)
			{
				ChessPiece target = chessPieces[turnJson.originalX,turnJson.originalY];
				availableMoves = target.GetAvailableMoves(ref chessPieces,TILE_COUNT_X,TILE_COUNT_Y);
				specialMove=target.GetSpecialMoves(ref chessPieces,ref moveList,ref availableMoves);
				MoveTo(turnJson.originalX,turnJson.originalY,turnJson.destinationX,turnJson.destinationY);
			}
			

	 
        
	}
	public void GiveUp()
	{
        
		if (gameType == GameType.VSPLAYERS)
		{
			socket.Emit("give up", JsonUtility.ToJson(new Room(myTurn.ToString(), roomID)));
		}

		string id = "";
		if (turnOrder[myTurn] == 'B')
		{
			id = "WHITE ";
			CheckMate(0);
 		BlockEvents();

		}
		else
		{
			id = "BLACK ";
			CheckMate(1);
 		BlockEvents();

		}
 	}

	void GaveUp(SocketIOEvent socketIOEvent)
	{
		string res = socketIOEvent.data;
		int gavedTurn = 1;

		if (res.Contains("0"))
		{
			gavedTurn = 0;
		}

		string id = "";
		if (turnOrder[gavedTurn] == 'B')
		{
			id = "WHITE ";
			CheckMate(0);
			BlockEvents();


		}
		else
		{
			id = "BLACK ";
			CheckMate(1);
			BlockEvents();


		}

	}


	public void CancelWaiting()
	{
		if (PlayerPrefs.GetInt("Main") == 1)
		{
			socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID)));
			Destroy(socket.gameObject);
			SceneManager.LoadScene("MainMenu");
		}
		else
		{
			socket.Emit("leaveRoom");
			Destroy(socket.gameObject);
			SceneManager.LoadScene("MainMenu");
		}
	}
	
	public string GetTime(int t)
	{
		string rt = "";
		rt = (t / 59).ToString("00") + ":" + (t % 59).ToString("00");

		return rt;

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
			PlayerPrefs.SetInt("matchResultUpdated",0);
			PlayerPrefs.Save();
			int winningTeamz = 	PlayerPrefs.GetInt("winningTeam",0);
			if(winningTeamz==0)
			{
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team1id",0),PlayerPrefs.GetInt("team2id",0)));
 
			}
			
			else{
				
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team2id",0),PlayerPrefs.GetInt("team1id",0)));
 

			}
			
		}
		
		else if(request.isNetworkError)
			
		{
			Debug.Log("Error While Sending: " + request.error);
			int winningTeam = 	PlayerPrefs.GetInt("winningTeam",0);
			if(winningTeam==0)
			{
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team1id",0),PlayerPrefs.GetInt("team2id",0)));
 
			}
			
			else{
				
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team2id",0),PlayerPrefs.GetInt("team1id",0)));
 

			}
			PlayerPrefs.SetInt("matchResultUpdated",0);
			PlayerPrefs.Save();
			
		}
		else if(request.isHttpError)
			
		{
			Debug.Log("Error While Sending: " + request.error);
			int winningTeam = 	PlayerPrefs.GetInt("winningTeam",0);
			if(winningTeam==0)
			{
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team1id",0),PlayerPrefs.GetInt("team2id",0)));
 
			}
			
			else{
				
		 
				StartCoroutine(MatchResultAPICAll(PlayerPrefs.GetInt("matchid",0),PlayerPrefs.GetInt("team2id",0),PlayerPrefs.GetInt("team1id",0)));
 

			}
			PlayerPrefs.SetInt("matchResultUpdated",0);
			PlayerPrefs.Save();
			
		}
	 
	 
		else
		{
			Debug.Log("Received: " + request.downloadHandler.text);
			PlayerPrefs.SetInt("matchResultUpdated",1);
			PlayerPrefs.Save();
			
		}
	}




	public void EndGame(int winnerTeam)
	{

		PlayerPrefs.SetInt("winningTeam",winnerTeam);
		PlayerPrefs.Save();
	    	
	    	
		OpponentPanel.SetActive(false);
		PlayerPanel.SetActive(false);

	 
		if(winnerTeam==0)
		{
			StartCoroutine(LoadWinnerAvatar(PlayerPrefs.GetString("Player1Avatar","avatar")));
			StartCoroutine(LoadLooserAvatar(PlayerPrefs.GetString("Player2Avatar","avatar")));
			winnerName.text = PlayerPrefs.GetString("Player1Name","winner");
			looserName.text = PlayerPrefs.GetString("Player2Name","winner");
				
			gameOverWindow.SetActive(true);
			//	winnerText.text = winner.ToUpper();
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
			//winnerText.text = winner.ToUpper();
			LooserFrame.DOAnchorPos(new Vector2(330,45),.7f);
			WinnerFrame.DOAnchorPos(new Vector2(-280,52),.7f);
			backToBattlelabBtn.DOAnchorPos(new Vector2(0,100),.7f);

		}
	
		 
		//InputInfoText.text = winner;
		//ErrorText.text = string.Empty;
		//SearchInfoText.text = string.Empty;
	}
	

  
}

[Serializable]
public class GameTurn
{
	public int turn;
	public int playing;

	public static GameTurn CreateFromJSON(string data)
	{
		return JsonUtility.FromJson<GameTurn>(data);
	}
}


[Serializable]
public class PlayerTurn
{
	public int type;
	public int originalX;
	public int originalY;
	public int destinationX;
	public int destinationY;
	public int turn;
	public string roomId;

	public PlayerTurn(int type, int originalX,int originalY,int destinationX,int destinationY, int turn)
	{
		this.type = type;
		this.originalX = originalX;
		this.originalY = originalY;
		this.destinationX = destinationX;
		this.destinationY = destinationY;
		this.turn = turn;
		this.roomId = PlayerPrefs.GetString("RoomID", "");
	}
	public static PlayerTurn CreateFromJson(string data)
	{
		return JsonUtility.FromJson<PlayerTurn>(data);
	}
}
