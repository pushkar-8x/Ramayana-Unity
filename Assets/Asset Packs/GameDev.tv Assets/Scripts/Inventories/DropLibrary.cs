using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/DropLibrary")]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField]
        private DropConfig[] potentialDrops;
        [SerializeField] private float[] dropPercentage;
        [SerializeField] private int[] minDrops;
        [SerializeField] private int[] maxDrops;
        
        
        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                if(!item.IsStackable())
                {
                    return 1;
                }
                else
                {
                    int min = GetByLevel(minNumber,level);
                    int max= GetByLevel(maxNumber, level);
                    return Random.Range(min, max + 1);
                }
            }
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }
            if(level > values.Length)
            {
                return values[values.Length - 1];
            }
            if(level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if(!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        private Dropped GetRandomDrop(int level)
        {
            var drop = SelectRandomDrop(level);
            var result = new Dropped();
            result.item = drop.item;
            result.number = drop.GetRandomNumber(level);
            return result;
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int max = GetByLevel(maxDrops, level);
            int min = GetByLevel(minDrops, level);
            return Random.Range(min, max + 1);
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropPercentage, level);
        }

        DropConfig SelectRandomDrop(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;

            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance,level);
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        float GetTotalChance(int level)
        {
            float totalChance = 0;
            foreach (var drop in potentialDrops)
            {
                totalChance += GetByLevel(drop.relativeChance,level);
            }

            return totalChance;
        }
    }
}

