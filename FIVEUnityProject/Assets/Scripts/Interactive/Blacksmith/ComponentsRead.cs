using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.Blacksmith
{
    public class ComponentsRead
    {
        private static TextAsset csvFile;
        private static string[] records;
        private static string[] currentTerm;
        private static List<List<string>> CompositeLists;
        public static List<string> PathLists;
        public static int index;
        private static void GenerateLists()
        {
            index = 0;
            csvFile = Resources.Load<TextAsset>("CSV/Composite");
            records = csvFile.text.Split('\n');
            CompositeLists = new List<List<string>>(records.Length);
            PathLists = new List<string>();
            for (int i = 0; i < records.Length; i++)
            {
                string[] fields = records[i].Split(',');
                List<string> temp = new List<string> { fields[0], fields[1], fields[2] };
                CompositeLists.Add(temp);
                PathLists.Add(fields[3]);

            }
        }
        private static bool isCompositeComponents(List<GameObject> compositeItems)
        {
            bool isComp = true;
            GenerateLists();
            SortedSet<string> sortedCompositeItems = new SortedSet<string>();
            foreach (GameObject a in compositeItems)
            {
                sortedCompositeItems.Add(a.name);
            }
            List<string> sortedCompositeItemList = new List<string>(sortedCompositeItems);

            for (int i = 0; i < CompositeLists.Count; i++)
            {
                SortedSet<string> set = new SortedSet<string>(CompositeLists[i]);
                List<string> sorteSet = new List<string>(set);
                isComp = true;
                for(int a = 0; a < sorteSet.Count; a++)
                {
                    if (!sortedCompositeItemList[a].Contains(sorteSet[a]))
                    {
                        isComp = false;
                    }
                }
                if(isComp == true)
                {
                    index = i;
                    break;
                }
                
            }
                
            return isComp;
        }
        public static GameObject generateItem(List<GameObject> compositeItems)
        {
            if (isCompositeComponents(compositeItems))
            {
                return Resources.Load<GameObject>(PathLists[index]);
            }
            
            return null;
        }
    }
}
