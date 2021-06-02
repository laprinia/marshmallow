using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest Manager/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] private bool m_inProgress = false;
    [SerializeField] private bool m_isCompleted = false;
    [SerializeField] private bool m_isInRange = false;

    [SerializeField] private GameObject m_questGiver;

    [SerializeField] private int m_currentPhase = 0;
    [SerializeField] private QuestPhase[] questPhases;

    // Getters
    public bool GetIsInProgress()
    {
        return m_inProgress;
    }
    public bool GetIsCompleted()
    {
        return m_isCompleted;
    }
    public bool GetIsInRange()
    {
        return m_isInRange;
    }
    public int GetCurrentPhaseNumber()
    {
        return m_currentPhase;
    }
    public QuestPhase GetCurrentPhase()
    {
        return questPhases[m_currentPhase];
    }
    public Vector3 GetQuestGiverPosition()
    {
        return m_questGiver.transform.position;
    }


    // Setters
    public void StartProgress()
    {
        m_inProgress = true;
        questPhases[m_currentPhase].SetProgress(true);
    }
    public void SetQuestCompleted()
    {
        m_isCompleted = true;
    }
    public void SetToNextPhase()
    {
        int nextPhase = m_currentPhase + 1;
        if(nextPhase >= questPhases.Length)
        {
            SetQuestCompleted();
            questPhases[m_currentPhase].SetProgress(false);
            questPhases[m_currentPhase].SetPhaseCompleted();
        }
        else
        {
            questPhases[m_currentPhase].SetProgress(false);
            questPhases[m_currentPhase].SetPhaseCompleted();
            m_currentPhase++;
            questPhases[m_currentPhase].SetProgress(true);
        }
    }
    public void SetInRange(bool isInRange)
    {
        m_isInRange = isInRange;
    }
    public void InitializeQuest()
    {
        m_currentPhase = 0;
        m_inProgress = false;
        m_isCompleted = false;
        m_isInRange = false;
        
        foreach(var phase in questPhases)
        {
            phase.Reset();
        }
    }
}



[System.Serializable]
public class QuestPhase
{
    [SerializeField] private bool m_inProgress = false;
    [SerializeField] private bool m_isCompleted = false;
    [SerializeField] private string m_description;
    [SerializeField] private GameObject m_destination;

    // Getters
    public bool GetIsInProgress()
    {
        return m_inProgress;
    }
    public bool GetIsCompleted()
    {
        return m_isCompleted;
    }
    public Vector3 GetDestinationPosition()
    {
        return m_destination.transform.position;
    }
    public string GetDescription()
    {
        return m_description;
    }

    // Setters
    public void SetProgress(bool isInProgress)
    {
        m_inProgress = isInProgress;
    }
    public void SetPhaseCompleted()
    {
        m_isCompleted = true;
    }
    public void Reset()
    {
        m_inProgress = false;
        m_isCompleted = false;
    }
}