//responsible for updating every lod's position and rotation (and active state)
using UnityEngine;

public class Transform_LODProperty : MonoBehaviour, ILodable
{
    private int prevGroup = 0;

    public void OnLODUpdate(int currentGroup, Transform[] groups)
    {
        int index = 0;
        foreach (Transform child in groups[currentGroup])
        {
            child.position = groups[prevGroup].GetChild(index).position;
            child.rotation = groups[prevGroup].GetChild(index).rotation;
            child.gameObject.SetActive(groups[prevGroup].GetChild(index).gameObject.activeSelf);
            index++;
        }
        prevGroup = currentGroup;
    }
}
