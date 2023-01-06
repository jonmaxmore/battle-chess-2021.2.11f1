using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop2 : ChessPiece
{
	public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board,int tileCountX,int tileCountY){
	
		List<Vector2Int> r = new List<Vector2Int>();
		
		//Top right 
		
		for (int x = currentX+1,y=currentY+1;  x < tileCountX && y<tileCountY; x++,y++) {
			
			if (board[x,y]==null)
			{
				r.Add(new Vector2Int(x,y));	

			}
			else
			{
				if (board[x,y].team !=team)
				
					r.Add(new Vector2Int(x,y));	

				break;
			}
			
		}
		
		// Change of  x>= 0 to x>0

		//Top left 
		
		for (int x = currentX-1,y=currentY+1;  x >0 && y<tileCountY; x--,y++) {
			
			if (board[x,y]==null)
			{
				r.Add(new Vector2Int(x,y));	

			}
			else
			{
				if (board[x,y].team !=team)
				
					r.Add(new Vector2Int(x,y));	

				break;
			}
			
		}
		
		//Bottom Right
		// Change of  y>= 0 to y>0

		for (int x = currentX+1,y=currentY-1;  x <tileCountX && y>0; x++,y--) {
			
			if (board[x,y]==null)
			{
				r.Add(new Vector2Int(x,y));	

			}
			else
			{
				if (board[x,y].team !=team)
				
					r.Add(new Vector2Int(x,y));	

				break;
			}
			
		}
		
		
		//Bottom left
		// Change of  x>= 0 y>=0 to x>0, y>0

		for (int x = currentX-1,y=currentY-1;  x >0 && y>0; x--,y--) {
			
			if (board[x,y]==null)
			{
				r.Add(new Vector2Int(x,y));	

			}
			else
			{
				if (board[x,y].team !=team)
				
					r.Add(new Vector2Int(x,y));	

				break;
			}
			
		}
 	
		return r;
		
	}
}
