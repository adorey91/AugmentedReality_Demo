using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 scaleChange = new Vector3(0.5f, 0.5f, 0.5f);
    private ARRaycastManager arRayManager;
    private ARPlaneManager arPlaneManager;
    private List<ARRaycastHit> hits = new();
   [SerializeField] private bool isItemPlaced;
    private Pose poseSaved;
    private GameObject obj;
    [SerializeField] private GameObject selectedObject = null;



    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arRayManager = GetComponent<ARRaycastManager>();
    }


    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        Vector2 touchPosition = finger.currentTouch.screenPosition;

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f);

            if (hit.collider.gameObject.CompareTag("Selectable"))
            {
                SelectObject(hit.collider.gameObject);
                return;
            }
        }

        if (arRayManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            if (!isItemPlaced)
            {
                Pose pose = hits[0].pose;
                poseSaved = pose;

                obj = Instantiate(prefab, pose.position, pose.rotation);
                obj.tag = "Selectable";
                isItemPlaced = true;
            }

            //else
            //{

            //    if (hits[0].pose == poseSaved)
            //    {
            //        obj.transform.localScale += scaleChange;
            //    }
            //}
            
            //foreach (var hit in hits)
            //{
            //    Pose pose = hit.pose;
            //GameObject obj = Instantiate(prefab, pose.position, pose.rotation);
            //}
        }
    }

    private void SelectObject(GameObject obj)
    {
        // Deselect currently selected object (if any)
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        // Select the new object
        selectedObject = obj;

        // Provide visual feedback
        selectedObject.GetComponent<Renderer>().material.color = Color.green;
    }
}
