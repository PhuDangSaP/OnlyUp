using UnityEngine;

public static class ObstacleChecker
{
    private static float heightRayLength =6f;
    public static ObstacleInfo CheckObstacle(Vector3 rayStart, Transform playerTransform, float rayLength, LayerMask obstacleLayer)
    {
        rayStart += new Vector3(0, 0.1f, 0);
        ObstacleInfo hitdata = new ObstacleInfo();
        hitdata.hitFound = Physics.Raycast(rayStart, playerTransform.forward * rayLength, out hitdata.hitInfo, rayLength, obstacleLayer);
       
        if(hitdata.hitFound ) 
        {
            hitdata.heightHitFound = Physics.Raycast(hitdata.hitInfo.point + Vector3.up * heightRayLength, Vector3.down,out hitdata.heightInfo,heightRayLength,obstacleLayer);
        }
        return hitdata;
    }
   

    public struct ObstacleInfo
    {
        public bool hitFound;
        public bool heightHitFound;
        public RaycastHit hitInfo;
        public RaycastHit heightInfo;
    }
}
