using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class CellData
    {
        public CellData(Vector2 position, bool walkable = true, int cost = 1)
        {
            this.position = position;
            this.walkable = walkable;
            this.cost = cost;
        }

        private Vector2 position;
        private bool walkable;
        private int cost;

        public Vector2 Position => this.position;
        public bool Walkable => this.walkable;
        public int Cost
        {
            get
            {
                if (!walkable)
                    return 1000;

                return this.cost;
            }
        }

        public string Name
        {
            get
            {
                return $"Cell x : {position.x} , y = {position.y}";
            }
        }
    }

    public class PathfindingNavigationGraph : MonoBehaviour
    {
        #region Fields
        private static PathfindingNavigationGraph _instance;

        private Dictionary<Vector2, CellData> _graphCellList = new();
        private Dictionary<Vector2, List<CellData>> _graphAjacentList = new();

        private bool _isInit = false;

        private Vector2 _graphOrigin = new();
        private Vector3 _screenHalfSize = new();

        private float _cellSize = 0.5f;

        private float _asteroidCollisionRadiusCheck = 0.3f;
        #endregion

        #region Properties
        public static PathfindingNavigationGraph Instance => _instance;

        public Dictionary<Vector2, CellData> GraphCellList => _graphCellList;
        public Dictionary<Vector2, List<CellData>> GraphAjacentList => _graphAjacentList;

        public bool IsInit => _isInit;
        #endregion

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Error : try to create an already existing singleton ! Gameobject has been destroyed !");
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        #region Graph Generation / Init
        public void InitNavigationGraph(float baseCost = 1f)
        {
            if (_graphCellList == null)
                _graphCellList = new Dictionary<Vector2, CellData>();

            if (_graphAjacentList == null)
                _graphAjacentList = new Dictionary<Vector2, List<CellData>>();

            if (_graphCellList != null)
                _graphCellList.Clear();

            if (_graphAjacentList != null)
                _graphAjacentList.Clear();

            _screenHalfSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            _graphOrigin = Camera.main.transform.position - _screenHalfSize;

            float width = _screenHalfSize.x * 2f;
            float height = _screenHalfSize.y * 2f;


            for (float x = 0; x < width; x += _cellSize)
            {
                for (float y = 0; y < height; y += _cellSize)
                {
                    Vector2 currentPosition = SnapToGrid(_graphOrigin + new Vector2(x, y));

                    bool walkable = true;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, _asteroidCollisionRadiusCheck);
                    foreach (var c in colliders)
                    {
                        if (c != null && c.gameObject != null && c.gameObject.CompareTag("Asteroid"))
                        {
                            walkable = false;
                            break;
                        }
                    }

                    _graphCellList[currentPosition] = new CellData(currentPosition, walkable);
                }
            }

            for (float x = 0; x < width; x += _cellSize)
            {
                for (float y = 0; y < height; y += _cellSize)
                {
                    Vector2 currentPosition = SnapToGrid(_graphOrigin + new Vector2(x, y));

                    List<CellData> neighborsData = new List<CellData>();

                    Vector2[] dirs =
                    {
                        new Vector2(_cellSize,0), new Vector2(-_cellSize,0),
                        new Vector2(0,_cellSize), new Vector2(0,-_cellSize),
                        new Vector2(_cellSize,_cellSize), new Vector2(-_cellSize,_cellSize),
                        new Vector2(_cellSize,-_cellSize), new Vector2(-_cellSize,-_cellSize)
                    };

                    foreach (Vector2 dir in dirs)
                    {
                        Vector2 neighborPos = SnapToGrid(currentPosition + dir);

                        if (_graphCellList.TryGetValue(neighborPos, out CellData neighborCellData))
                        {
                            neighborsData.Add(neighborCellData);
                        }
                    }

                    _graphAjacentList[currentPosition] = neighborsData;

                    bool walkable = true;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, _asteroidCollisionRadiusCheck);

                    if (colliders != null && colliders.Length > 0)
                    {
                        foreach (Collider2D collider in colliders)
                        {
                            if (collider == null || collider.gameObject == null)
                                continue;

                            if (collider.gameObject.CompareTag("Asteroid"))
                            {
                                walkable = false;
                                break;
                            }
                        }
                    }

                    _graphCellList[currentPosition] = new CellData(currentPosition, walkable);
                }
            }

            _isInit = true;
        }
        #endregion

        #region Snap to grid
        public Vector2 SnapToGrid(Vector2 pos)
        {
            float x = Mathf.Round(pos.x / _cellSize) * _cellSize;
            float y = Mathf.Round(pos.y / _cellSize) * _cellSize;
            return new Vector2(x, y);
        }
        #endregion

        #region Get path / neighbors
        public bool IsWalkable(Vector2 position)
        {
            if (!_graphCellList.ContainsKey(SnapToGrid(position)))
                return false;

            return _graphCellList[position].Walkable;
        }

        public List<CellData> GetNeighbors(Vector2 cellPosition)
        {
            if (_graphAjacentList == null || !_graphAjacentList.TryGetValue(SnapToGrid(cellPosition), out List<CellData> neighbors))
                return new List<CellData>();

            return neighbors;
        }

        public CellData GetCell(Vector2 cellPosition)
        {
            cellPosition = SnapToGrid(cellPosition);

            if (!_graphCellList.TryGetValue(cellPosition, out CellData startCell))
                return null;

            return startCell;
        }

        public List<CellData> FindPathTo(Vector2 startPosition, Vector2 targetPosition)
        {
            startPosition = SnapToGrid(startPosition);
            targetPosition = SnapToGrid(targetPosition);

            if (!_graphCellList.TryGetValue(startPosition, out CellData startCell))
                return new List<CellData>();

            if (!_graphCellList.TryGetValue(targetPosition, out CellData targetCell))
                return new List<CellData>();

            List<CellData> path = new List<CellData>();

            SortedSet<(float costHeuristic, CellData cellData)> unExplored = new SortedSet<(float, CellData)>(
                Comparer<(float, CellData)>
                .Create((a, b) => a.Item1 == b.Item1 ? a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode()) : a.Item1.CompareTo(b.Item1)));

            HashSet<CellData> visited = new HashSet<CellData>();
            Dictionary<CellData, float> cellDistance = new Dictionary<CellData, float>();
            Dictionary<CellData, CellData> cellParent = new Dictionary<CellData, CellData>();

            unExplored.Add((0f, startCell));
            cellDistance[startCell] = 0;

            int loopCount = 0;

            while (unExplored.Count > 0)
            {
                ++loopCount;

                var currentTuple = unExplored.Min;

                CellData currentCell = currentTuple.cellData;

                if (currentCell == null)
                {
                    //Debug.Log($"No path possible to target : {targetCell.Name} from start : {startCell.Name}");
                    return path;
                }

                //Debug.Log($"Current cell: {currentCell.Name}");

                unExplored.Remove(currentTuple);

                if (currentCell == targetCell)
                {
                    break;
                }

                if (!_graphAjacentList.ContainsKey(currentCell.Position))
                {
                    Debug.LogError($"Error : stop path finding algo because cannot find Current Cell position in adjancent List ! Please, inform and see with a programmer !");
                    break;
                }

                foreach (CellData neighbor in _graphAjacentList[currentCell.Position])
                {
                    if (!neighbor.Walkable || visited.Contains(neighbor))
                        continue;

                    //Debug.Log($"Explore adjacent {neighbor.Name}");
                    float newDistance = cellDistance[currentCell] + neighbor.Cost;
                    
                    float heuristic = Heuristic(neighbor, targetCell);

                    float estimatedScore = newDistance + heuristic;

                    if (!cellDistance.ContainsKey(neighbor) || cellDistance[neighbor] > newDistance)
                    {
                        if (cellDistance.ContainsKey(neighbor))
                        {
                            unExplored.Remove((cellDistance[neighbor] + heuristic, neighbor));
                        }
                        
                        //Debug.Log($"Add parent of {neighbor.Name} to {currentCell.Name}");
                        cellDistance[neighbor] = newDistance;
                        cellParent[neighbor] = currentCell;

                        unExplored.Add((estimatedScore, neighbor));
                    }
                }

                visited.Add(currentCell);

                if (loopCount > 10000)
                {
                    Debug.LogError("A* stopped because of too many iterations (infinite loop security). Please inform a programmer !");
                    break;
                }
            }

            //Debug.Log("-------------------------------");

            CellData currentParent = targetCell;
            path.Add(targetCell);

            //Debug.Log("Goal node : " + targetCell.Name);

            while (currentParent != startCell)
            {
                //Debug.Log($"Parent of {currentParent.Name} is : " + cellParent[currentParent].Name);

                path.Add(cellParent[currentParent]);
                currentParent = cellParent[currentParent];
            }

            path.Reverse();

            //Debug.Log($"Count loop ASTAR : { loopCount }");

            return path;
        }

        private float Heuristic(CellData from, CellData to)
        {
            float dx = to.Position.x - from.Position.x;
            float dy = to.Position.y - from.Position.y;

            return dx * dx + dy * dy;
        }

        #endregion
    }
}
