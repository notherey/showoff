//property which handles transform sync between LOD's
using UnityEngine;

namespace Car
{
    public enum PartName
    {
        Chassis,
        DoorFL,
        DoorFR,
        DoorRL,
        DoorRR,
        GlassDoorFL,
        GlassDoorFR,
        GlassDoorRL,
        GlassDoorRR,
        GlassFront,
        GlassRear,
        Hood,
        LightFL,
        LightFR,
        LightRL,
        LightRR,
        SteeringWheel,
        Trunk,
        WheelFL,
        WheelFR,
        WheelRL,
        WheelRR
    }
    public class Parts_LODProperty : MonoBehaviour, ILodable
    {
        public Transform[] Part { get; private set; }

        public void OnLODUpdate(int currentGroup, Transform[] groups)
        {
            if (Part == null)
            {
                Part = new Transform[groups[0].childCount];
            }

            for (int i = 0; i < Part.Length; i++)
            {
                Part[i] = groups[currentGroup].GetChild(i);
            }
        }
    }
}
