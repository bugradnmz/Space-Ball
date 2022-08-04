using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] float speed = 0f;
    Vector3 playerPos;
    Vector3 direction;

    void Start()
    {
        playerPos = GameObject.Find("ball").transform.position; 
        direction = playerPos - transform.position;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
    }
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime; ////Sending all blades towards player right after they spawn, Y axis excluded
    }
}
