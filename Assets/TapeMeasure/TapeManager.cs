using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class TapeManager : MonoBehaviour
{
    ARRaycastManager aRRaycastManager;

    public GameObject[] tapePoints;
    public GameObject reticle;

    float distanceBetweenPoints = 0f;

    int currentTapePoint = 0;

    public TMP_Text distanceText;

    public TMP_Text floatingDistanceText;
    public GameObject floatingDistanceObject;

    public LineRenderer line;

    public bool placementEnabled = true;

    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateDistance();
        PlaceFloatingText();

        // shoot a raycast from the center of the screen
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);

        //if the raycast hits a plane, update the reticle
        if (hits.Count > 0)
        {
            reticle.transform.position = hits[0].pose.position;
            reticle.transform.rotation = hits[0].pose.rotation;

            //draw the line to the reticle if the first point is placed
            if (currentTapePoint == 1)
            {
                DrawLine();
            }

            // enable the reticle if its disabled and the tape points aren't placed
            if (!reticle.activeInHierarchy && currentTapePoint < 2)
            {
                reticle.SetActive(true);
            }
                
            //if the user taps, place a tape point. disable more placements until the end of the touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (currentTapePoint < 2)
                {
                    PlacePoint(hits[0].pose.position, currentTapePoint);
                }
                placementEnabled = false;
            }
            else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                placementEnabled = true;
            }
        }

        //if the raycast isn't hitting anything, don't display the reticle
        else if (hits.Count == 0 || currentTapePoint == 2)
        {
            reticle.SetActive(false);
        }

    }

    //change the position of the approperiate tape point and make it active.
    public void PlacePoint(Vector3 pointPosition, int pointIndex)
    {
        tapePoints[pointIndex].SetActive(true);

        tapePoints[pointIndex].transform.position = pointPosition;

        if (currentTapePoint == 1)
        {
            DrawLine();
        }

        currentTapePoint += 1;
    }

    void UpdateDistance()
    {
        if (currentTapePoint == 0)
        {
            distanceBetweenPoints = 0f;
        }
        else if (currentTapePoint == 1)
        {
            distanceBetweenPoints = Vector3.Distance(tapePoints[0].transform.position, reticle.transform.position);
        }
        else if (currentTapePoint == 2)
        {
            distanceBetweenPoints = Vector3.Distance(tapePoints[0].transform.position, tapePoints[1].transform.position);
        }
        
        string distanceStr = distanceBetweenPoints.ToString("#.##") + "m";

        distanceText.text = distanceStr;
        floatingDistanceText.text = distanceStr;

    }

    //set the positions of the line to the tape points (or reticle)
    void DrawLine()
    {
        line.enabled = true;
        line.SetPosition(0, tapePoints[0].transform.position);
        if (currentTapePoint == 1)
        {
            line.SetPosition(1, reticle.transform.position);
            
        }
        else if (currentTapePoint == 2)
        {
            line.SetPosition(1, tapePoints[1].transform.position);

        }
    }

    void PlaceFloatingText()
    {
        if (currentTapePoint == 0)
        {
            floatingDistanceObject.SetActive(false);
        }
        else if (currentTapePoint == 1)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, reticle.transform.position, 0.5f);
        }
        else if (currentTapePoint == 2)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, tapePoints[1].transform.position, 0.5f);
        }

        floatingDistanceObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

    }

}
