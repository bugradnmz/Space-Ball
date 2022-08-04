using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    GameObject basket;
    GameObject ball;
    //Vector3 camVelocity; //smoothdamp referance variable

    [SerializeField] float cameraOffsetXZ, cameraOffsetY;//, smoothTime;
    [HideInInspector] public float ballYPos; //Ball Y position on the ground. We are calling it from BallMechanics.cs, its purpose make camera follow the ball without bouncing or jumping with it.
    public float cameraTilt; //public beacuse ball direction helper need to read this variable    
    
    void Start()
    {
        basket = GameObject.Find("basket");
        ball = GameObject.Find("ball");
    }

    void LateUpdate()//Using camera stuff better in Late Update
    {
        //Position
        Vector3 basketPos = basket.transform.position - new Vector3(0, basket.transform.position.y, 0); //Neglate Y axis of the basket.
        Vector3 ballPos = ball.transform.position - new Vector3(0, ball.transform.position.y, 0); //Neglate Y axis of the ball

        Vector3 direction = -(basketPos - ballPos); //Find the direction and turn it to negative beacuse we want our camera behind the ball relative to basket.
        direction = direction.normalized; //Normalize for use as position multiplier
        Vector3 cameraPosition = ballPos + cameraOffsetXZ * direction; //Setting the new camera position relative to basket and ball on XZ axis
        cameraPosition += new Vector3(0, ballYPos + cameraOffsetY, 0); //Adding camera position Y offset and ball Y position value. We are adding it seperately beacuse we don't want to multiply Y value by direction multiplier
        transform.position = cameraPosition;
        //transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref camVelocity, smoothTime); //Anti-Stuttering for camera if needed. Smoothier positioning

        //Rotation
        Vector3 focusPoint = new Vector3 (basketPos.x,transform.position.y,basketPos.z);
        transform.LookAt(focusPoint);
        transform.rotation *= Quaternion.Euler(cameraTilt, 0, 0); //tilt down the camera despite LookAt function

        /*Draws virtual triangle between basket and ball
        Debug.DrawRay(new Vector3(basketPos.x, 0.1f, basketPos.z), direction, Color.magenta);
        Debug.DrawRay(new Vector3(basketPos.x, 0.1f, basketPos.z) + new Vector3(0, 0, direction.z), new Vector3(direction.x, 0, 0), Color.red);
        Debug.DrawRay(new Vector3(basketPos.x, 0.1f, basketPos.z), new Vector3(0, 0, direction.z), Color.blue);*/
    }
}
