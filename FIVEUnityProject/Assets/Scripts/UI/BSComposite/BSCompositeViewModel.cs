using FIVE.Interactive.BlackSmith;
using FIVE.UI.BlackSmith;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.BSComposite
{
    ///*
    public class BSCompositeViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Shop/BSComposite";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public Button Button1 { get; }
        public Button Button2 { get; }
        public Button Button3 { get; }
        public Button Button4 { get; }
        public Button Button5 { get; }
        public Button Button6 { get; }
        public Button Button7 { get; }
        public Button Button8 { get; }
        public Button Button9 { get; }
        public Button Button10 { get; }
        public Button Button11 { get; }
        public Button Button12 { get; }

        public BSCompositeViewModel() : base()
        {
            Button1 = Get<Button>(nameof(Button1));
            Bind(Button1).To(() =>OnButtonClickedComp(1));

            Button2 = Get<Button>(nameof(Button2));
            Bind(Button2).To(() => OnButtonClickedComp(2));

            Button3 = Get<Button>(nameof(Button3));
            Bind(Button3).To(() => OnButtonClickedComp(3));

            Button4 = Get<Button>(nameof(Button4));
            Bind(Button4).To(() => OnButtonClickedInv(4));

            Button5 = Get<Button>(nameof(Button5));
            Bind(Button5).To(() => OnButtonClickedInv(5));

            Button6 = Get<Button>(nameof(Button6));
            Bind(Button6).To(() => OnButtonClickedInv(6));

            Button7 = Get<Button>(nameof(Button7));
            Bind(Button7).To(() => OnButtonClickedInv(7));

            Button8 = Get<Button>(nameof(Button8));
            Bind(Button8).To(() => OnButtonClickedInv(8));

            Button9 = Get<Button>(nameof(Button9));
            Bind(Button9).To(() => OnButtonClickedInv(9));

            Button10 = Get<Button>(nameof(Button10));
            Bind(Button10).To(() => OnButtonClickedInv(10));

            Button11 = Get<Button>(nameof(Button11));
            Bind(Button11).To(() => OnButtonClickedInv(11));

            Button12 = Get<Button>(nameof(Button12));
            Bind(Button12).To(() => OnButtonClickedInv(12));


        }

        private void OnButtonClickedInv(int index)
        {
            InventorySlotHolder a = new InventorySlotHolder();
            a.SetParentInvToComp(index);
        }
        private void OnButtonClickedComp(int index)
        {
            //BlackSmithGenerate.SetToInv();
        }
    } //*/
}