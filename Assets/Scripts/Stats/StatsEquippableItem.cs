using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Inventory/EquippableItem")]
public class StatsEquippableItem : EquipableItem , IModifierProvider
{
    [SerializeField]
    private Modifiers[] additiveModifiers;
    [SerializeField]
    private Modifiers[] percentageModifiers;
    
    [System.Serializable]
    struct Modifiers
    {
        public Stat stat;
        public float value;
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
        foreach (var modifier in additiveModifiers)
        {
            if (stat == modifier.stat)
            {
                yield return modifier.value;
            }
        }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat)
    {
        foreach (var modifier in percentageModifiers)
        {
            if (stat == modifier.stat)
            {
                yield return modifier.value;
            }
        }
    }
}
