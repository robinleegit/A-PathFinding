using UnityEngine;
using System.Collections;
/// <summary>
/// Tile
/// 
/// Extensibility:
/// 1. logic is decoupled from representation, according to MVC principal
/// 2. it is built for maze, but any map types, forrest, islands, dungeons... 
/// could work if gridified into 2D int[,] of walkables and non-walkables
/// 3. user could terrain type penalties
/// 4. user could customized heuristic function
/// 5. minor tweak (main in neighboring) is needed for hexagon grid based map
/// 
/// 
/// Robin Li
/// robinlee@cmu.edu
/// </summary>
public class Tile
{	
	public int x, y;
	// the position of tile

	public float g, h, f; 
	// the f_cost, g_cost and h_cost 
	// g_cost - traditional cost,  in this case ManhattenDistance(current, start) is used
	// h_cost - heuristic cost, in this case ManhattenDistance(current, target) is used
	// f_cost = g_cost + h_cost
	// user could redifine his own heuristic function

	public int flag;
	// 1 for unwalkable, 0 for walkable

	public Tile parent;
	// where this tile is coming from
	
	public Tile (int x, int y, int flag)
	{
		this.x = x;
		this.y = y;
		this.flag = flag;   
		this.parent = null;
		this.g = 999999;
		this.h = 999999;
		this.f = 999999;	
	}	
}
