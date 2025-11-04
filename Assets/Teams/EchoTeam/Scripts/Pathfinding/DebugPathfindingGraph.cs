using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Echo
{
    public class DebugPathfindingGraph : MonoBehaviour
    {
        #region Fields
        [Range(0.1f, 1f)]
        [SerializeField] private float _cellSize;
        [SerializeField] private Color _cellColor;
        [SerializeField] private Color _unWalkableCell;
        [SerializeField] private Color _neighborColor;
        [SerializeField] private Color _drawPathColor;

        private List<CellData> _debugPath = new();

        private CellData _startCell;
        private CellData _endCell;

        private PathfindingNavigationGraph _pathfindingNavigationGraph;
        #endregion

        #region Properties


        #endregion

        private void Start()
        {
            _pathfindingNavigationGraph = PathfindingNavigationGraph.Instance;

            if (_pathfindingNavigationGraph != null && !_pathfindingNavigationGraph.IsInit)
                _pathfindingNavigationGraph.InitNavigationGraph();
        }

        private void Update()
        {
            if (_pathfindingNavigationGraph == null || !_pathfindingNavigationGraph.IsInit)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _startCell = _pathfindingNavigationGraph.GetCell(mousePos);

                if (_startCell == null)
                {
                    Debug.LogError($"Error : No cell data associated found ! Maybe you clicked outside the map, or the graph isn't correctly snapped. In that case, please inform a programmer !", this);
                }
                else
                {
                    //Debug.Log($"Left click selected cell : Name : {_startCell.Name }");
                }

                CheckBuildPath();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _endCell = _pathfindingNavigationGraph.GetCell(mousePos);

                if (_endCell == null)
                {
                    Debug.LogError($"Error : No cell data associated found ! Maybe you clicked outside the map, or the graph isn't correctly snapped. In that case, please inform a programmer !", this);
                }
                else
                {
                    //Debug.Log($"Right click selected cell : Name : {_endCell.Name}");
                }

                CheckBuildPath();
            }
        }

        private void CheckBuildPath()
        {
            if (_startCell == null || _endCell == null)
            {
                _debugPath.Clear();
                return;
            }

            _debugPath = _pathfindingNavigationGraph.FindPathTo(_startCell.Position, _endCell.Position);
        }

        private void OnDrawGizmos()
        {
            if (_pathfindingNavigationGraph == null || !_pathfindingNavigationGraph.IsInit)
                return;

            DrawCellGizmos();
            DrawPathGizmos();
        }

        private void DrawCellGizmos()
        {
            foreach (KeyValuePair<Vector2, CellData> pair in _pathfindingNavigationGraph.GraphCellList)
            {
                Vector2 position = pair.Key;
                CellData cell = pair.Value;

                if (cell == null)
                    continue;

                if (!cell.Walkable)
                {
                    Gizmos.color = _unWalkableCell;
                }
                else
                {
                    Gizmos.color = _cellColor;
                }

                Gizmos.DrawCube(new Vector3(position.x, position.y), new Vector3(_cellSize, _cellSize));
            }

            Gizmos.color = _neighborColor;

            foreach (KeyValuePair<Vector2, List<CellData>> pair in _pathfindingNavigationGraph.GraphAjacentList)
            {
                foreach (CellData neighbor in pair.Value)
                {
                    Vector2 position = neighbor.Position;

                    Gizmos.DrawCube(new Vector3(position.x, position.y), new Vector3(_cellSize, _cellSize));
                }
            }
        }

        private void DrawPathGizmos()
        {
            if (_debugPath == null || _debugPath.Count <= 0)
                return;

            Gizmos.color = _drawPathColor;

            for (int i = 0;  i < _debugPath.Count; ++i)
            {
                if (i + 1 >= _debugPath.Count)
                    continue;

                Vector3 position = _debugPath[i].Position;
                Vector3 nextPos = _debugPath[i + 1].Position;

                Gizmos.DrawLine(position, nextPos);
            }
        }

    }
}
