//responsible for updating lod script properties
using UnityEngine;

public class CarLODUpdater : MonoBehaviour, IOptUpdate
{
    [SerializeField] private bool enable = true;
    [Space]
    [SerializeField] private Object[] properties;
    [Header("LOD Objects for checking")]
    [SerializeField] private GameObject[] checkedObjects;

    [Header("LODGroups")]
    [SerializeField] private Transform[] LODGroups;

    private bool[] prevVisibilities;
    private int currentGroup = 0;

    private void Start()
    {
        try
        {
            prevVisibilities = new bool[checkedObjects.Length];
            UpdateStats();
        }
        catch
        {
            Debug.LogError("LODUpdater ~ No assigned LOD checker objects", this);
        }
    }

    public void OptimizedUpdate()
    {
        if (!enable) return;

        for (int i = 0; i < checkedObjects.Length; i++)
        {
            bool isVisible = checkedObjects[i].GetComponent<Renderer>().isVisible;
            if (!prevVisibilities[i] && isVisible)
            {
                currentGroup = i;
                UpdateLods();
                UpdateStats();
                return;
            }
            prevVisibilities[i] = isVisible;
        }
    }//checks for any change in chassis' renderers (that's when the LOD system changes it's state)

    private void UpdateStats()
    {
        for (int y = 0; y < checkedObjects.Length; y++)
        {
            bool isVisible = checkedObjects[y].GetComponent<Renderer>().isVisible;
            prevVisibilities[y] = isVisible;
        }
    }//set previous renderers' values to actual ones (reset them)

    private void UpdateLods()
    {
        try
        {
            foreach (ILodable lod in properties)
            {
                lod.OnLODUpdate(currentGroup, LODGroups);
            }
        }
        catch
        {
            Debug.LogError("LODUpdater ~ Missing interface in property (perhaps you assigned a wrong script?)", this);
        }
    }//update every property
}
