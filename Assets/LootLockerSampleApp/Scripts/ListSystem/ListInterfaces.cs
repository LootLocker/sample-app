
using UnityEngine.Events;

public class OnListChangeOccured : UnityEvent<int> { }

public interface IListData
{
    int index { get; set; }
    ListPopulator listParent { get; set; }
}

public interface IListCallback
{
    void OnInit(IListData listData);
    void OnListPopulated(int listCount);
}

