using System.Collections.Generic;
using UnityEngine;

public class CurvedTrack : MonoBehaviour {
    public int numberOfPoints = 100; // Total number of points
    public float distanceBetweenPoints = 5f; // Spacing between points
    public float curveStrength = 0.3f; // Curviness of the track (lower = less intersection)
    public float maxTurnAngle = 30f; // Maximum turn angle in degrees to constrain sharp turns
    public LineRenderer lineRenderer; // Reference to LineRenderer component
    public TrackWalls trackWalls;
    public TrackWalls trackWalls2;

    public GameObject battery;
    public int distBat = 100;

    private List<Vector3> controlPoints = new List<Vector3>();
    private List<Vector3> smoothPoints = new List<Vector3>();
    public RoadColor roadCol;

    public Transform carr;
    public Transform camm;

    Vector3 direction = Vector3.right;

    void Start() {
        // Generate the track points
        controlPoints = GenerateTrackPoints(Vector3.zero, numberOfPoints, distanceBetweenPoints, curveStrength);

        // Interpolate points for smooth curves
        smoothPoints = InterpolatePoints(controlPoints, 20);
        for (int q = 200; q < smoothPoints.Count; q+=100) {
         
                Instantiate(battery, smoothPoints[q],Quaternion.identity);
        }

        // Draw the track as a solid white line
        GenerateSideC(smoothPoints,smoothPoints);
        DrawTrack(smoothPoints);
        carr.position = smoothPoints[110];
        carr.Rotate(0, 0, -90+ 180 * Mathf.Atan((smoothPoints[111].y - smoothPoints[110].y) / (smoothPoints[111].x - smoothPoints[110].x)) / Mathf.PI);
        camm.position = smoothPoints[110];


        //call the program to create the walls
        List<Vector3> pointsL = trackWalls.SetPoints(smoothPoints);
        List<Vector3> pointsR = trackWalls2.SetPoints(smoothPoints);
        roadCol.GenerateRoadMesh(pointsL, pointsR);
    }

    private void FixedUpdate() {
        if (Vector3.Distance(carr.position, smoothPoints[smoothPoints.Count - 1]) < 150f) AddPoints();
    }

    public void AddPoints() {
        List<Vector3> newControlPoints = new List<Vector3>();
        newControlPoints = GenerateTrackPoints(controlPoints[ controlPoints.Count-3 ] , numberOfPoints, distanceBetweenPoints, curveStrength);
        /*Debug.Log(controlPoints[controlPoints.Count - 2]);
        Debug.Log(controlPoints[controlPoints.Count - 1]);
        Debug.Log(newControlPoints[0]);
        Debug.Log(newControlPoints[1]);*/
        List<Vector3> newSmoothPoints = new List<Vector3>();
        newSmoothPoints = InterpolatePoints(newControlPoints, 20);
        /*Debug.Log(smoothPoints[smoothPoints.Count-2]);
        Debug.Log(smoothPoints[smoothPoints.Count-1]);
        Debug.Log(newSmoothPoints[0]);
        Debug.Log(newSmoothPoints[1]);*/
        for (int q = 0; q < newSmoothPoints.Count; q+=100) {
           
                Instantiate(battery, newSmoothPoints[q], Quaternion.identity);
            
        }
        List<Vector3> allSmoothPoints = new List<Vector3>();
        for (int q = 0; q < smoothPoints.Count; q++) {
            allSmoothPoints.Add(smoothPoints[q]);
        }
        for (int q = 0; q < newSmoothPoints.Count; q++) {
            allSmoothPoints.Add(newSmoothPoints[q]);
        }
        GenerateSideC(newSmoothPoints,allSmoothPoints);
        // Draw the track as a solid white line
        DrawTrack(allSmoothPoints);
        //call the program to create the walls
        List<Vector3> pointsL=trackWalls.SetPoints(allSmoothPoints);
        List<Vector3> pointsR = trackWalls2.SetPoints(allSmoothPoints);
        roadCol.GenerateRoadMesh(pointsL, pointsR);

        controlPoints = newControlPoints;
        smoothPoints = newSmoothPoints;
    }

    public GameObject[] sideCon;
    //public string[] sideConNm;

    public void GenerateSideC(List<Vector3> points, List<Vector3> points2) {
        for (int q = 0; q < 10; q++) {
            while (true) {
                int ind = Random.Range(0, points.Count - 100);
                Vector3 dir = new Vector3(1, Random.Range(-3f,3f), 0);
                dir.Normalize();
                dir *= 35;
                bool dali = true;
                for (int i = 0; i < points.Count; i++) {
                    if (Vector3.Distance(points[ind] + dir, points[i])<25f) {
                        dali = false;
                        break;
                    }
                }
                if (dali) Instantiate(sideCon[Random.Range(0, sideCon.Length)], points[ind] + dir, Quaternion.identity).transform.Rotate(0,0,Random.Range(0f,360f));
                if (dali) break;
            }
        }
    }

    // Generate procedural track points
    private List<Vector3> GenerateTrackPoints(Vector3 start, int numPoints, float distance, float curveStrength) {
        List<Vector3> points = new List<Vector3>();
        //points.Add(start);
        Vector3 lastPoint = start;
        points.Add(start);


        for (int i = 1; i < numPoints; i++) {
            // Generate a random direction change within constraints
            Vector3 randomOffset = new Vector3(
                Random.Range(-curveStrength, curveStrength),
                Random.Range(-curveStrength, curveStrength),
                0
            );

            direction += randomOffset;
            direction.Normalize(); // Normalize the direction vector

            // Constrain the turn angle
            float angle = Vector3.Angle(Vector3.right, direction);
            if (angle > maxTurnAngle) {
                // Clamp the direction to the maximum turn angle
                direction = Vector3.Slerp(Vector3.right, direction, maxTurnAngle / angle);
            }

            // Calculate next point
            Vector3 nextPoint = lastPoint + direction * distance;
            if (start != Vector3.zero && i == 1) { nextPoint = controlPoints[controlPoints.Count - 2]; direction = (nextPoint - lastPoint); direction.Normalize(); }
            nextPoint.z = 0; // Ensure Z remains 0
            points.Add(nextPoint);
            lastPoint = nextPoint;
        }

        return points;
    }

    // Interpolate points using Catmull-Rom spline
    private List<Vector3> InterpolatePoints(List<Vector3> points, int segmentsPerCurve) {
        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = 0; i < points.Count - 3; i++) {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];
            Vector3 p2 = points[i + 2];
            Vector3 p3 = points[i + 3];

            for (int j = 0; j <= segmentsPerCurve; j++) {
                float t = j / (float)segmentsPerCurve;
                smoothPoints.Add(CatmullRomSpline(p0, p1, p2, p3, t));
            }
        }

        return smoothPoints;
    }

    // Catmull-Rom spline function
    private Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 ans= 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
        ans.z = -2;
        return ans;
    }

    // Draw the track using LineRenderer
    private void DrawTrack(List<Vector3> smoothPoints) {
        lineRenderer.positionCount = smoothPoints.Count;
        lineRenderer.SetPositions(smoothPoints.ToArray());
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
    }
}
