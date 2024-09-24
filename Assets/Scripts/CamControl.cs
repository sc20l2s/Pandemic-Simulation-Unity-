using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float speed = 30.0f;
    public float sensitivity = 5.0f;
    public bool mouseMovement;

    private void Start()
    {
        mouseMovement = false;//make sure it is set to false when the scene restarts
    }
    // Update is called once per frame
    void Update()
    {
        // Rotate the camera based on the mouse movement
        
        if(mouseMovement)//toggle cam movement on and off
        {
            //code snippet taken from
            //https://medium.com/@mikeyoung_97230/creating-a-simple-camera-controller-in-unity3d-using-c-ec1a79584687
            //by Mike Young
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);//change axis mocvement
            transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime; 
            transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            //end of code snippet
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cursor.visible = mouseMovement; //invis if mouse movement enabled, visible otherwise
            mouseMovement = !mouseMovement; //toggle mouse movement on and off  
        }
    }
}
