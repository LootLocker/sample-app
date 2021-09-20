using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ListPopulator : MonoBehaviour
{
    [SerializeField]
    Transform parent;
    [SerializeField]
    GameObject prefab;
    public OnListChangeOccured onListPopulated = new OnListChangeOccured();
    List<IListData> listData = new List<IListData>();

    public void ClearParent()
    {
        foreach (Transform tr in parent)
            Destroy(tr.gameObject);
    }

    public void Populate(IEnumerable<IListData> listData)
    {
        this.listData = listData.ToList();
        ClearParent();
        int i = 0;
        foreach (var data in listData)
        {
            data.listParent = this;
            data.index = i;
            ListPrefab lisPrefab = Instantiate(prefab, parent)?.GetComponent<ListPrefab>();
            ///Parent list should be able to keep track of items left in the list
            lisPrefab?.onListItemDestroyed?.AddListener(OnListItemDestroyed);
            lisPrefab?.Init(data);
            i++;
        }

        onListPopulated?.Invoke(this.listData.Count);
    }

    /// <summary>
    /// Prefabs will call this when they are destroyed and the item can be removed from the master list
    /// </summary>
    /// <param name="index"></param>
    public void OnListItemDestroyed(int index)
    {
        if (listData.Count > index) 
        listData.RemoveAt(index);
    }
}
