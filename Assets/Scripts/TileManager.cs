using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs; // Array of tile prefabs
    [SerializeField] Transform player; // Reference to the player's transform
    [SerializeField] float deleteDistance = 10f; // Distance behind the player to delete tiles
    [SerializeField] int numberOfTiles = 5;
    [SerializeField] int maxTries;

    private Queue<GameObject> activeTiles = new Queue<GameObject>(); // Queue to store active tiles
    private Queue<List<GameObject>> activeObj = new Queue<List<GameObject>>(); // Queue to store active tiles
    private Vector3 lastTilePosition = Vector3.zero; // Store the endpoint of the last tile
    public Transform carr;
    public Transform camm;
    private GameObject lastTile;    // Reference to the last placed tile
    public GameObject battery;
    void Start() {
        for(int i = 0; i < numberOfTiles; i++)
            SpawnTile();
    }
    void Update()
    {
        //Debug.Log(IsOverlapping(lastTile));
        // Delete tiles behind the player
        if ((activeTiles.Count > 0 && Vector3.Distance(player.position, activeTiles.Peek().transform.position) > deleteDistance))
        {
            Destroy(activeTiles.Dequeue()); // Remove and destroy the oldest tile
            foreach (GameObject bater in activeObj.Peek()) Destroy(bater);
            activeObj.Dequeue().Clear();
            if (activeTiles.Count < numberOfTiles)
                SpawnTile();
        }

        if (Vector3.Distance(player.position, lastTilePosition) < 200) {
            Destroy(activeTiles.Dequeue());
            foreach (GameObject bater in activeObj.Peek()) Destroy(bater);
            activeObj.Dequeue().Clear();
            SpawnTile();
        }
    }

    void SpawnTile()
    {
        int currentTry = 0;

        GameObject prefab = null;
        GameObject newTile = null;
        bool tilePlaced = false;

        while (currentTry < maxTries && !tilePlaced)
        {
            prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
            newTile = Instantiate(prefab, transform);
            newTile.transform.localScale *= 1.5f;

            if (lastTile == null) {
                lastTile = newTile;
                tilePlaced = true;
                lastTilePosition = newTile.transform.position;
                activeTiles.Enqueue(newTile);
                newTile.transform.position = Vector3.zero;
                ////////////////////////////////////////////////////
                SpawnBattery(newTile,true);
                break;
            }

            AlignTile(newTile, lastTile);
            

            // Check for overlap
            if (!IsOverlapping(newTile) || currentTry == maxTries - 1)
            {
                if (IsOverlapping(newTile, true)) { ////// pravi ako se presicha da nqma collider
                    PolygonCollider2D[] plgns = newTile.GetComponents<PolygonCollider2D>();
                    foreach (PolygonCollider2D poly in plgns) {
                        if (!poly.isTrigger) {
                            poly.enabled = false;
                        }
                    }
                }
                lastTilePosition = newTile.transform.position; // Update the endpoint for the next tile
                tilePlaced = true; // Tile successfully placed
                activeTiles.Enqueue(newTile); // Add the tile to the active tiles queue
                ////////////////////////////////////////////////////
                SpawnBattery(newTile);
            }
            else
            {
                // Destroy the unsuitable tile and try again
                Destroy(newTile);
                newTile = null;
                currentTry++;
            }
        }

        // Update the reference to the last tile
        lastTile = newTile;
    }
    public GameObject[] sideCon;
    public string[] sideConNm;

    void SpawnBattery(GameObject newTile, bool dali=false) {
        List<GameObject> lst = new List<GameObject>();
        Transform childTransform = newTile.transform.Find("batteryPoint");
        if (childTransform != null) lst.Add(Instantiate(battery,childTransform.position,Quaternion.identity));
        Transform childTransform1 = newTile.transform.Find("batteryPoint1");
        if (childTransform1 != null) lst.Add(Instantiate(battery, childTransform1.position, Quaternion.identity));
        Transform childTransform2 = newTile.transform.Find("batteryPoint2");
        if (childTransform2 != null) lst.Add(Instantiate(battery, childTransform2.position, Quaternion.identity));
        for (int q = 0; q < sideCon.Length; q++) {
            Transform objTr = newTile.transform.Find(sideConNm[q]);
            if (objTr != null) lst.Add(Instantiate(sideCon[q], objTr.position, Quaternion.identity));
            Transform ObjTr1 = newTile.transform.Find(sideConNm[q] + "1");
            if (ObjTr1 != null) lst.Add(Instantiate(sideCon[q], ObjTr1.position, Quaternion.identity));
            Transform ObjTr2 = newTile.transform.Find(sideConNm[q] + "1");
            if (ObjTr2 != null) lst.Add(Instantiate(sideCon[q], ObjTr2.position, Quaternion.identity));
        }
        activeObj.Enqueue(lst);
        if (dali) {
            Transform child1 = newTile.transform.Find("StartPoint");
            Transform child2 = newTile.transform.Find("Forward");
              carr.position = new Vector3(child1.position.x, child1.position.y, carr.position.z);
            carr.Rotate(0, 0, -90 + 180 * Mathf.Atan((child2.position.y - child1.position.y) / (child2.position.x - child1.position.x)) / Mathf.PI);
            if (child2.position.x < child1.position.x) {
                carr.Rotate(0, 0, 180f);
            }
            camm.position = new Vector3(child1.position.x,child1.position.y,camm.position.z);
        }
    }



    void AlignTile(GameObject newTile, GameObject lastTile) {
        // Get the Tile component of the last tile
        Tile lastTileScript = lastTile.GetComponent<Tile>();
        Tile newTileScript = newTile.GetComponent<Tile>();

        // Step 1: Position Adjustment
        Transform lastEndPoint = lastTileScript.GetEndPoint();
        Transform newStartPoint = newTileScript.GetStartPoint();

        // Calculate the position difference to align the startPoint of the new tile
        Vector3 positionOffset = lastEndPoint.position - newStartPoint.position;

        // Apply the position offset
        newTile.transform.position += positionOffset;

        // Step 2: Rotation Adjustment
        // Match the rotation of the new tile's startPoint to the last tile's endPoint
        Quaternion lastEndPointRotation = lastEndPoint.rotation;
        Quaternion newStartPointRotation = newStartPoint.rotation;

        // Calculate the rotation needed to align the points
        Quaternion rotationOffset = lastEndPointRotation * Quaternion.Inverse(newStartPointRotation);

        // Apply the rotation to the new tile
        newTile.transform.rotation = rotationOffset * newTile.transform.rotation;

        // Step 3: Reapply Position Adjustment
        // After rotating, re-align the position in case the rotation caused a shift
        positionOffset = lastEndPoint.position - newStartPoint.position;
        newTile.transform.position += positionOffset;
    }

    // Function to check for overlap with existing tiles
    bool IsOverlapping(GameObject tile, bool vizh = false) {
        Physics2D.SyncTransforms();
        PolygonCollider2D tilePolygonCollider = tile.GetComponent<PolygonCollider2D>();
        foreach (GameObject activeTile in activeTiles) {
            if (activeTile == tile) // Skip self-checking
                continue;

            PolygonCollider2D activeTilePolygonCollider = activeTile.GetComponent<PolygonCollider2D>();


            //ColliderDistance2D distance = Physics2D.Distance(tilePolygonCollider, activeTilePolygonCollider);

            if (/*distance.isOverlapped*/ /*tilePolygonCollider.IsTouching(activeTilePolygonCollider)*/ Vector3.Distance(tile.transform.position,activeTile.transform.position)<=100f) {
               //Debug.Log($"Collision detected between {tile.name} and {activeTile.name}");
                //if (vizh) Debug.Log(distance.pointA);
                //if (vizh) Debug.Log(distance.pointB);
                //if (vizh) Debug.Log(distance.distance);
                //if (vizh) Debug.Log(distance.isValid);
                return true;
            }
        }
        return false;
    }
}