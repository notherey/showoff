using UnityEngine;

public interface ILodable
{
    void OnLODUpdate(int currentGroup, Transform[] groups); //called on LOD group change
}
