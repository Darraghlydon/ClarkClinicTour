using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckifPlayerisLooking : MonoBehaviour
{

    private Transform _playerTransform;
    private Transform _myTransform;
    public TMP_Text infoText;
    public string textFieldData;



    private void OnEnable()
    {
        infoText.enabled = false;
    }

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(_playerTransform.position - _myTransform.position) < 20)
        {
            Vector3 playerDirection = _myTransform.position - _playerTransform.position;
            if (Vector3.Dot(_playerTransform.transform.forward, playerDirection) > 0.98f)
            {
                infoText.text = textFieldData;
                infoText.enabled = true;
            }
            else
            {
                infoText.enabled = false;
            }
        }

    }
}
