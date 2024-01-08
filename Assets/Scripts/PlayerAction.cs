using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Action Menu/Create New Action")]
public class PlayerAction : ScriptableObject
{
    private float heightRayLength = 10f;
    [Header("Obstacle")]
    [SerializeField] string animationName;
    [SerializeField] float minimumHeight;
    [SerializeField] float maximumHeight;
    [SerializeField] Vector3 offSet;
    [SerializeField] bool canInAir;

    [Header("Target Matching")]
    [SerializeField] bool allowTargetMatching = true;
    [SerializeField] AvatarTarget compareBodyPart;
    [SerializeField] float compareStartTime;
    [SerializeField] float compareEndTime;
    public Vector3 ComparePosition { get; set; }
    public bool CheckIfAvailable(Vector3 rayStart, Transform playerTransform, float rayLength, LayerMask obstacleLayer, bool inAir = false)
    {
        rayStart += offSet;
        ObstacleInfo hitData = new ObstacleInfo();
        hitData.hitFound = Physics.Raycast(rayStart, playerTransform.forward, out hitData.hitInfo, rayLength, obstacleLayer);

        if (!hitData.hitFound)        
            return false;

        hitData.heightHitFound = Physics.Raycast(hitData.hitInfo.point + Vector3.up * heightRayLength, Vector3.down, out hitData.heightInfo, heightRayLength, obstacleLayer);
        float checkHeight = hitData.heightInfo.point.y - playerTransform.position.y;
       
        if (checkHeight < minimumHeight || checkHeight > maximumHeight)       
            return false;
        
        if (inAir == true && canInAir == false)        
            return false;
        
        
        ComparePosition = hitData.heightInfo.point;

        return true;
    }



    public string AnimationName => animationName;

    public bool AllowTargetMatching => allowTargetMatching;
    public AvatarTarget CompareBodyPart => compareBodyPart;
    public float CompareStartTime => compareStartTime;
    public float CompareEndTime => compareEndTime;
    public struct ObstacleInfo
    {
        public bool hitFound;
        public bool heightHitFound;
        public RaycastHit hitInfo;
        public RaycastHit heightInfo;
    }
}
