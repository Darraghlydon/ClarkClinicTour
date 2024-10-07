using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckifPlayerisLooking : MonoBehaviour
{

    public Transform playerTransform;
    public TMP_Text infoText;
    public string textFieldData;



    private void OnEnable()
    {
        infoText.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(playerTransform.position - transform.position) < 20)
        {
            Vector3 playerDirection = transform.position - playerTransform.position;
            if (Vector3.Dot(playerTransform.transform.forward, playerDirection) > 0.98f)
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
