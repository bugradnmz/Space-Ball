using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour  //Starts turrets and platform
{
    [SerializeField] TurretL turretL;
    [SerializeField] TurretR turretR;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {            
            GetComponentInParent<Animator>().enabled = true; //Start platform
            turretL.enabled = true; //Start the engines of death wahaha
            turretR.enabled = true;
        }
    }
}
