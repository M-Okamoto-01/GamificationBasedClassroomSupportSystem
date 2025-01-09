using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishMaster : MonoBehaviour
{
    private TimelineMaster timelineMaster;

    // Start
    void Start()
    {
        timelineMaster = GameObject.FindGameObjectWithTag("TimelineMaster").GetComponent<TimelineMaster>();
    }

    public void OnClickCancel()
    {
        timelineMaster.PublishCloseWindow();
    }

    public void OnClickPublish()
    {
        StartCoroutine(timelineMaster.PublishComment());
    }
}
