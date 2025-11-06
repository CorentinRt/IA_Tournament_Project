using DoNotModify;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class CellData
    {
        public CellData(Vector2 position, bool walkable = true, int cost = 1)
        {
            this._position = position;
            this._walkable = walkable;
            this._cost = cost;
        }

        private Vector2 _position;
        private bool _walkable;
        private int _cost;

        public Vector2 Position { get => _position; set => _position = value; }
        public bool Walkable { get => _walkable; set => _walkable = value; }
        public int Cost
        {
            get
            {
                if (!_walkable)
                    return 1000;

                return this._cost;
            }

            set
            {
                _cost = value;
            }
        }

        public string Name
        {
            get
            {
                return $"Cell x : {_position.x} , y = {_position.y}";
            }
        }
    }

    public class PathfindingNavigationGraph : MonoBehaviour
    {
        #region Fields
        private Dictionary<Vector2, CellData> _graphCellDict = new();
        private Dictionary<Vector2, List<CellData>> _graphAjacentDict = new();

        private bool _isInit = false;

        private Vector2 _graphOrigin = new();
        private Vector3 _screenHalfSize = new();

        [SerializeField] private float _cellSize = 0.5f;

        [Header("Asteroids")]
        [SerializeField] private float _asteroidCollisionRadiusCheck = 0.3f;

        [Header("Waypoints")]
        [SerializeField] private float _waypointRadiusCheck = 0.3f;
        [SerializeField] private int _waypointNearCost = 1;

        [Header("Mines")]
        [SerializeField] private float _minesCheckRadius = 0.1f;
        [SerializeField] private int _mineNearCost = 6;

        private List<AsteroidView> _asteroids = new();
        private List<WayPointView> _waypoints = new();

        private EchoData _data;
        #endregion

        #region Properties
        public Dictionary<Vector2, CellData> GraphCellDict => _graphCellDict;
        public Dictionary<Vector2, List<CellData>> GraphAjacentDict => _graphAjacentDict;

        public bool IsInit => _isInit;
        #endregion

        private void Awake()
        {
            _data = gameObject.GetComponent<EchoData>();

            if (_data == null)
            {
                Debug.LogError("Error : Could not find EchoData component on controller ! Please check the component is on the gameObject !");
            }

        }

        private void Start()
        {
            InitNavigationGraph();
        }

        #region Graph Generation / Init
        public void InitNavigationGraph()
        {
            if (_isInit)
                return;

            if (_graphCellDict == null)
                _graphCellDict = new Dictionary<Vector2, CellData>();

            if (_graphAjacentDict == null)
                _graphAjacentDict = new Dictionary<Vector2, List<CellData>>();

            if (_graphCellDict != null)
                _graphCellDict.Clear();

            if (_graphAjacentDict != null)
                _graphAjacentDict.Clear();

            _asteroids = _data.GetAsteroids();
            _waypoints = _data.GetWayPoints();

            _screenHalfSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

            _graphOrigin = Camera.main.transform.position - _screenHalfSize;

            float width = _screenHalfSize.x * 2f;
            float height = _screenHalfSize.y * 2f;

            // Init Dictionary with CellDatas
            for (float x = 0; x < width; x += _cellSize)
            {
                for (float y = 0; y < height; y += _cellSize)
                {
                    Vector2 currentPosition = SnapToGrid(_graphOrigin + new Vector2(x, y));

                    _graphCellDict[currentPosition] = new CellData(currentPosition);
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

                        if (_graphCellDict.TryGetValue(neighborPos, out CellData neighborCellData))
                        {
                            neighborsData.Add(neighborCellData);
                        }
                    }

                    _graphAjacentDict[currentPosition] = neighborsData;

                    bool walkable = true;

                    int cost = 2;

                    foreach (DoNotModify.AsteroidView asteroid in _asteroids)
                    {
                        float distance = Vector2.Distance(asteroid.Position, currentPosition);

                        if (distance < _asteroidCollisionRadiusCheck + asteroid.Radius)
                        {
                            walkable = false;
                            break;
                        }
                    }

                    foreach (DoNotModify.WayPointView wayPoint in _waypoints)
                    {
                        if (Vector2.Distance(wayPoint.Position, currentPosition) < _waypointRadiusCheck + wayPoint.Radius)
                        {
                            cost = _waypointNearCost;
                            break;
                        }
                    }

                    _graphCellDict[currentPosition].Position = currentPosition;
                    _graphCellDict[currentPosition].Walkable = walkable;
                    _graphCellDict[currentPosition].Cost = cost;
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
            if (!_graphCellDict.ContainsKey(SnapToGrid(position)))
                return false;

            return _graphCellDict[position].Walkable;
        }

        public List<CellData> GetNeighbors(Vector2 cellPosition)
        {
            if (_graphAjacentDict == null || !_graphAjacentDict.TryGetValue(SnapToGrid(cellPosition), out List<CellData> neighbors))
                return new List<CellData>();

            return neighbors;
        }

        public CellData GetCell(Vector2 cellPosition)
        {
            cellPosition = SnapToGrid(cellPosition);

            if (!_graphCellDict.TryGetValue(cellPosition, out CellData startCell))
                return null;

            return startCell;
        }

        public List<CellData> FindPathTo(Vector2 startPosition, Vector2 targetPosition)
        {
            if (!_isInit)
                InitNavigationGraph();

            float width = _screenHalfSize.x * 2f;
            float height = _screenHalfSize.y * 2f;

            startPosition.x = Mathf.Clamp(startPosition.x, _graphOrigin.x, _graphOrigin.x + width);
            startPosition.y = Mathf.Clamp(startPosition.y, _graphOrigin.y, _graphOrigin.y + height);
            targetPosition.x = Mathf.Clamp(targetPosition.x, _graphOrigin.x, _graphOrigin.x + width);
            targetPosition.y = Mathf.Clamp(targetPosition.y, _graphOrigin.y, _graphOrigin.y + height);

            Debug.DrawLine(startPosition, targetPosition, Color.magenta);

            startPosition = SnapToGrid(startPosition);
            targetPosition = SnapToGrid(targetPosition);

            if (!_graphCellDict.TryGetValue(startPosition, out CellData startCell) || !_graphCellDict.TryGetValue(targetPosition, out CellData targetCell))
            {
                List<CellData> errorPath = new List<CellData>
                {
                    new CellData(startPosition),
                    new CellData(targetPosition)
                };
                return errorPath;
            }

            List<CellData> path = new List<CellData>();

            if (startCell == targetCell)
            {
                path.Add(startCell);
                return path;
            }

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

                /*
                if (currentCell == null)
                {
                    //Debug.Log($"No path possible to target : {targetCell.Name} from start : {startCell.Name}");
                    return path;
                }
                */

                //Debug.Log($"Current cell: {currentCell.Name}");

                unExplored.Remove(currentTuple);

                if (currentCell == targetCell)
                {
                    break;
                }

                if (!_graphAjacentDict.ContainsKey(currentCell.Position))
                {
                    Debug.LogError($"Error : stop path finding algo because cannot find Current Cell position in adjancent List ! Please, inform and see with a programmer !");
                    break;
                }

                foreach (CellData neighbor in _graphAjacentDict[currentCell.Position])
                {
                    if (!neighbor.Walkable || visited.Contains(neighbor))
                        continue;

                    //Debug.Log($"Explore adjacent {neighbor.Name}");

                    neighbor.Cost = 2;

                    if (CheckMineNear(neighbor.Position))
                    {
                        neighbor.Cost = _mineNearCost;
                    }
                    else if (CheckWaypointNear(neighbor.Position))
                    {
                        neighbor.Cost = _waypointNearCost;
                    }

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

            if (!cellParent.ContainsKey(currentParent))
            {

                return path;
            }

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

        private bool CheckWaypointNear(Vector2 position)
        {
            foreach (var waypoint in _waypoints)
            {
                float sqrDist = (position - waypoint.Position).sqrMagnitude;
                float maxDist = _waypointRadiusCheck + waypoint.Radius;

                if (sqrDist <= maxDist)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckMineNear(Vector2 position)
        {
            foreach (var mine in _data.GetMines())
            {
                float sqrDist = (position - mine.Position).sqrMagnitude;
                float maxDist = _minesCheckRadius + mine.ExplosionRadius;

                if (sqrDist <= maxDist)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
