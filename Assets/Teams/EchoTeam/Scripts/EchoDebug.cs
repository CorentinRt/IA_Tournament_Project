using System.Collections.Generic;
using UnityEngine;

public class EchoDebug : MonoBehaviour
{
    private class CircleData
    {
        public Vector3 Center;
        public float Radius;
        public Color Color;
    }

    // ----- FIELDS ----- //
    private Dictionary<string, CircleData> _circles = new Dictionary<string, CircleData> ();
    // ----- FIELDS ----- //

    public string AddCircle(string name, Vector3 center, float radius, Color color, bool canHaveMultiple = false)
    {
        if (!canHaveMultiple && _circles.ContainsKey(name)) return "";

        string newName = name;
        int index = 0;

        // If multiple circles with same name
        while (_circles.ContainsKey(newName))
        {
            index++;
            newName = $"{name}_{index}";
        }

        _circles.Add(newName, (new CircleData
        {
            Center = center,
            Radius = radius,
            Color = color,
        }));

        return newName;
    }

    public void UpdateDebugCirclePosition(string name, Vector3 position)
    {
        if (_circles.ContainsKey(name))
        {
            _circles[name].Center = position;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (KeyValuePair<string, CircleData> keyValue in _circles)
        {
            CircleData circle = keyValue.Value;

            Gizmos.color = circle.Color;
            DrawCircle(circle.Center, circle.Radius);  
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 32)
    {
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / segments);
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
