using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.BlackSmith
{
    public class ComponentsRead : MonoBehaviour
    {
        private TextAsset csvFile;
        private string[] records;
        private string[] currentTerm;
        void Start()
        {
            csvFile = Resources.Load<TextAsset>("CSV/Composite");
            records = csvFile.text.Split('\n');
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < records.Length; i++)
            {
                string[] fields = records[i].Split(',');
                
            }
        }
    }
}
