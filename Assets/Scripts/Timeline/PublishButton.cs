using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublishButton : MonoBehaviour
{
    public GameObject TimelineMasterObj;
    public TimelineMaster timlinemaster;

    // Start
    void Start()
    {
        timlinemaster = TimelineMasterObj.GetComponent<TimelineMaster>();
        this.GetComponent<Button>().onClick.AddListener(OnClick);
        //Webかそうでないかで表示を変える
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) {
            //Webかそうでないかで表示を変える
            this.GetComponent<RectTransform>().SetWidth(50);
            this.GetComponent<RectTransform>().SetHeight(50);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 50);
        }
    }

    /// <summary>
    /// Open the publish window
    /// </summary>
    private void OnClick()
    {
        timlinemaster.PublishOpenWindow();
    }


}
