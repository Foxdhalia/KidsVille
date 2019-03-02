using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveX;
    private float moveZ;

    [SerializeField] private float speed = 0.5f;

    [Header("Limites: 0 = limite mínimo, 1 = limite máximo")]
    [SerializeField] private float[] x_limits = new float[2];
    [SerializeField] private float[] z_limits = new float[2];
    
        
    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        transform.position += new Vector3(moveX, 0f, moveZ) * speed * Time.deltaTime;

        if(transform.position.x < x_limits[0])
        {
            transform.position = new Vector3(x_limits[0], transform.position.y, transform.position.z);
        }
        if (transform.position.x > x_limits[1])
        {
            transform.position = new Vector3(x_limits[1], transform.position.y, transform.position.z);
        }
        if (transform.position.z < z_limits[0])
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z_limits[0]);
        }
        if (transform.position.z > z_limits[1])
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z_limits[1]);
        }
    }
}
