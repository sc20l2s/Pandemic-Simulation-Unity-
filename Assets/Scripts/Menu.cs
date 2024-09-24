using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    //our menu 
    private bool showMenu;
    public GameObject menu;
    public GameObject CamReference; //need to stop the camera moving when the menu is shown

    private void Start()
    {
        showMenu = false;
        menu.SetActive(false);//hide menu on startup
        //get component on startup so we do not have to do it every time
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            showMenu = !showMenu;
            menu.SetActive(showMenu);
            //toggle menu on and
            if(showMenu)
            {
                Cursor.visible = true;
            }
        }
    }    
}
