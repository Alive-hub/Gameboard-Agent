using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator gunCharacterAnimation;
    public float speedThreshold = 0.1f; // The speed threshold to change animation

    private Vector3 lastPosition;  // Store the last position
    private float lastTime;        // Store the last time position was recorded

    private WaitForSeconds quartSec = new WaitForSeconds(.25f);

    
    //Control the animation state, so that 
    private GunCharacterAnimationState AnimationState;
    void Start()
    {
        gunCharacterAnimation = GetComponent<Animator>();
        lastPosition = transform.position;  // Initialize last position
        lastTime = Time.time;
        StartCoroutine(ControlState());
        AnimationState = GunCharacterAnimationState.isIdle;
    }
    
    public enum GunCharacterAnimationState
    {
        isIdle, 
        isRunning,
    }
    
    IEnumerator ControlState()
    {
        while (true)
        {
            // Calculate elapsed time
            float elapsedTime = Time.time - lastTime;

            // Calculate the distance traveled since the last frame
            float distance = Vector3.Distance(transform.position, lastPosition);

            // Calculate the speed
            float speed = distance / elapsedTime;

            // Update last time and last position for the next frame
            lastTime = Time.time;
            lastPosition = transform.position;
            
            Debug.Log("CurrentState is => " + AnimationState);

            
            //Change Animation based on speed
            if (speed > speedThreshold)
            {
                if (AnimationState != GunCharacterAnimationState.isRunning)
                {
                    Debug.Log("Set to is Running, Animation State => " + AnimationState);
                    ChangeToState("setRun");
                    AnimationState = GunCharacterAnimationState.isRunning;
                }
            }
            else if(AnimationState != GunCharacterAnimationState.isIdle)
            {
                Debug.Log("Set to is Idle =>" + AnimationState);
                ChangeToState("setIdle");
                AnimationState = GunCharacterAnimationState.isIdle;

            }
            
            yield return quartSec;
        }
        yield return null;
    }

    //Change one bool to true and the others to false
    void ChangeToState(string setToState)
    {
        foreach (var param in gunCharacterAnimation.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                gunCharacterAnimation.SetBool(param.name, param.name == setToState);
            }
        }
    }
}





