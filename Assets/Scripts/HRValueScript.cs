using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HRValueScript : MonoBehaviour
{
    [SerializeField] private int _targetValue;
    [SerializeField] private int _currentValue;
    [SerializeField] private int _warningThreshold;
    [SerializeField] private int _minimumValue = 70;
    [SerializeField] private int _maximumValue = 140;
    [SerializeField] private int _alarmThreshold;
    [SerializeField] private bool _audioEnabled;
    [SerializeField] private AudioSource _alarmAudioSource;
    [SerializeField] private AudioClip _alarmClip;
    [SerializeField] private AudioClip _warningClip;

    private float _lerpValue = 0f;
    private float _moveToTargetTiming = 2f;
    private float _targetChangeInterval = 120f;
    private TMP_Text _myText;

    // Start is called before the first frame update
    void Start()
    {
        _myText = GetComponent<TMP_Text>();
        InvokeRepeating("MoveToTargetValue",0,_moveToTargetTiming);
        InvokeRepeating("ChangeTarget",0,_targetChangeInterval);
    }

   
    private void ChangeTarget()
    {
        _lerpValue = 0;
        _targetValue = Random.Range(_minimumValue, _maximumValue);
    }
    private void MoveToTargetValue()
    {
        //We want the reading to move in 1 unit increments,and to be able to go up as well as down 
        _lerpValue = Mathf.Abs(1f/(_currentValue-_targetValue));
        _currentValue = (int)Mathf.Lerp(_currentValue, _targetValue, _lerpValue );
        _myText.text = _currentValue.ToString();
        if (_audioEnabled)
        {
            if (_currentValue > _alarmThreshold && _currentValue < _warningThreshold)
            {
                _alarmAudioSource.clip = _warningClip;
                if (!_alarmAudioSource.isPlaying)
                    _alarmAudioSource.Play();

            }
            else if (_currentValue < _alarmThreshold)
            {
                _alarmAudioSource.clip = _alarmClip;
                if (!_alarmAudioSource.isPlaying)
                    _alarmAudioSource.Play();

            }
            else
            {
                if (_alarmAudioSource.isPlaying)
                    _alarmAudioSource.Stop();
            }
        }

    }
}
