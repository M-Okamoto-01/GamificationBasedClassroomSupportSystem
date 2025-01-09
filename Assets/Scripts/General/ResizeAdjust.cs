using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ResizeAdjust
///
/// キャンバスのサイズが変更された際に発火
/// </summary>

public class ResizeAdjust : MonoBehaviour
{
    public GameObject TimelineMainObject;

    private void OnRectTransformDimensionsChange()
    {
        // The RectTransform has changed!
        //TimelineMainObject.GetComponent<TimelineMaster>().SetAreaSize();
    }

}
