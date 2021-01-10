using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float time;
    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.text = "Time: " + time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
