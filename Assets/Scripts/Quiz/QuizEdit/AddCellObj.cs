using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;

public class AddCellObj : MonoBehaviour
{
    public GameObject CellEditContents;
    public GameObject CellEditPrefab;

    public void OnClickAddCell()
    {
        GameObject cellEditObj = Instantiate(CellEditPrefab);
        //下から2番目に追加
        cellEditObj.transform.SetParent(CellEditContents.transform, false);
        //自分を一番下に移動
        gameObject.transform.SetAsLastSibling();
    }
}
