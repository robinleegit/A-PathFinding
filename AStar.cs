/// <summary>
/// 
/// A* PathFinding
/// For maze, tile based map
/// Works for procedural cave/forest using Cellular Automaton too
/// Given a 2D (3D needs modification) array of walkable/inwalkable tiles,
/// return the shortest path tiles from start to target
/// Heuristics: Manhattan Distance to Target
/// User could define his own heuristic in different setting
/// 
/// Astar outperforms dijkstra and BFS because of the help of heuristic
/// Astar may not outperform concurrent dijkstra, which involves multi-threading and mutex
/// 4-neighboring, no diagnal crossing
/// 
/// Robin Li
/// robinlee@cmu.edu
/// 
///  Extensibility:
/// 1. logic is decoupled from representation, according to MVC principal
/// 2. it is built for maze, but any map types, forrest, islands, dungeons... 
/// could work if gridified into 2D int[,] of walkables and non-walkables
/// 3. user could terrain type penalties
/// 4. user could customized heuristic function
/// 5. minor tweak (main in neighboring) is needed for hexagon grid based map
/// 
/// </summary>

 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour
{
	
	
		Tile[,] tiles;
		List<Tile> open;
		List<Tile> closed;
		int[][] offsets = new int[4][];
		public List<Tile> shortestPath;
	
		void Awake ()
		{
				//Initialize the 4-neighbour offsets
				offsets [0] = new int[2]{0,-1};
				offsets [1] = new int[2]{-1,0};
				offsets [2] = new int[2]{0,1};
				offsets [3] = new int[2]{1,0};
		}
	
		void Start ()
		{	
				//pass in the int[,] maze
				ConstructTiles (MazeGenerater.maze);

				//find the shortest path from left down corner and top right corner of the maze
				UpdateShortestPath (1, 1, MazeGenerater.maze.GetUpperBound (0) - 1, MazeGenerater.maze.GetUpperBound (1) - 1);
		
		}
	
		void Update ()
		{
		
				// update the shortest path for testing :D
				if (Input.GetKeyDown (KeyCode.U)) {
				
				// UpdateShortestPath (2, 1, MazeGenerater.maze.GetUpperBound (0) - 1, MazeGenerater.maze.GetUpperBound (1) - 1);
			
				}
		}


		/// <summary>
		/// This is INTERFACE for other class to call when wish to update the shortestpath
		/// given start and end tile, return the path. 
		/// </summary>
		/// <returns>The shortest path, in list of tiles</returns>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public List<Tile> FindPath (Tile start, Tile end)
		{
				// First clear previous parenting info and g, h, f values.
				for (int  i = 0; i <= tiles.GetUpperBound(0); i++) {
						for (int j = 0; j<= tiles.GetUpperBound(1); j++) {
								tiles [i, j].parent = null;
								tiles [i, j].g = 99999;
								tiles [i, j].h = 99999;
								tiles [i, j].f = 99999;	
						}
				}

				List<Tile> path = new List<Tile> ();
		
				closed = new List<Tile> ();  
				// The closedSet, or 'explored', is the set with known shorstest distance from start
				open = new List<Tile> ();   
				// The openSet or 'the frontier', is the tiles to be evaluated the next interation
				// For each iteration, move the one with lowest f_cost from openSet to closedSet
		
				int x1 = start.x;
				int y1 = start.y;
		
				// Initialize before the main Searching loop
				tiles [x1, y1].h = ComputeManhattan (tiles [x1, y1], end);
				tiles [x1, y1].g = 0;
				tiles [x1, y1].f = tiles [x1, y1].h + tiles [x1, y1].g;
				open.Add (tiles [x1, y1]);

				// Main loop for Heuristic-based search. 
				while (open.Count>0) {

						Tile current = GetLowestF (open);  // move the one with lowest f_cost from openSet to closedSet
						open.Remove (current);
						closed.Add (current);
						if (current == end) {
								Tile temp = current;
								// Reconstruct the path according to parenting info.
								while (temp != null) {
					
										path.Add (temp);
										temp = temp.parent;
								}
				
								//	Debug.Log ("Path found, length:  " + path.Count.ToString ());
								return path;
						}
								foreach (Tile _neighbor in GetValidNeighbors(current)) {
				
								if (closed.Contains (_neighbor))
										continue;
				
								if ((current.f + 1 < _neighbor.f) || (!open.Contains (_neighbor))) {
					
										_neighbor.f = current.f + 1;
										_neighbor.parent = current;
					
										if (!open.Contains (_neighbor)) {
												open.Add (_neighbor);
										}
								}
						}
				}
		
				return null;
		
		}




		/// <summary>
		/// Constructs the tiles according to maze array for initialization.
		/// </summary>
		/// <param name="map"> 2D 0-1 int array </param>
		void ConstructTiles (int[,] map)
		{
		
				tiles = new Tile[map.GetUpperBound (0) + 1, map.GetUpperBound (1) + 1];
				for (int  i = 0; i <= map.GetUpperBound(0); i++) {
						for (int j = 0; j<= map.GetUpperBound(1); j++) {
								tiles [i, j] = new Tile (i, j, map [i, j]);
						}
				}
		}




		/// <summary>
		/// Gets the lowest f_cost given a list of tiles (openSet).
		/// </summary>
		/// <returns>The lowest f.</returns>
		/// <param name="open">Open.</param>
		private Tile GetLowestF (List<Tile> open)
		{
		
				if (open.Count > 0) {
						//Vector2 temp = new Vector2 (open [0].x, open [0].y);
						Tile temp = open [0];
						foreach (Tile tile in open) {
								if (tile.f < temp.f) {
										temp = tile;
								}
						}
						return temp;
				} else {
						throw new ArgumentException ("open set is empty");
				}
		}




		/// <summary>
		/// Computes the manhattan.
		/// </summary>
		/// <returns>The manhattan Distance of tile1, and tile2.</returns>
		/// <param name="tile1">Tile1.</param>
		/// <param name="tile2">Tile2.</param>
		private int ComputeManhattan (Tile tile1, Tile tile2)
		{
				return (Math.Abs (tile1.x - tile2.x) + Math.Abs (tile1.y - tile2.y)); 
		}




		/// <summary>
		/// return valid (reachable walkable) neighbor tiles of current tile
		/// </summary>
		/// <returns>The valid neighbors.</returns>
		/// <param name="tile">Tile.</param>
		private List<Tile> GetValidNeighbors (Tile tile)
		{
				List<Tile> neighbors = new List<Tile> ();
		
				for (int i = 0; i < 4; i++) {
			
						if (tiles [tile.x + offsets [i] [0], tile.y + offsets [i] [1]].flag == 0) {
								if ((tile.x + offsets [i] [0] > 0) && (tile.y + offsets [i] [1] > 0) 
										&& (tile.x + offsets [i] [0] < tiles.GetUpperBound (0))
										&& (tile.y + offsets [i] [1] < tiles.GetUpperBound (1)))
										neighbors.Add (tiles [tile.x + offsets [i] [0], tile.y + offsets [i] [1]]);
						}
				}
				return neighbors;
		}


		/// <summary>
		/// Draw gizmo in scene view for easy debugging
		/// </summary>
		void OnDrawGizmos ()
		{
				if (shortestPath != null && shortestPath.Count > 0) {
						foreach (Tile t in shortestPath) {
								Gizmos.color = Color.red;
								Gizmos.DrawWireSphere (new Vector3 (transform.position.x + t.x, 0.6f, transform.position.z + t.y), 0.1f);
						}
				}
		}





		/// <summary>
		/// Updates the shortest path.
		/// </summary>
		/// <param name="x1">The start x value.</param>
		/// <param name="y1">The start y value.</param>
		/// <param name="x2">The target x value.</param>
		/// <param name="y2">The target y value.</param>
		public void UpdateShortestPath (int x1, int y1, int x2, int y2)
		{
				shortestPath = FindPath (tiles [x1, y1], tiles [x2, y2]);

		}
	
	
}
