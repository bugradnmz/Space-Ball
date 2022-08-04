using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class neutralizes the value of parents camera tilt variable (x rotation). 
 * Beacuse we are using this game object to help direction of the ball.
 * If we don't neutralize then ball will move on the Y axis beacuse camera is tilted downward*/

public class BallDirectionHelper : MonoBehaviour
{
    GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        transform.Rotate(-cam.GetComponent<CameraMovement>().cameraTilt, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
