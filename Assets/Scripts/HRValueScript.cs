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
    public int minimumValue = 70;
    public int maximumValue = 140;
    public int alarmThreshold;
    public bool audioEnabled;
    private float lerpValue = 0f;
    private float moveToTargetTiming = 2f;
    

    private float targetChangeInterval = 120f;
    
    public AudioSource alarmAudioSource;
    public AudioClip alarmClip;

    public AudioClip warningClip;
    // Start is called before the first frame update
    void Start()
    {
        _myText = GetComponent<TMP_Text>();
        
        InvokeRepeating("MoveToTargetValue",0,moveToTargetTiming);
        
        InvokeRepeating("ChangeTarget",0,targetChangeInterval);
    }

   
    private void ChangeTarget()
    {
        lerpValue = 0;
        targetValue = Random.Range(minimumValue, maximumValue);
    }
    private void MoveToTargetValue()
    {
        //We want the reading to move in 1 unit increments,and to be able to go up as well as down 
        lerpValue = Mathf.Abs(1f/(currentValue-targetValue));
        currentValue = (int)Mathf.Lerp(currentValue, targetValue, lerpValue );
        _myText.text = currentValue.ToString();
        if (audioEnabled)
        {
            if (currentValue > alarmThreshold && currentValue < warningThreshold)
            {
                alarmAudioSource.clip = warningClip;
                if (!alarmAudioSource.isPlaying)
                    alarmAudioSource.Play();

            }
            else if (currentValue < alarmThreshold)
            {
                alarmAudioSource.clip = alarmClip;
                if (!alarmAudioSource.isPlaying)
                    alarmAudioSource.Play();

            }
            else
            {
                if (alarmAudioSource.isPlaying)
                    alarmAudioSource.Stop();
            }
        }

    }
}
