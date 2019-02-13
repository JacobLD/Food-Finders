using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour {

    public Slider sl;
    public Text tx;
    public string words;

    private void Update()
    {
        tx.text = words + " " + sl.value;
    }
}
