using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField]
    Quest quest;


    private void Start()
    {
        foreach (var quest in quest.GetQuests())
        {
            Debug.Log(quest);
        }
    }
}
