using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.BlackSmith
{
    public class ComponentsRead
    {
        private TextAsset csvFile;
        private string[] records;
        private string[] currentTerm;
        private List<List<string>> CompositeLists;
        public static List<string> PathLists;
        private void GenerateLists()
        {
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
        public bool isCompositeComponents(List<GameObject> compositeItems)
        {
            GenerateLists();
            SortedSet<string> sortedCompositeItems = new SortedSet<string>();
            List<string> sortedCompositeItemList = new List<string>(sortedCompositeItems);
            for(int i = 0; i < compositeItems.Count; i++)
            {
                sortedCompositeItems.Add(compositeItems[i].name);
            }
            
            for (int i = 0; i < CompositeLists.Count; i++)
            {
                SortedSet<string> set = new SortedSet<string>(CompositeLists[i]);
                List<string> sorteSet = new List<string>(set);
                if (!sorteSet[i].Contains(sortedCompositeItemList[i]))
                {
                    return false;
                }
            }
                
            return true;
        }
    }
}
