using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.Extensions.Gameboard;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private GameboardAgent agent; 
    
    [SerializeField]
    private List<Vector3> wayPoints = new();
    
    [SerializeField] private int currentWayPointIndex = 0;
    [SerializeField] private Vector3 currentWayPoint; 

    private WaitForSeconds half = new WaitForSeconds(.5f);
    
    private bool isPlaced = false;
    // Update is called once per frame

    private void Start()
    {
        PlacementOnMesh_Character.characterPlaced += StartAfterPlacement;
    }


    private void StartAfterPlacement()
    {
        agent = GameObject.FindObjectOfType<GameboardAgent>();
        isPlaced = true;
        StartCoroutine(MoveAlongWayPoints());
    }

    void Update()
    {
        if (!isPlaced) return;
        
        
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI Hit was recognized");
                return;
            }
            TouchToRay(Input.mousePosition);
        }
#endif
#if UNITY_IOS || UNITY_ANDROID
        
        if (Input.touchCount > 0 && Input.touchCount < 2 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = touch.position;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0) {
                // We hit a UI element
                Debug.Log("We hit an UI Element");
                return;
            }
            
            Debug.Log("Touch detected, fingerId: " + touch.fingerId);  // Debugging line


            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("Is Pointer Over GOJ, No placement ");
                return;
            }
            TouchToRay(touch.position);
        }
#endif
    }

    IEnumerator MoveAlongWayPoints()
    {
        while (true)
        {
            Debug.Log("AgentState is " + agent.State); 
            Debug.Log("AgentPathStatus is " + agent.path.PathStatus);
            
            if (wayPoints.Count == 0) yield return half;

            if (agent.path.PathStatus == Path.Status.PathComplete &&
                currentWayPointIndex+1 <= wayPoints.Count)
            {
                Debug.Log("agentState => Pathcomlete");
                agent.SetDestination(wayPoints[currentWayPointIndex]);
                currentWayPointIndex++;
            } else if (agent.State == GameboardAgent.AgentNavigationState.Idle 
                       && wayPoints.Count > currentWayPointIndex)
            {
                Debug.Log("agentState => Idle");
                agent.SetDestination(wayPoints[currentWayPointIndex]);
                currentWayPointIndex++;
            }
            yield return half;
        }
        yield return null; 
    }
    
    void TouchToRay(Vector3 touch)
    {
        Ray ray = mainCam.ScreenPointToRay(touch);
        RaycastHit hit;
        
        if (Physics.Raycast(ray ,out hit))
        {
            wayPoints.Add(hit.point);
            isPlaced = true;
        }
    }
}
