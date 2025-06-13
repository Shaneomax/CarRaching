using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(Waypoint waypoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.blue * 0.5f;
        }

        // Draw the sphere for the waypoint position
        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);

        // Draw the width lines
        Gizmos.color = Color.white;
        Gizmos.DrawLine(
            waypoint.transform.position + (waypoint.transform.right * waypoint.waypointWidth / 2f),
            waypoint.transform.position - (waypoint.transform.right * waypoint.waypointWidth / 2f)
        );

        // Draw lines to the next waypoint
        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;

            Vector3 offset = waypoint.transform.right * waypoint.waypointWidth / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * waypoint.nextWaypoint.waypointWidth / 2f;

            Gizmos.DrawLine(
                waypoint.transform.position + offset,
                waypoint.nextWaypoint.transform.position + offsetTo
            );

            Gizmos.DrawLine(
                waypoint.transform.position - offset,
                waypoint.nextWaypoint.transform.position - offsetTo
            );
        }
    }
}
