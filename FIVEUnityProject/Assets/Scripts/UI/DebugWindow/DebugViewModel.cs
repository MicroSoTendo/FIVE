using System.Collections;
using TMPro;
using UnityEngine;

namespace FIVE.UI.DebugWindow
{
    public class DebugViewModel : ViewModel
    {
        protected override string PrefabPath => "EntityPrefabs/UI/DebugWindow";

        public TMP_Text TmpText { get; }

        public DebugViewModel()
        {
            TmpText = Get<TMP_Text>("Text");
        }

        public IEnumerator Routine()
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}
