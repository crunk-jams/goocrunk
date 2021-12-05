using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoBox : MonoBehaviour
{
    private Color color;
    private Vector3 position;
    private Vector3 size;

    public void DefineBox(Vector3 pos, Vector3 size, Color color)
    {
        this.position = pos;
        this.size = size;
        this.color = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(position, size);
    }
}
