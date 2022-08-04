using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDisable : MonoBehaviour  //Disables turret when player reached to basket island
{
    TurretL turretL;
    TurretR turretR;

    void Start()
    {
        turretL = GameObject.Find("turretL").GetComponent<TurretL>();
        turretR = GameObject.Find("turretR").GetComponent<TurretR>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            turretL.CancelInvoke(); //Cancel spawn
            turretR.CancelInvoke();
            turretL.enabled = false; //Disable turret rotating
            turretR.enabled = false;
        }
    }
}
