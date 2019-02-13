using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEaten : MonoBehaviour {

    SpriteRenderer sr;
    BoxCollider2D cd;

    private void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        cd = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update () {
        if (Input.GetButtonDown("Reset"))
        {
            sr.enabled = true;
            cd.enabled = true;
            //Debug.Log("Food reset");
        }
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        sr.enabled = false;
        cd.enabled = false;
    }
}
