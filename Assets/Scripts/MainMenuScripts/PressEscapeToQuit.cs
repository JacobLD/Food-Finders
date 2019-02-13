using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressEscapeToQuit : MonoBehaviour {

    private void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
    }
}
