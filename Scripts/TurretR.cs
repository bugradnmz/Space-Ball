using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretR : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject blade;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] float spawnTime = 5f;

    void Start()
    {
        InvokeRepeating("SpawnBlade", spawnTime/2, spawnTime); //Spawn blade loop
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z)); //Did for logical/graphical purposes
    }

    private void SpawnBlade()
    { 
        Instantiate(blade, spawnPoint.transform.position, transform.rotation);
    }
}
