using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public string dogName;
    public float[] position;

    public PlayerData(GameObject gameObject)
    {
        dogName = gameObject.name;
        position = new float[2];
        position[0] = gameObject.transform.position.x;
        position[1] = gameObject.transform.position.y;
    }
}