using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookatcamera : MonoBehaviour
{
    public Camera cam;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position);
           transform.Rotate(0, 180f, 0); // يقلبه صح
        
    }
}
