using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableCharacter : MonoBehaviour
{   
    public void SaveCharacter()
    {
        SaveSystem.SavePlayer(this.gameObject);
    }

    public void LoadCharacter(PlayerData data)
    {
        transform.position = new Vector2(data.position[0], data.position[1]);
    }
}