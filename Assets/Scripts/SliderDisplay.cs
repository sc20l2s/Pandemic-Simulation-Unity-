using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderDisplay : MonoBehaviour
{
    TextMeshProUGUI display;
    // Start is called before the first frame update
    void Start()
    {
        display = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void displayText(float value)
    {
        display.text = value.ToString();
    }
}
