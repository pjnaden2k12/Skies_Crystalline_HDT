using UnityEngine;

public class CameraController : MonoBehaviour
{
    public LevelData levelData;
 
    void Start()
    {
        if (levelData != null && levelData.mapSize != null)
        {
            float centerX = levelData.mapSize.x / 2f;
          

           
            Vector3 newPosition = new Vector3(centerX, 10f, -7f);
            transform.position = newPosition;

           
        }
        else
        {
            Debug.LogWarning("LevelData hoặc mapSize chưa được gán.");
        }
    }
}
