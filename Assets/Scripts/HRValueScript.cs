using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HRValueScript : MonoBehaviour
{
    private TMP_Text _myText;

    public int targetValue;
    public int currentValue;
    public int warningThreshold;
    public int alarmThreshold;

    private float lerpValue = 0f;
    private float moveToTargetTiming = 2f;

    private float targetChangeInterval = 120f;
    // Start is called before the first frame update
    void Start()
    {
        _myText = GetComponent<TMP_Text>();
        InvokeRepeating("MoveToTargetValue",0,moveToTargetTiming);
        
        InvokeRepeating("ChangeTarget",targetChangeInterval,targetChangeInterval);
    }

   
    private void ChangeTarget()
    {
        lerpValue = 0;
        targetValue = Random.Range(40, 130);
    }
    private void MoveToTargetValue()
    {
        //We want the reading to move in 1 unit increments,and to be able to go up as well as down 
        lerpValue = Mathf.Abs(1f/(currentValue-targetValue));
        currentValue = (int)Mathf.Lerp(currentValue, targetValue, lerpValue );
        _myText.text = currentValue.ToString();
        if (currentValue>  alarmThreshold && currentValue < warningThreshold)
        {
            //FAO SM: Add audio here
            print("Warning Sound");
        }
        else if (currentValue< alarmThreshold)
        {
            //FAO SM: Add audio here
            print("Alarm Sound");
        }
    }
}
