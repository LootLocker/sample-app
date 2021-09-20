using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListPrefab : MonoBehaviour
{
    public OnListChangeOccured onListItemDestroyed = new OnListChangeOccured();
    IListCallback listCallback;

    public virtual void Init(IListData listData)
    {
        listCallback = GetComponent<IListCallback>();
        ///So the lists prefabs can know when the parent list has finished populating. They might want to do something after this
        listData.listParent.onListPopulated.AddListener(OnListPopulated);
        listCallback?.OnInit(listData);
    }

    public virtual void OnListPopulated(int listCount)
    {
        listCallback?.OnListPopulated(listCount);
    }

    public void OnDestroy()
    {
        onListItemDestroyed?.Invoke(transform.GetSiblingIndex());
    }

}
