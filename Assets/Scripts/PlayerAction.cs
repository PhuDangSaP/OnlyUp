using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Action",menuName ="Action Menu/Create New Action")]
public class PlayerAction : ScriptableObject 
{
    [Header("Obstacle Height")]
    [SerializeField] string animationName;
    [SerializeField] float minimumHeight;
    [SerializeField] float maximumHeight;

    
    public bool CheckIfAvailable(ObstacleChecker.ObstacleInfo hitData,Transform player)
    {
        float checkHeight= hitData.heightInfo.point.y-player.position.y;
        if (checkHeight<minimumHeight||checkHeight>maximumHeight)
        {
            return false;
        }
        
        return true;
    }
    public string AnimationName => animationName;
  
}
