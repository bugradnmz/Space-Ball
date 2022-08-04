using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMechanics : MonoBehaviour
{
    GameManager gameManager;
    CameraMovement cameraMovement;
    Rigidbody ballRB;
    Collider ballCollider; //will use it for change the bounciness value of the ball when thrown
    GameObject throwTarget; //Target for our throw direction. We will use its position
    GameObject ballClone; //It is not actual ball. We will use this clone for rotate the ball without disturbing the movement of the real ball
    Rigidbody cloneRB; //Rigidbody of clone
    Transform directionHelper; /* We will use it to make movements of ball relative to the direction of the camera. We are using it's child beacuse we don't want the rotation of the camera.
                                * Need only direction and i insist to don't use the rotation of ball for movement direction. Beacuse i want keep ball's rotation free for animate ball
                                * relative to movement direction*/
    Touch touch;
    Vector2 startPosition, direction, distance; //first touch position, direction of finger movement, distance of finger movement
    Vector3 forceDirection;
    Vector3 targetDistance;
    Vector3 moveDirection;
    RaycastHit hit; //Necessary for fix air jump and passing gaps between island without jumping

    [SerializeField] float moveSpeed = 6f; //Movement speed of the ball
    [SerializeField] float rotateSpeed = 150f;
    [SerializeField] float swipeTimeInterval = 0.35f; //Maximum touch time for swipe
    [SerializeField] float moveDelay = 0.1f; //Move delay for fast swipe
    [SerializeField] float swipeDistance = 300; //Minumum resultant vector length in pixels for swipe
    [SerializeField] float forceMultiplier = 250;
    [SerializeField] float forceY = 2.15f;
    [SerializeField] float fallMultiplier = 1.5f;
    [SerializeField] float minRange = 2.5f; //We will use range variables to change throw mechanics depending on balls distance to basket for sure basket
    [SerializeField] float lowerMidRange = 4.5f;
    [SerializeField] float upperMidRange = 7.5f;
    [SerializeField] float maxRange = 11.5f;

    float touchBeganTime, touchEndTime, timeInterval; //Necessary for check if it is swipe or movement
    bool onTheGround; //Will use it for ground snapping on moving platforms, air jump block, and movement speed decrease on gaps
    bool isThrowed;
    bool cheat; //necessary for win condition
    bool targetReached; //necessary for win condition
    bool disableControls; //disable the controls after win

    void Start()
    {
        ballRB = GetComponent<Rigidbody>();
        ballCollider = gameObject.GetComponent<Collider>();
        throwTarget = GameObject.Find("throw_target");        
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        directionHelper = GameObject.Find("ball_direction_helper").transform;
        
        ballClone = transform.GetChild(0).gameObject;
        cloneRB = ballClone.GetComponent<Rigidbody>();
        cloneRB.detectCollisions = false; //Clone will not detect collisions. By that way clone wont rotate it self when it hits something.
    }

    void Update()
    {
        ballClone.transform.position = transform.position;  //updating clone position to real position of the ball
        if(moveDirection == Vector3.zero && !isThrowed) //if there is no movement and isn't throwed the ball, stop rotating of the ball 
            cloneRB.angularVelocity = Vector3.zero;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.6f)) //Raycast bottom of ball in the range of bounce
        {
            Debug.DrawRay(transform.position, new Vector3(0, -1.6f, 0), Color.yellow); //just graphic

            if (hit.collider.tag == "ground") //if it hits the ground
            {
                onTheGround = true;
                transform.parent = hit.transform;
                cameraMovement.ballYPos = hit.transform.position.y; //It changes camera position on vertical platforms as well while bouncing or throwing not changing. We send it to camera script for updating camera.
                //Debug.Log(cameraMovement.ballYPos);
            }
            else //if it hits something else
            {
                if (timeInterval > swipeTimeInterval) //We give throw chance at the edge of platform to player. Ball may slightly move outside of platform before throw.
                {
                    onTheGround = false;
                    transform.parent = null;
                }
            }
        }
        else //if it doesn't hit anything at all
        {
            if (timeInterval > swipeTimeInterval) //We give throw chance at the edge of platform to player. Ball may slightly move outside of platform before throw.
            {
                onTheGround = false;
                transform.parent = null;
            }
        }

        if (ballRB.velocity.y < 0 && isThrowed) //Ball falls faster after throw
        {
            ballRB.velocity += Vector3.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }

        if (Input.touchCount > 0 && isThrowed == false && !disableControls) //can't move the ball after throw until it is grounded
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchBeganTime = Time.time;
                    startPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    touchEndTime = Time.time;
                    timeInterval = touchEndTime - touchBeganTime;
                    direction = touch.position - startPosition;
                    direction = Vector3.Normalize(direction); //Turning pixel difference to normalized vector value
                    moveDirection = directionHelper.forward * direction.y + directionHelper.transform.right * direction.x;

                    if (timeInterval >= moveDelay) //Added some delay to prevent the ball from moving at the start of thrown if it is fast swipe
                    {
                        transform.position += (moveDirection) * moveSpeed * Time.deltaTime; //Move the real ball
                        cloneRB.AddTorque(moveDirection.z * rotateSpeed, 0, -moveDirection.x * rotateSpeed); //Rotate the clone ball                        
                    }
                    break;

                case TouchPhase.Stationary:
                    transform.position += (moveDirection) * moveSpeed * Time.deltaTime; //Move the real ball
                    cloneRB.AddTorque(moveDirection.z * rotateSpeed, 0, -moveDirection.x * rotateSpeed); //Rotate the clone ball 
                    //Debug.Log(moveDirection);
                    //Debug.Log(cloneRB.angularVelocity);
                    break;

                case TouchPhase.Ended:                    
                    distance = touch.position - startPosition;                    
                    //Debug.Log(distance.magnitude);
                    //Debug.Log(timeInterval);

                    if (timeInterval < swipeTimeInterval && distance.magnitude >= swipeDistance && direction.y > 0) //Checking is it swipe or movement and throw the ball if it is upwards swipe
                    {
                        forceDirection = (throwTarget.transform.position - transform.position - new Vector3(0, throwTarget.transform.position.y - transform.position.y, 0)); //Find the target direction, neglate Y axis
                        targetDistance = forceDirection; //Saving target distance for check the ball distance to change throw mechanic
                        //Debug.Log("Magnitude: " + targetDistance.magnitude);
                        forceDirection = forceDirection.normalized; //Normalize the direction
                        if (targetDistance.magnitude > upperMidRange && targetDistance.magnitude <= maxRange) //If we are throwing in range of middle and max range(further then default range), we will tweak force(XZ) for sure basket
                        {
                            //Debug.Log("Throw far range");
                            float maxDistanceDifference = maxRange - upperMidRange;
                            float distanceDifference = targetDistance.magnitude - upperMidRange; //current distance to mid range
                            float multiplier = distanceDifference / maxDistanceDifference; //finding multiplier depending on range
                            multiplier = Mathf.Lerp(1, 1.2f, multiplier); //lerping for tweak the multiplier now therefore force
                            Throw(forceDirection, forceMultiplier * multiplier, forceY);
                        }
                        else if (targetDistance.magnitude <= lowerMidRange)  //if we are throwing from close range we will tweak force(XYZ)
                        {
                            //Debug.Log("Throw close range");
                            float maxDistanceDifference = lowerMidRange - minRange;
                            float distanceDifference = targetDistance.magnitude - minRange;
                            float multiplier = distanceDifference / maxDistanceDifference;
                            float multiplierXZ = Mathf.Lerp(0.44f, 1, multiplier); //set XZ multiplier relative to main multiplier
                            float multiplierY = Mathf.Lerp(2.32f, 1, multiplier);//set Y multiplier relative to main multiplier
                            Throw(forceDirection, forceMultiplier * multiplierXZ, forceY * multiplierY);
                        }
                        else //if we are throwing neither far not close
                        {
                            //Debug.Log("Throw default");
                            Throw(forceDirection, forceMultiplier, forceY);
                        }
                    }
                    moveDirection = Vector3.zero; //Resetting move direction for next touch
                    break;
            }
        }
    }
   
    void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.tag == "ground") //Neglation of velocity after ball touched the ground but continue bouncing. Otherwise ball gain horizontal velocity on slopes
        {
            ballRB.velocity = Vector3.zero;
            ballRB.velocity = transform.up * 4.5f; //Continue bouncing
            ballCollider.material.bounciness = 0.965f;
            isThrowed = false;
            targetReached = false;
        }
        else if (collision.gameObject.tag == "obstacle")
        {
            gameManager.RestartLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "throw_target") //Sure basket
        {
            ballRB.velocity = Vector3.zero;
            ballRB.AddForce(forceDirection.x * 100, 0, forceDirection.z * 100);
            targetReached = true;
            //Debug.Log("Should be Basket!");
        }
        else if(other.gameObject.tag == "collectable" && !disableControls) //collecting after game over isn't possible
        {            
            gameManager.points += 1; //collected astronauts. We will send it to GameManager and it will update user interface with it
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name == "win_trigger" && targetReached && !cheat) //win
        {
            StartCoroutine(Win());
        }
        else if (other.gameObject.name == "deadzone") //mokoko
        {
            gameManager.RestartLevel();
        }
        else if (other.gameObject.name == "anti_cheat") //block reverse basket
        {
            cheat = true;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "anti_cheat" && !isThrowed) //block reverse basket
        {
            cheat = false;
        }
    }

    private void Throw(Vector3 direction, float multiplier, float y)
    {
        if (onTheGround) //Can't jump on gaps between islands
        {
            direction = new Vector3(direction.x, y, direction.z); //Set up force
            //Debug.Log("Force: " + direction);

            ballRB.velocity = Vector3.zero; //Negate velocity of bouncing before throw
            ballRB.AddForce(direction * multiplier);

            cloneRB.angularVelocity = Vector3.zero; //Reset rotation velocity before throw  
            cloneRB.AddTorque(direction.z * rotateSpeed * 100, 0, -direction.x * rotateSpeed * 100); //Rotate the clone ball relative to throw direction

            ballCollider.material.bounciness = 0.3f;
            isThrowed = true;            
        }
    }

    private IEnumerator Win()
    {
        disableControls = true;
        yield return new WaitForSeconds(0.5f);
        gameManager.GameMenu();
    }
}
