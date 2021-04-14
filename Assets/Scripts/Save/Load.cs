using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    private void Awake()
    {
        if (MenuButton.isContinue)
        {
            LoadCharacter();
        }
        else
        {
            GameObject.Find("Marshmallow").GetComponent<SaveableCharacter>().SaveCharacter();
        }
    }
    
    public void LoadCharacter()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        switch (data.dogName)
        {
            case "Campanelle":
                GameObject.Find("Campanelle").GetComponent<SaveableCharacter>().LoadCharacter(data);
                break;
            case "Dumpling":
                GameObject.Find("Dumpling").GetComponent<SaveableCharacter>().LoadCharacter(data);
                break;
            default:
                GameObject.Find("Marshmallow").GetComponent<SaveableCharacter>().LoadCharacter(data);
                break;
        }
    }
}
