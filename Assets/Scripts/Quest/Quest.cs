using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] string[] quests;

    public IEnumerable<string> GetQuests()
    {
        return quests;
    }
}
