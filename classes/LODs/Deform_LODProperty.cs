using UnityEngine;

public class Deform_LODProperty : MonoBehaviour, ILodable
{
    public void OnLODUpdate(int currentGroup, Transform[] groups)
    {
        Debug.LogWarning("DeformLODProp ~ not implemented");
    }
}
