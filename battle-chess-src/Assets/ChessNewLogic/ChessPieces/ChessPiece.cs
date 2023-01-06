using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChessPieceType{
	None=0,
	Pawn2=1,
	Rook2=2,
	Knight2=3,
	Bishop2=4,
	Queen2=5,
	King2=6
}

public class ChessPiece : MonoBehaviour
{
    
	public int team;
	public int currentX;
	public int currentY;
	public ChessPieceType type;
	
	private Vector3 desiredPosition;
	private Vector3 desiredScale=Vector3.one;
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position,desiredPosition,Time.deltaTime*10);
		transform.localScale = Vector3.Lerp(transform.localScale,desiredScale,Time.deltaTime*10);
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	private void Start()
	{
		
		transform.rotation = Quaternion.Euler((team == 0)? new Vector3(0,90,0):new Vector3(0,-90,0));
 		
	}
	
	
	public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board,ref List<Vector2Int[]> moveList ,ref List<Vector2Int> availableMoves){
 		
	 
		
		return SpecialMove.None;
		
	}
	public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board,int tileCountX,int tileCountY){
		List<Vector2Int> r = new List<Vector2Int>();
		
		r.Add(new Vector2Int(3,3));
		r.Add(new Vector2Int(3,4));
		r.Add(new Vector2Int(4,3));
		r.Add(new Vector2Int(4,4));
		
		return r;
		
	}
	
	public virtual void SetPosition(Vector3 position, bool force = false)
	{
		desiredPosition = position;
		if (force)
		{
			transform.position = desiredPosition;
		}
	}
	
	public virtual void SetScale(Vector3 scale, bool force = false)
	{
		desiredScale = scale;
		if (force)
		{
			transform.localScale = desiredScale;
		}
	}
}
