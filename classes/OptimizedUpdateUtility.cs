/*
 * Adds a delayed update, use in any script that doesn't need to update everything at all times
 * looks for an IOptUpdate interface in every monoBehabiour script
 * ***
 * Usage:
 * Implement IOptUpdate in every script you want to have this functionality
 * Put your code in OptimizedUpdate(){}
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class OptimizedUpdateUtility : MonoBehaviour
{
    //interval for each update
    private float updateTime = 0.2f;

    private float localUpdateTime;
    private List<IOptUpdate> updateables;

    private void Start()
    {
        updateables = FindInterfaces.Find<IOptUpdate>();
    }
    private void Update()
    {
        if (Time.timeSinceLevelLoad - localUpdateTime > updateTime)
        {
            localUpdateTime = Time.timeSinceLevelLoad;
            if (updateables != null)
            {
                foreach (IOptUpdate update in updateables)
                {
                    update.OptimizedUpdate();
                }
            }
        }
    }
}