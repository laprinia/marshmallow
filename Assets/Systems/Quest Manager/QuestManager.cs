using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> quests;
    [SerializeField] [ReadOnly] private float questGiverRange = 1.5f;
    [SerializeField] [ReadOnly] private CharacterController2D characterController;

    void Start()
    {
        foreach (var quest in quests)
        {
            quest.InitializeQuest();
        }
    }

    public void StartQuest()
    {
        foreach (var quest in quests)
        {
            if (!quest.GetIsCompleted())
            {
                var questGiverPosition = quest.GetQuestGiverPosition();

                // Check if player is in quest range
                if ((characterController.GetPosition().x > questGiverPosition.x - questGiverRange)
                    && (characterController.GetPosition().x < questGiverPosition.x + questGiverRange))
                {
                    quest.SetInRange(true);
                    if(!quest.GetIsInProgress())
                    {
                        quest.StartProgress();
                    }
                }
            }
        }
    }

    void Update()
    {
        foreach (var quest in quests)
        {
            if (!quest.GetIsCompleted())
            {
                var questGiverPosition = quest.GetQuestGiverPosition();

                // Check if player is in quest range
                if ((characterController.GetPosition().x > questGiverPosition.x - questGiverRange)
                    && (characterController.GetPosition().x < questGiverPosition.x + questGiverRange))
                {
                   quest.SetInRange(true);
                }
                else
                {
                    quest.SetInRange(false);
                }

                if (quest.GetIsInProgress())
                {
                    var questPhase = quest.GetCurrentPhase();
                    var destinationPosition = questPhase.GetDestinationPosition();

                    if ((characterController.GetPosition().x > destinationPosition.x - questGiverRange)
                    && (characterController.GetPosition().x < destinationPosition.x + questGiverRange))
                    {
                        quest.SetToNextPhase();
                    }
                }
            }
        }
    }
}