using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class TileEditor : MonoBehaviour { }

#if UNITY_EDITOR
[CustomEditor(typeof(TileEditor))]
[CanEditMultipleObjects]
public class TileEditorExt : Editor {

	//name string constants
	const string basicTileName = "Tile";
	const string bckgrTileName = "Background";
	const string spclTileName = "Special";
	const string basicParentName = "basicTileParent";
	const string bckgrParentName = "backgroundTileParent";

	//tile hash tables, for increased performance
	Dictionary<TileCoords, Transform> basicTileDictionary, bckgrTileDictionary;

	//editor state variables
	bool building, deletingPrompt, largePlacementPrompt;

	//problem flags
	bool tilesInParent, unkownObjectInParent, invalidObjectInTiles, tilesetMissing, 
		tilesetInvalid, basicTileParentMissing, backgroundTileParentMissing;

	//tile editor children, needed for general functionality
	TileSet tileSet;
	Transform basicTileParent;
	Transform backgroundTileParent;

	//build input variables
	bool mouseInBuildPlane;
	int buildX, buildY;
	int dragKey; bool freezeDrag;
	int dragX1, dragX2, dragY1, dragY2;

	Transform transform{ get{
		return ((TileEditor)this.target).transform;
	}}

	private void OnEnable() {
		building = false;
		deletingPrompt = false;
		CheckForProblems();
		if(!anyProblem)
			PrepareForBuilding();
	}

	/// <summary>
	/// Check if some problem is occuring, and switch problem flags accordingly.
	/// </summary>
	private void CheckForProblems() {
		//iterate trough child objects of tileEditor
		unkownObjectInParent = false;
		tilesInParent = false;
		tilesetMissing = true;
		basicTileParentMissing = true;
		backgroundTileParentMissing = true;
		for(int i = 0; i < transform.childCount; i++) {
			Transform t = transform.GetChild(i);

			//look for tile set
			if(t.GetComponent<TileSet>() != null && tilesetMissing) {
				tilesetMissing = false;
				tileSet = t.GetComponent<TileSet>();
			}

			//look for basic tile parent
			else if(t.name == basicParentName && basicTileParentMissing) {
				basicTileParentMissing = false;
				basicTileParent = t;
			}

			//look for background tile parent
			else if(t.name == bckgrParentName && backgroundTileParentMissing) {
				backgroundTileParentMissing = false;
				backgroundTileParent = t;
			}

			else {
				//found foreign object, check if it is a stray tile or something else
				if(IsLegalTileName(t.name))
					tilesInParent = true;
				else
					unkownObjectInParent = true;
			}
		}

		//check if tileset is valid, if we found one
		if(!tilesetMissing) 
			tilesetInvalid = !tileSet.Valid();

		//check for invalid object in tile parents, if they're available
		invalidObjectInTiles = false;
		if(basicTileParentMissing || backgroundTileParentMissing)
			return;

		//check for invalid object in basic tiles.
		for(int i = 0; i < basicTileParent.childCount; i++) {
			string name = basicTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				invalidObjectInTiles = true;
			else if(!IsBasic(name))
				invalidObjectInTiles = true;
		}

		//check for invalid object in background tiles.
		for(int i = 0; i < backgroundTileParent.childCount; i++) {
			string name = backgroundTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				invalidObjectInTiles = true;
			else if(!IsBackgr(name))
				invalidObjectInTiles = true;
		}
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		//tileSet = (TileSet)EditorGUILayout.ObjectField(tileSet, typeof(TileSet), true);

		if(building) {
			if(deletingPrompt) {
				GUILayout.Label("Are you sure you want to delete all tiles?");
				if(GUILayout.Button("Delete all")) {
					DeleteAllTiles();
					deletingPrompt = false;
				}
				if(GUILayout.Button("Cancel"))
					deletingPrompt = false;
			}
			else if(largePlacementPrompt) {
				int nTiles = (Mathf.Abs(dragX2 - dragX1) + 1) * (Mathf.Abs(dragY2 - dragY1) + 1);
				GUILayout.Label("You are about to place down ");
				GUILayout.Label(nTiles + " tiles");
				GUILayout.Label("Are you sure you want to do this?");
				if(GUILayout.Button("Cancel")) {
					largePlacementPrompt = false;
					dragKey = 0;
					freezeDrag = false;
				}
				if(GUILayout.Button("Confirm")) {
					largePlacementPrompt = false;
					PlaceRectangleOfTiles();
					freezeDrag = false;
				}
			}
			else {
				//building normally
				GUILayout.Label("1 toggle current tile");
				GUILayout.Label("2 press twice to fill rectangle of tiles");
				GUILayout.Label("3 press twice to clear rectangle of tiles");
				GUILayout.Label("4 toggle (standard) background tile");
				GUILayout.Label("5 set 2nd background tile (light)");
				GUILayout.Label("6 set 3rd background tile (window)");
				GUILayout.Label("7 cycle trough special tiles");
				GUILayout.Label("8 press twice to fill rectangle with standard background tiles");
				GUILayout.Label("9 press twice to clear rectangle off background tiles");

				if(GUILayout.Button("Delete all"))
					deletingPrompt = true;
				if(GUILayout.Button("Update all"))
					UpdateAllTiles();
			}
		}
		else {
			if(tilesInParent) {
				GUILayout.Label("Tiles found parented to editor!");
				if(GUILayout.Button("Move tile objects")) {
					MoveTilesToAppropriateParents();
					CheckForProblems();
				}
				GUILayout.Label("");
			}

			if(unkownObjectInParent) {
				GUILayout.Label("Foreign objects found parented to editor!");
				if(GUILayout.Button("Delete foreign objects")) {
					DeleteForeignObjects();
					CheckForProblems();
				}
				GUILayout.Label("");
			}

			if(invalidObjectInTiles) {
				GUILayout.Label("Found invalid objects in tile hierarchy");
				if(GUILayout.Button("Delete invalid objects")) {
					DeleteInvalidTiles();
					CheckForProblems();
				}
				GUILayout.Label("");
			}

			if(tilesetMissing) {
				GUILayout.Label("Could not find tileSet object!");
				GUILayout.Label("Please parent a valid tileSet to the tileEditor.");
				GUILayout.Label("");
			}

			if(tilesetInvalid) {
				GUILayout.Label("Tileset object is invalid!");
				GUILayout.Label("Please solve the problems shown in the tileSet.");
				GUILayout.Label("");
			}

			if(basicTileParentMissing) {
				GUILayout.Label("Could not find \"" + basicParentName + "\"!");
				if(GUILayout.Button("Spawn empty \"" + bckgrParentName + "\"")) {
					Transform newParent = (new GameObject(basicParentName)).transform;
					newParent.SetParent(transform);
					newParent.localScale = Vector3.one;
					CheckForProblems();
				}
				GUILayout.Label("");
			}

			if(backgroundTileParentMissing) {
				GUILayout.Label("Could not find \"" + bckgrParentName + "\"!");
				if(GUILayout.Button("Spawn empty \"" + bckgrParentName + "\"")) {
					Transform newParent = (new GameObject(bckgrParentName)).transform;
					newParent.SetParent(transform);
					newParent.localScale = Vector3.one;
					CheckForProblems();
				}
				GUILayout.Label("");
			}

			if(anyProblem) {
				if(GUILayout.Button("Refresh")) {
					CheckForProblems();
				}
			}
			else { 
				if(GUILayout.Button("Start building!")) {
					PrepareForBuilding();
				}
			}
		}
	}

	private bool anyProblem { get {
		return tilesInParent || unkownObjectInParent ||
			invalidObjectInTiles || tilesetMissing || tilesetInvalid ||
			basicTileParentMissing || backgroundTileParentMissing;
	}}

	private void PrepareForBuilding() {
		basicTileDictionary = new Dictionary<TileCoords, Transform>();
		bckgrTileDictionary = new Dictionary<TileCoords, Transform>();

		for(int i = 0; i < basicTileParent.childCount; i++) {
			Transform tile = basicTileParent.GetChild(i);
			TileCoords key = new TileCoords(tile);
			if(basicTileDictionary.ContainsKey(key)) {
				DestroyImmediate(tile.gameObject);
				Debug.LogWarning("Deleted duplicate tile!");
			}
			else
				basicTileDictionary.Add(key, tile);
		}

		for(int i = 0; i < backgroundTileParent.childCount; i++) {
			Transform tile = backgroundTileParent.GetChild(i);
			TileCoords key = new TileCoords(tile);
			if(bckgrTileDictionary.ContainsKey(key)) {
				DestroyImmediate(tile.gameObject);
				Debug.LogWarning("Deleted duplicate tile!");
			}
			else
				bckgrTileDictionary.Add(key, tile);
		}

		building = true;
	}

	/// <summary>
	/// Moves all valid tiles parented to editor to their parents.
	/// </summary>
	private void MoveTilesToAppropriateParents() {

		//look for basic and special tiles
		for(int i = 0; i < transform.childCount; i++) {
			string name = transform.GetChild(i).name;
			if(IsLegalTileName(name)) {
				bool empty = GetTile(GetX(name), GetY(name)) == null;
				if(IsBasic(name) && empty) {
					transform.GetChild(i).SetParent(basicTileParent);
					transform.localScale = Vector3.one;
					i--;
				}
			}
		}

		//look for background tiles
		for(int i = 0; i < transform.childCount; i++) {
			string name = transform.GetChild(i).name;
			if(IsLegalTileName(name)) {
				bool empty = GetBackGroundTile(GetX(name), GetY(name)) == null;
				if(IsBackgr(name) && empty) {
					transform.GetChild(i).SetParent(backgroundTileParent);
					transform.localScale = Vector3.one;
					i--;
				}
			}
		}
	}

	private void DeleteForeignObjects() {
		//firstly, move all valid tiles out of the way
		MoveTilesToAppropriateParents();

		//now remove anything that is not one of the necessary children
		for(int i = 0; i < transform.childCount; i++) {
			Transform t = transform.GetChild(i);
			if(t != tileSet.transform && t != basicTileParent && t != backgroundTileParent)
				DestroyImmediate(t.gameObject);
		}
	}

	private void DeleteInvalidTiles() {

		//check for invalid object in basic tiles.
		for(int i = 0; i < basicTileParent.childCount; i++) {
			string name = basicTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				DestroyImmediate(basicTileParent.GetChild(i).gameObject);
			else if(!IsBasic(name))
				DestroyImmediate(basicTileParent.GetChild(i).gameObject);
		}

		//check for invalid object in background tiles.
		for(int i = 0; i < backgroundTileParent.childCount; i++) {
			string name = backgroundTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				DestroyImmediate(backgroundTileParent.GetChild(i).gameObject);
			else if(!IsBackgr(name))
				DestroyImmediate(backgroundTileParent.GetChild(i).gameObject);
		}
	}

	private void OnSceneGUI() {
		if(!building)
			return;

		if(Event.current.isKey) {
			int index;
			if(int.TryParse(Event.current.character.ToString(), out index)) {
				switch(index) {
				case 1:
				if(GetTile(buildX, buildY) == null)
					SpawnTile();
				else
					DeleteTile();
				break;

				case 4:
				if(GetBackGroundTile(buildX, buildY) == null)
					SpawnBackgroundTile(1);
				else
					DeleteBackgroundTile();
				break;

				case 5:	case 6:
				if(GetBackGroundTile(buildX, buildY) != null)
					DeleteBackgroundTile();
				SpawnBackgroundTile(index - 3);
				break;

				case 7:
				SpawnSpecialTile(1);
				break;

				case 2:	case 3:	case 8:	case 9:
				if(dragKey == 0) {
					//start drag selection
					dragKey = index;
					dragX1 = buildX;
					dragY1 = buildY;
				}
				else if(dragKey == index) {
					//confirm drag selection
					int nTiles = (Mathf.Abs(dragX2 - dragX1) + 1) * 
						(Mathf.Abs(dragY2 - dragY1) + 1);
					bool placing = dragKey == 2 || dragKey == 8;

					if(placing && nTiles > 1000) {
						freezeDrag = true;
						largePlacementPrompt = true;
						Repaint();
					}
					else
						PlaceRectangleOfTiles();
				}
				else {
					//cancel drag selection
					dragKey = 0;
				}
				break;
				}
			}
		}
		if(dragKey != 0) {
			if(!freezeDrag) {
				dragX2 = buildX;
				dragY2 = buildY;
			}
			buildX = dragX1;
			buildY = dragY1;
			MarkBuildPoint(new Color(1f, .2f, .2f));
			buildX = dragX2;
			buildY = dragY2;
		}

		if(Event.current.type == EventType.MouseMove) {
			Vector3 mousePos = Event.current.mousePosition;
			mousePos = Camera.current.ScreenToViewportPoint(mousePos);
			mousePos.Scale(new Vector3(-1f, 1f));
			mousePos = new Vector3(0f, 1f) - mousePos;
			Ray ray = Camera.current.ViewportPointToRay(mousePos);
			MouseCast(ray);
		}
		if(mouseInBuildPlane)
			MarkBuildPoint(new Color(1f, 1f, .2f));
	}

	/// <summary>
	/// Places or deletes strech of tiles defined by drag coordinates and dragKey.
	/// </summary>
	private void PlaceRectangleOfTiles() {
		for(int x = Mathf.Min(dragX1, dragX2); x <= Mathf.Max(dragX1, dragX2); x++) {
			for(int y = Mathf.Min(dragY1, dragY2); y <= Mathf.Max(dragY1, dragY2); y++) {
				buildX = x;
				buildY = y;
				switch(dragKey) {
				case 2:
				if(GetTile(buildX, buildY) == null)
					SpawnTile();
				break;
				case 3:
				DeleteTile();
				break;
				case 8:
				if(GetBackGroundTile(buildX, buildY) == null)
					SpawnBackgroundTile(1);
				break;
				case 9:
				DeleteBackgroundTile();
				break;
				}
			}
		}
		dragKey = 0;
	}

	private void MouseCast(Ray ray) {		
		Vector3 normal = this.transform.forward;
		float offset = Vector3.Dot(transform.position, normal);

		Plane buildPlane = new Plane(normal, -offset);

		float enter = 0f;
		mouseInBuildPlane = buildPlane.Raycast(ray, out enter);
		if(mouseInBuildPlane) {
			Vector3 intersect = ray.origin + enter * ray.direction;

			float relX = Vector3.Dot(intersect - transform.position, transform.right) / transform.localScale.x;
			float relY = Vector3.Dot(intersect - transform.position, transform.up) / transform.localScale.y;

			buildX = Mathf.FloorToInt(relX);
			buildY = Mathf.FloorToInt(relY);
		}
	}

	private void MarkBuildPoint(Color clr) {
		Vector3 center = GetWorldPoint(buildX, buildY);
		Handles.color = clr;

		for(int b = 0; b < 8; b++) {
			Vector3 up = (b % 2 == 0 ? -.5f : .5f) * transform.up;
			up = up * transform.localScale.y;
			Vector3 right = ((b >> 1) % 2 == 0 ? -.5f : .5f) * transform.right;
			right = right * transform.localScale.x;
			Vector3 forward = ((b >> 2) % 2 == 0 ? -1f : 1f) * transform.forward;
			forward = forward * transform.localScale.z;
			Vector3 corner = center + up + right + forward;
			
			Handles.DrawLine(corner, corner - up);
			Handles.DrawLine(corner, corner - right);
			Handles.DrawLine(corner, corner - forward);
		}

		SceneView.RepaintAll();
	}

	private Vector3 GetWorldPoint(int x, int y) {
		Vector3 coordOffset = ((x + .5f) * transform.right * transform.localScale.x) + 
			((y + .5f) * transform.up * transform.localScale.y);

		return transform.position + coordOffset;
	}

	/// <summary>
	/// Looks for tile at given x and y position and returns it.
	/// Returns null if no tile was found at given position.
	/// </summary>
	private Transform GetTile(int x, int y) {
		if(building) {
			Transform tile;
			basicTileDictionary.TryGetValue(new TileCoords(x, y), out tile);
			return tile;
		}

		//if not building, search for the tile manually
		for(int i = 0; i < basicTileParent.childCount; i++) {
			string name = basicTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				continue;
			int refX = GetX(name);
			int refY = GetY(name);
			if(IsBasic(name))
				if(x == refX && y == refY)
					return basicTileParent.GetChild(i);
		}
		return null;
	}

	/// <summary>
	/// Delete tile object at current build position
	/// </summary>
	private void DeleteTile() {
		Transform t = GetTile(buildX, buildY);
		if(t != null) {
			//if building, maintain tile dictionary
			if(building)
				basicTileDictionary.Remove(new TileCoords(t));

			DestroyImmediate(t.gameObject);
		}

		UpdateTile(buildX + 1, buildY);
		UpdateTile(buildX - 1, buildY);
		UpdateTile(buildX, buildY + 1);
		UpdateTile(buildX, buildY - 1);
	}

	/// <summary>
	/// Spawn tile object at current build position
	/// </summary>
	private void SpawnTile() {
		Transform newTile = tileSet.SpawnTile(false, false, false, false);
		newTile.position = GetWorldPoint(buildX, buildY);
		newTile.SetParent(basicTileParent);
		newTile.localScale = Vector3.one;
		newTile.name = GetName(basicTileName, buildX, buildY);

		//if building, add tile to the dictionary
		if(building)
			basicTileDictionary[new TileCoords(newTile)] = newTile;

		UpdateTile(buildX, buildY);

		UpdateTile(buildX + 1, buildY);
		UpdateTile(buildX - 1, buildY);
		UpdateTile(buildX, buildY + 1);
		UpdateTile(buildX, buildY - 1);
	}

	/// <summary>
	/// Force tile at given position to adapt to its surroundings, if it exists
	/// </summary>
	private void UpdateTile(int x, int y) {
		//check if there is a tile at this position
		Transform oldTile = GetTile(x, y);
		if(oldTile == null)
			return;

		//should not update special tiles
		if(GetTypeName(oldTile.name).StartsWith(spclTileName))
			return;

		//dealing with actual tile, delete old one and spawn a new.
		DestroyImmediate(oldTile.gameObject);

		bool up = GetTile(x, y + 1) != null;
		bool left = GetTile(x - 1, y) != null;
		bool right = GetTile(x + 1, y) != null;
		bool down = GetTile(x, y - 1) != null;
		Transform newTile = tileSet.SpawnTile(up, left, right, down);

		newTile.SetParent(basicTileParent);
		newTile.localScale = Vector3.one;
		newTile.position = GetWorldPoint(x, y);
		newTile.name = GetName(basicTileName, x, y);
		newTile.gameObject.isStatic = transform.gameObject.isStatic;

		//if building, add tile to the dictionary
		if(building)
			basicTileDictionary[new TileCoords(newTile)] = newTile;
	}

	/// <summary>
	/// Deletes all valid tiles
	/// </summary>
	private void DeleteAllTiles() {
		while(basicTileParent.childCount > 0)
			DestroyImmediate(basicTileParent.GetChild(0).gameObject);
		while(backgroundTileParent.childCount > 0)
			DestroyImmediate(backgroundTileParent.GetChild(0).gameObject);
	}

	/// <summary>
	/// Forces update on all valid tiles
	/// </summary>
	private void UpdateAllTiles() {

		int repeat = basicTileParent.childCount; 
		while(repeat > 0) {
			repeat--;

			string name = basicTileParent.GetChild(0).name;
			int x = GetX(name); int y = GetY(name);

			//basic tile
			if(GetTypeName(name).StartsWith(basicTileName))
				UpdateTile(x, y);

			//special tile
			else {
				buildX = x;	buildY = y;
				bool hasSpecials = tileSet.GetNbSpecialTiles() > 0;
				if(hasSpecials)
					SpawnSpecialTile(0);
				else
					DeleteTile();
			}
		}

		repeat = backgroundTileParent.childCount;
		while(repeat > 0) {
			repeat--;

			//background tile
			string name = backgroundTileParent.GetChild(0).name;
			buildX = GetX(name); buildY = GetY(name);
			DeleteBackgroundTile();
			SpawnBackgroundTile(GetVariationIndex(name));
		}
	}

	/// <summary>
	/// Return background tile at given coordinates, if one exists.
	/// </summary>
	private Transform GetBackGroundTile(int x, int y) {
		if(building) {
			Transform tile;
			bckgrTileDictionary.TryGetValue(new TileCoords(x, y), out tile);
			return tile;
		}

		//if not building, search for the tile manually
		for(int i = 0; i < backgroundTileParent.childCount; i++) {
			string name = backgroundTileParent.GetChild(i).name;
			if(!IsLegalTileName(name))
				continue;
			int refX = GetX(name); int refY = GetY(name);
			if(IsBackgr(name) && x == refX && y == refY)
				return backgroundTileParent.GetChild(i);
		}
		return null;
	}
	
	/// <summary>
	/// spawns tile of given type at build coordinates.
	/// </summary>
	private void SpawnBackgroundTile(int type) {
		Transform newTile = tileSet.SpawnBackground(type);
		newTile.position = GetWorldPoint(buildX, buildY);
		newTile.SetParent(backgroundTileParent);
		newTile.localScale = Vector3.one;
		newTile.name = GetName(bckgrTileName + "_" + type, buildX, buildY);
		newTile.gameObject.isStatic = transform.gameObject.isStatic;

		//if building, add tile to the dictionary
		if(building)
			bckgrTileDictionary[new TileCoords(newTile)] = newTile;
	}
	
	/// <summary>
	/// Delete tile at build coordinates, if it exists
	/// </summary>
	private void DeleteBackgroundTile() {
		Transform t = GetBackGroundTile(buildX, buildY);
		if(t != null) {
			//if building, maintain tile dictionary
			if(building)
				bckgrTileDictionary.Remove(new TileCoords(t));

			DestroyImmediate(t.gameObject);
		}
	}


	private void SpawnSpecialTile(int cycleOffset) {
		Transform oldTile = GetTile(buildX, buildY);
		int next = cycleOffset - 1;
		if(oldTile != null) {
			next = GetVariationIndex(oldTile.name) + cycleOffset;
			DestroyImmediate(oldTile.gameObject);
		}

		int nbSpecial = tileSet.GetNbSpecialTiles();
		next = next % nbSpecial;

		Transform newTile = tileSet.SpawnNextSpecial(next);
		newTile.position = GetWorldPoint(buildX, buildY);
		newTile.SetParent(basicTileParent);
		newTile.localScale = Vector3.one;
		newTile.name = GetName(spclTileName + '_' + next, buildX, buildY);
		newTile.gameObject.isStatic = transform.gameObject.isStatic;

		//if building, add tile to the dictionary
		if(building)
			basicTileDictionary[new TileCoords(newTile)] = newTile;
		
		UpdateTile(buildX + 1, buildY);
		UpdateTile(buildX - 1, buildY);
		UpdateTile(buildX, buildY + 1);
		UpdateTile(buildX, buildY - 1);
	}

	/// <summary>
	/// Returns true if given name is a legal name, 
	/// assigned to a tile generated by the tileEditor.
	/// </summary>
	private static bool IsLegalTileName(string name) {
		if(string.IsNullOrEmpty(name))
			return false;
		string[] split = name.Split('.');
		if(split.Length != 3)
			return false;
		int dummy = 0;
		if(!int.TryParse(split[1], out dummy))
			return false;
		if(!int.TryParse(split[2], out dummy))
			return false;
		return true;
	}


	private static int GetVariationIndex(string name) {
		string sub = name.Split('.')[0];
		string[] split = sub.Split('_');
		if(split.Length < 2)
			return -1;
		return int.Parse(split[1]);
	}

	private static bool IsBasic(string name) {
		return GetTypeName(name).StartsWith(basicTileName) ||
			GetTypeName(name).StartsWith(spclTileName);
	}

	private static bool IsBackgr(string name) {
		return GetTypeName(name).StartsWith(bckgrTileName);
	}

	private static string GetTypeName(string name) {
		return name.Split('.')[0];
	}

	static int GetX(string name) {
		return int.Parse(name.Split('.')[1]);
	}
	
	static int GetY(string name) {
		return int.Parse(name.Split('.')[2]);
	}

	private static string GetName(string typeName, int x, int y) {
		return typeName + "." + x + '.' + y;
	}

	/// <summary>
	/// x y coordinate struct used in tile dictionaries
	/// </summary>
	private struct TileCoords {
		int x, y;

		public TileCoords(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public TileCoords(Transform tile) {
			x = GetX(tile.name);
			y = GetY(tile.name);
		}

		public override bool Equals(object obj) {
			TileCoords o = (TileCoords)obj;
			return x == o.x && y == o.y;
		}

		public override int GetHashCode() {
			return x * 0x3FD3 + y;
		}

		public override string ToString() {
			return x.ToString() + '.' + y.ToString();
		}
	}
}
#endif