using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
   /* character-levelcomponent key

      w  = wall
      r_ = ramp ()
         rr = ramp that can be driven up if moving in +X
         rl = ramp that can be driven up if moving in -X
         ru = ramp that can be driven up if moving in +Z
         rd = ramp that can be driven up if moving in -Z
      h  = barrier that bullets can pass through, but not tanks
      b  = breakable wall
      a_ = arch
         az = arch that can be driven under if moving in +/- Z
         ax = arch that can be driven under if moving in +/- X
      e# = enemy tank, type specified by number (in place of #)
      g# = gimmick object for this theme, which one specified by #
      p#.## = player #'s spawn point for round ## (eg p1.3 means player 1's spawnpoint for round 3)

    */
   public bool readFromGameState = false;

   public ThemeAssetPack theme;
   public TextAsset csvFloor1;
   public TextAsset csvFloor2;

   public const float FLOOR_HEIGHT = 0.5f;
   public const float SPAWN_LOC_HEIGHT_BUMP = 0.1f;

   private GameObject levelParent;

   private List<GameObject> gimmicks;
   private List<GameObject> enemies;
   private List<GameObject> breakableWalls;
   public List<GameObject>[] spawnPoints;

   private int counter = 0;
   
   [SerializeField] private NavigationBaker navMeshBaker = null;

   // Start is called before the first frame update
   void Start()
   {
      //BuildLevel();
   }

   // Update is called once per frame
   void Update()
   {

   }

   public void ResetEnemiesAndGimmicks()
   {
      foreach(GameObject g in gimmicks)
         GameObject.Destroy(g);
      foreach(GameObject g in enemies)
         GameObject.Destroy(g);
      foreach(GameObject g in breakableWalls)
         GameObject.Destroy(g);
      
      gimmicks = new List<GameObject>();
      enemies  = new List<GameObject>();
      breakableWalls = new List<GameObject>();
      
      //string[,] floor1 = BuildFloor(0, csvFloor1.text, true);
      //string[,] floor2 = BuildFloor(1, csvFloor2.text, true);

      string[] floors = GetFloors();
      for(int i = 0; i < floors.Length; i++)
         BuildFloor(i, floors[i], true);

   }

   public int GetNumEnemies()
   {
      return enemies.Count;
   }

   private string[] GetFloors()
   {
      string[] floors;

      if (readFromGameState)
      {
         //csvFloor1 = GameState.levelToLoad.Length >= 1? GameState.levelToLoad[0] : null;
         //csvFloor2 = GameState.levelToLoad.Length >= 2? GameState.levelToLoad[1] : null;
         floors = GameState.levelsToBuild[GameState.currentLevel];
         this.theme = GameState.levelTheme;

         //Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
         //Debug.Log(this.theme.gameObject.name + " ????? " + GameState.levelTheme.gameObject.name);
      }
      else
      {
         if (csvFloor2 != null)
            floors = new string[2];
         else
            floors = new string[1];

         floors[0] = csvFloor1.text;
         if (csvFloor2 != null)
            floors[1] = csvFloor2.text;
      }

      return floors;
   }

   public void BuildLevel()
   {
      if (readFromGameState)
         this.theme = GameState.levelTheme;

      //Debug.Log(theme.gameObject.name);

      GameObject.Destroy(levelParent);

      gimmicks = new List<GameObject>();
      enemies  = new List<GameObject>();
      breakableWalls = new List<GameObject>();
      spawnPoints = new List<GameObject>[4];   for(int i = 0; i < 4; i++) spawnPoints[i] = new List<GameObject>();

      levelParent = new GameObject("LevelParent" + (counter++));
      levelParent.transform.parent = this.transform;
      levelParent.transform.localPosition = new Vector3(0,0,0);

      string[] floors = GetFloors();

      // GameObject floor = GameObject.Instantiate(theme.floor); // commented line

      // new
      string[][,] builtFloors = new string[floors.Length][,];
      for(int i = 0; i < floors.Length; i++)
         builtFloors[i] = BuildFloor(i, floors[i]);

      int maxX=0, maxZ=0;

      for(int i = 0; i < builtFloors.Length; i++)
      {
         maxX = Mathf.Max(maxX, builtFloors[i].GetLength(1));
         maxZ = Mathf.Max(maxZ, builtFloors[i].GetLength(0));
      }

      Vector2 levelSize = new Vector2(maxX, maxZ-1);
      for(int i = 0; i < floors.Length+1; i++)
         BuildBoundingWalls(levelSize, i);

      GameState.loadedLevel = new string[builtFloors.Length, maxX, maxZ];

      for(int i = 0; i < builtFloors.Length; i++)
         for(int x = 0; x < builtFloors[i].GetLength(1); x++)
            for(int z = 0; z < builtFloors[i].GetLength(0); z++)
               GameState.loadedLevel[i, x, z] = builtFloors[i][z,x];
      
      
      // floor.transform.position = new Vector3(levelSize.x / 2f, 0, -levelSize.y/2f); // commented line

      // GameObject realFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
      // realFloor.GetComponent<MeshRenderer>().enabled = false;
      // realFloor.layer = LayerMask.NameToLayer("Arena Objects");
      // realFloor.transform.position = floor.transform.position;
      // realFloor.transform.localScale = new Vector3(levelSize.x/10f, 1, levelSize.y/10f);
      // floor.layer = LayerMask.NameToLayer("Default");
      BuildGround(levelSize);

      // sort the spawnpoints
      foreach (List<GameObject> spawns in spawnPoints)
      {
         spawns.Sort(delegate(GameObject g1, GameObject g2) { return g1.name.CompareTo(g2.name); });
      }

      if (navMeshBaker != null) navMeshBaker.RebuildNavMesh();
   }

   public GameObject BuildGround(Vector2 levelSize)
   {
      GameObject floorParent = new GameObject("Ground");

      GameObject floor = GameObject.Instantiate(theme.floor); // commented line
      floor.transform.position = new Vector3(levelSize.x / 2f, 0, -levelSize.y/2f); // commented line
      floor.transform.parent = floorParent.transform;

      GameObject realFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
      realFloor.GetComponent<MeshRenderer>().enabled = false;
      realFloor.layer = LayerMask.NameToLayer("Arena Objects");
      realFloor.transform.position = floor.transform.position;
      realFloor.transform.localScale = new Vector3(levelSize.x/10f, 1, levelSize.y/10f);
      realFloor.transform.parent = floorParent.transform;
      floor.layer = LayerMask.NameToLayer("Default");

      return floorParent;
   }

   public GameObject BuildBoundingWalls(Vector2 size, int floorNum)
   {
      GameObject boundingWallsParent = new GameObject("Bounding Walls");
      boundingWallsParent.transform.parent = levelParent.transform;
      
      GameObject[] transparentWalls = theme.transparentWalls;
      if (transparentWalls == null || transparentWalls.Length == 0) transparentWalls = theme.walls;

      for (int x = -1; x <= size.x; x++)
      {
         CreateTile(theme.walls,      0, floorNum, x, -1).transform.parent = boundingWallsParent.transform; 
         CreateTile(transparentWalls, 0, floorNum, x, (int)Mathf.Round(size.y)).transform.parent = boundingWallsParent.transform; 
      }
      for (int z = 0; z <= size.y; z++)
      {
         CreateTile(theme.walls, 0, floorNum, -1,                       z).transform.parent = boundingWallsParent.transform;
         CreateTile(theme.walls, 0, floorNum, (int)Mathf.Round(size.x), z).transform.parent = boundingWallsParent.transform;
      }

      return boundingWallsParent;
   }

   public string[,] BuildFloor(int floorNum, string levelString)
   {
      return BuildFloor(floorNum, levelString, false);
   }

   public string[,] BuildFloor(int floorNum, string levelString, bool onlyGimmicksAndEnemies)
   {
      if (levelString == null)
      {
         return new string[1,1];
      }

      string[] lines = levelString.Split('\n');
      string[,] grid = new string[lines.Length, lines[0].Split(',').Length];

      for (int z = 0; z < lines.Length; z++) 
      {
         string[] tiles = lines[z].Split(',');
         for (int x = 0; x < tiles.Length; x++)
         {
            grid[z, x] = tiles[x];
         }
      }

      for (int z = 0; z < grid.GetLength(0); z++) 
      {
         for (int x = 0; x < grid.GetLength(1); x++)
         {
            string tile = grid[z, x];
            if (tile == "" || tile == null) continue;
            if (tile[0] == ' ') tile = tile.Substring(1);
            
            GameObject g = null;
            int idx = 0;

            // if the tile string is of the form "[tileName][number]"
            // find out which number it is
            if (char.IsDigit(tile[tile.Length-1]))
            {
               int digStart = tile.Length-1;
               for (; digStart > 0 && char.IsDigit(tile[digStart-1]); digStart--);

               idx = int.Parse(tile.Substring(digStart));
            }

            switch(tile[0])
            {
               case 'w': if (onlyGimmicksAndEnemies) break; g = CreateTile(theme.walls,            idx, floorNum, x, z);             break;
               case 't': if (onlyGimmicksAndEnemies) break; g = CreateTile(theme.transparentWalls, idx, floorNum, x, z);             break;
               
               case 'h': if (onlyGimmicksAndEnemies) break; g = CreateTile(theme.halfWalls,        idx, floorNum, x, z);         break;
               case 'b': g = CreateTile(theme.breakableWalls, idx, floorNum, x, z); breakableWalls.Add(g);    break;
               
               case 'r': 
                  if (onlyGimmicksAndEnemies) break;

                  g = CreateTile(theme.ramps, idx, floorNum, x, z);
                  switch(tile[1])
                  {
                     case 'r': g.transform.eulerAngles = new Vector3(0, -90, 0); break;
                     case 'l': g.transform.eulerAngles = new Vector3(0,  90, 0); break;
                     case 'u': g.transform.eulerAngles = new Vector3(0, 180, 0); break;
                     case 'd': break;
                  }
                  break;
               case 'a': 
                  if (onlyGimmicksAndEnemies) break;

                  g = CreateTile(theme.archs, idx, floorNum, x, z);
                  switch(tile[1])
                  {
                     case 'x': g.transform.eulerAngles = new Vector3(0, 90, 0); break;
                     case 'z': g.transform.eulerAngles = new Vector3(0, 0, 0); break;
                  }
                  break;
               
               case 'g': g = TryCreateGimmickTile(tile,                         floorNum, x, z); break;
               case 'e': g = TryCreateEnemyTile  (int.Parse(tile.Substring(1)), floorNum, x, z); break;
               
               case 'p': 
                  if (onlyGimmicksAndEnemies) break;

                  // p1.
                  int forRoundNum = int.Parse(tile.Substring(3));

                  GameObject spawnType = null;
                  int playerNum = 0;
                  switch(tile[1])
                  {
                     case '1': playerNum = 1; if (theme.p1Spawns.Length == 0) break; spawnType = theme.p1Spawns[(forRoundNum-1) % theme.p1Spawns.Length]; break;
                     case '2': playerNum = 2; if (theme.p2Spawns.Length == 0) break; spawnType = theme.p2Spawns[(forRoundNum-1) % theme.p2Spawns.Length]; break;
                     case '3': playerNum = 3; if (theme.p3Spawns.Length == 0) break; spawnType = theme.p3Spawns[(forRoundNum-1) % theme.p3Spawns.Length]; break;
                     case '4': playerNum = 4; if (theme.p4Spawns.Length == 0) break; spawnType = theme.p4Spawns[(forRoundNum-1) % theme.p4Spawns.Length]; break;
                  }

                  if (spawnType != null && GameState.playerJoined[playerNum-1])
                     g = GameObject.Instantiate(spawnType);//new GameObject(tile);
                  else
                     g = new GameObject();

                  if (1 <= playerNum && playerNum <= 4)
                     spawnPoints[playerNum-1].Add(g);
                  else
                     Debug.LogError("PRoblem " + tile+ "  " + playerNum);

                  g.name = tile;
                  g.transform.position = new Vector3(x+0.5f, floorNum*FLOOR_HEIGHT+SPAWN_LOC_HEIGHT_BUMP, -(z+0.5f));
                  g.transform.parent = levelParent.transform;
                  break;
               case 'k': if (onlyGimmicksAndEnemies) break; if (GameState.gameplayType != GameState.GameplayType.KING_OF_THE_HILL) break; g = CreateTile(new GameObject[1]{theme.kingOfTheHillCapturePoint}, 0, floorNum, x, z);             break;
            }

            if (g == null) continue;

            GameObject xp = null;
            GameObject xm = null;
            GameObject zp = null;
            GameObject zm = null;
            try 
            {
               Transform boundaries = g.transform.Find("Boundaries");
               boundaries.transform.eulerAngles = new Vector3(0, 0, 0);

               boundaries.gameObject.SetActive(GameState.config.levelBuildingConfig.makeBoundaries);

               xp = g.transform.Find("Boundaries/X+ Boundary").gameObject;
               xm = g.transform.Find("Boundaries/X- Boundary").gameObject;
               zp = g.transform.Find("Boundaries/Z+ Boundary").gameObject;
               zm = g.transform.Find("Boundaries/Z- Boundary").gameObject;
            } 
            catch (Exception)
            {
               continue;
            }

            if(IsNextFloorDrivable(grid, x+1, z,  1,  0)) xp.SetActive(false);
            if(IsNextFloorDrivable(grid, x-1, z, -1,  0)) xm.SetActive(false);
            if(IsNextFloorDrivable(grid, x, z-1,  0,  1)) zp.SetActive(false);
            if(IsNextFloorDrivable(grid, x, z+1,  0, -1)) zm.SetActive(false);
            
         }
      }

      // BuildFloorCollisionMesh(grid, floorNum);

      return grid;
   }

   // public void BuildFloorCollisionMesh(string[,] tiles, int floorNum)
   // {

   // }

   private GameObject CreateTile(GameObject[] prefabList, int idx, int floorNum, int x, int z)
   {
      if (prefabList == null || prefabList.Length == 0)
      {
         Debug.LogError("Failed to create tile of theme \"" + theme.gameObject.name + "\"" + " @ x="+x+", z="+z);
         return null;
      }

      if (idx >= prefabList.Length || idx < 0) idx = 0;

      GameObject g = GameObject.Instantiate(prefabList[idx]);
      g.transform.position = new Vector3(x+0.5f, g.transform.position.y+floorNum*FLOOR_HEIGHT, -(z+0.5f));
      g.transform.parent = levelParent.transform;
      return g;
   }

   private GameObject TryCreateGimmickTile(string tileString, int floorNum, int x, int z)
   {
      // tilestring will be of the form /g(\d+)(\.(.+))?/  eg: g1 or g5.38af5
      // the number right after the g is the "data" which determines which type of gimmick object to spawn
      // and the stuff after the . is just metadata
      string[] dataAndMetadata = tileString.Substring(1).Split('.');
      int gimmickNumber = int.Parse(dataAndMetadata[0]);

      try 
      {
         GameObject g = CreateTile(theme.gimmickObjects, gimmickNumber, floorNum, x, z);
         
         GimmickMeta gm = g.GetComponent<GimmickMeta>();
         if (gm == null)
         {
            g.AddComponent<GimmickMeta>();
            gm = g.GetComponent<GimmickMeta>();
         }

         gm.metadata = dataAndMetadata.Length >= 2? dataAndMetadata[1] : "";

         gimmicks.Add(g);
         return g;
      }
      catch (Exception)
      {
         Debug.LogError("Failed to create gimickObject" + gimmickNumber + " of theme \"" + theme.gameObject.name + "\"" + " @ x="+x+", z="+z);
      }

      return null;
   }

   private GameObject TryCreateEnemyTile(int enemyNumber, int floorNum, int x, int z)
   {
      try 
      {
         GameObject g = CreateTile(theme.enemyPrefabs, enemyNumber, floorNum, x, z);
         enemies.Add(g);
         return g;
      }
      catch (Exception)
      {
         Debug.LogError("Failed to create enemy" + enemyNumber + " of theme \"" + theme.gameObject.name + "\"" + " @ x="+x+", z="+z);
      }

      return null;
   }

   private bool IsNextFloorDrivable(string[,] grid, int x, int z, int dx, int dz)
   {
      if (x < 0 || z < 0 || z >= grid.GetLength(0) || x >= grid.GetLength(1)) return false;
      if (grid[z, x] == "" || grid[z, x] == null)                               return false;

      string tile = grid[z, x];
      switch(tile[0])
      {
         case 'w': return true;
         case 'h': return false;
         case 'b': return true;
         case 'r': 
            switch(tile[1])
            {
               case 'r': return dx < 0;
               case 'l': return dx > 0;
               case 'u': return dz < 0;
               case 'd': return dz > 0;
            }
            break;
         case 'a': return true;
         case 'g': return true; // maybe true, not always
         case 'e': return false;
         case 'p': return false;
      }

      return false;
   }
}
