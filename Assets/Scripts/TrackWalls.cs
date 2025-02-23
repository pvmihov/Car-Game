using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWalls : MonoBehaviour
{
    public LineRenderer lineRenderer1;
    public float wideness = 4f;
    public float ind = 1f;
    public int PointsToCheckCross = 150;

    public List<Vector3> SetPoints(List<Vector3> points) {
        List<Vector3> pointsL= new List<Vector3>(); 
        List<int> pointsDl= new List<int>();
        List<Vector3> pointsR= new List<Vector3>(); 
        //points.Add(new Vector3(0,0,0));
        Vector3 lstDlt = new Vector3(0, 0, 0);
        for (int q = 0; q < points.Count - 1; q++) {
            if ((q+1) % 21 == 0) continue;
            Vector3 lP = new Vector3(0,0,0);
            if (points[q].x == points[q+1].x) {
                if (points[q].y == points[q+1].y) {
                    continue;
                    //if (q<30) Debug.Log(lstDlt);
                    //lP = points[q] + lstDlt;
                    //if (q<30) Debug.Log(lP);
                }
                else if (points[q].y < points[q+1].y) {
                    lP = points[q] + ind * (new Vector3(wideness, 0 , 0));
                }
                else {
                    lP = points[q] + ind * (new Vector3(-1*wideness, 0, 0));
                }
            }
            else if (points[q].x < points[q+1].x) {
                if (points[q + 1].y != points[q].y) {
                    if (points[q + 1].y > points[q].y) {
                        float slope = (points[q + 1].y - points[q].y) / (points[q + 1].x - points[q].x);
                        float k = -1 / slope;
                        float a = Mathf.Sqrt(wideness * wideness / (k * k + 1));
                        lP = points[q] + ind * (new Vector3(a, a * k, 0));
                    }
                    else {
                        float slope = (points[q + 1].y - points[q].y) / (points[q + 1].x - points[q].x);
                        float k = -1 / slope;
                        float a = -1*Mathf.Sqrt(wideness * wideness / (k * k + 1));
                        lP = points[q] + ind * (new Vector3(a, a * k, 0));
                    }
                }
                else {
                     lP = points[q] + ind * (new Vector3(0, -1*wideness, 0));
                }
            }
            else {
                if (points[q + 1].y != points[q].y) {
                    if (points[q + 1].y > points[q].y) {
                        float slope = (points[q + 1].y - points[q].y) / (points[q + 1].x - points[q].x);
                        float k = -1 / slope;
                        float a =  Mathf.Sqrt(wideness * wideness / (k * k + 1));
                         lP = points[q] + ind * (new Vector3(a, a * k, 0));
                    } else {
                        float slope = (points[q + 1].y - points[q].y) / (points[q + 1].x - points[q].x);
                        float k = -1 / slope;
                        float a = -1* Mathf.Sqrt(wideness * wideness / (k * k + 1));
                         lP = points[q] + ind * (new Vector3(a, a * k, 0));
                    }
                } else {
                     lP = points[q] + ind * (new Vector3(0, wideness, 0));
                }
            }
            Vector3 curDlt = lP - points[q];
            //if (q!=0 && Vector3.Distance(lP, pointsL[q-1]) > 1) lP = points[q] + lstDlt;
            pointsL.Add(lP);
            pointsDl.Add(0);
            lstDlt = lP - points[q];
        }
        for (int q=0;q<pointsL.Count-1;q++) {
            int mks = 0;
            if (q - PointsToCheckCross > mks) mks = q - PointsToCheckCross;
            for (int w=mks;w<q-1;w++) {
                Vector3 intersection;
                Vector3 aDiff = pointsL[q+1] - pointsL[q];
                Vector3 bDiff = pointsL[w+1] - pointsL[w];
                if (LineLineIntersection(out intersection, pointsL[q], aDiff, pointsL[w], bDiff)) {
                    float aSqrMagnitude = aDiff.sqrMagnitude;
                    float bSqrMagnitude = bDiff.sqrMagnitude;

                    if ((intersection - pointsL[q]).sqrMagnitude <= aSqrMagnitude
                         && (intersection - pointsL[q+1]).sqrMagnitude <= aSqrMagnitude
                         && (intersection - pointsL[w]).sqrMagnitude <= bSqrMagnitude
                         && (intersection - pointsL[w+1]).sqrMagnitude <= bSqrMagnitude)
                    {
                        //Debug.Log(w);
                        //Debug.Log(w);
                        //Debug.Log(q);
                        for (int e = w; e < q; e++) pointsDl[e] = q;
                        break;
                    }
                }
            }
        }
        for (int q=0;q<pointsL.Count;q++) {
            if (pointsDl[q]==0) pointsR.Add(pointsL[q]);
            else pointsR.Add(pointsL[ pointsDl[q] ]);
        }
        DrawTrack1(pointsR);
        return pointsR;
    }

    private void DrawTrack1(List<Vector3> Points) {
        lineRenderer1.positionCount = Points.Count;
        lineRenderer1.SetPositions(Points.ToArray());
        //lineRenderer1.startColor = Color.black;
        //lineRenderer1.endColor = Color.black;
        lineRenderer1.startWidth = 0.63f;
        lineRenderer1.endWidth = 0.63f;
    }


    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
            Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f) {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                    / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        } else {
            intersection = Vector3.zero;
            return false;
        }
    }
}
