using FIVE.EventSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI.SplashScreens
{
    public class StartUpScreen : LoadingSplashScreen
    {
        public StartUpScreen() : base()
        {
            var canvasGameObject = new GameObject {name = nameof(StartUpScreen)};
            canvas = canvasGameObject.AddComponent<Canvas>();
            canvasGameObject.AddComponent<CanvasScaler>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvasGameObject.transform.SetSiblingIndex(0);

            MainThreadDispatcher.Schedule(
                SetUpFromCanvas,
                InstantiateTSS,
                SetUpTSSPositionAndBar,
                InstantiateCharacters,
                InstantiateSymbols,
                SetUpInitialPositions,
                SetUpTargetPositions,
                SetUpSymbolPositions,
                SetUpEventTrigering,
                SetActive,
                StartMoving);
        }

        private void SetUpFromCanvas()
        {
            width = canvas.GetComponent<RectTransform>().sizeDelta.x;
            height = canvas.GetComponent<RectTransform>().sizeDelta.y;
            unitWidth = width / 4f;
            canvas.gameObject.AddComponent<Image>().sprite = Resources.Load<Sprite>("UI/WhiteBackground");
        }

        private async void StartMoving()
        {
            genmtt.StartMoving();
            await Task.Delay(1);
            laurencemtt.StartMoving();
            await Task.Delay(2);
            wenmtt.StartMoving();
            await Task.Delay(1);
            wallymtt.StartMoving();
        }

        private void SetActive()
        {
            Gen.SetActive(true);
            Laurence.SetActive(true);
            Wen.SetActive(true);
            Wally.SetActive(true);
            Yu.SetActive(true);
            QuestionMark.SetActive(true);
            ExclamationMark.SetActive(true);
            LaurenceMark.SetActive(true);
            TBoundary.SetActive(true);
            TColor.SetActive(true);
            S1Boundary.SetActive(true);
            S1Color.SetActive(true);
            S2Boundary.SetActive(true);
            S2Color.SetActive(true);
        }

        private void Destroy()
        {
            float time = 3.5f;
            MainThreadDispatcher.Destroy(Wally, time);
            MainThreadDispatcher.Destroy(Gen, time);
            MainThreadDispatcher.Destroy(Laurence, time);
            MainThreadDispatcher.Destroy(Yu, time);
            MainThreadDispatcher.Destroy(Wen, time);
            MainThreadDispatcher.Destroy(QuestionMark, time);
            MainThreadDispatcher.Destroy(ExclamationMark, time);
            MainThreadDispatcher.Destroy(LaurenceMark, time);
            MainThreadDispatcher.Destroy(TBoundary, time);
            MainThreadDispatcher.Destroy(TColor, time);
            MainThreadDispatcher.Destroy(S1Boundary, time);
            MainThreadDispatcher.Destroy(S1Color, time);
            MainThreadDispatcher.Destroy(S2Boundary, time);
            MainThreadDispatcher.Destroy(S2Color, time);
            MainThreadDispatcher.Destroy(canvas.gameObject, time);
        }

        private void DoFadingOut()
        {
            float duration = 1.5f;
            Wally.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            Gen.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            Laurence.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            Yu.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            Wen.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            QuestionMark.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            ExclamationMark.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            LaurenceMark.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            TBoundary.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            TColor.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            S1Boundary.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            S1Color.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            S2Boundary.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            S2Color.GetComponent<Image>().CrossFadeAlpha(0, duration, false);
            Destroy();
        }


        private float GetTargetY(GameObject go, float canvasHeight)
        {
            return -canvasHeight / 2f + go.GetComponent<RectTransform>().sizeDelta.y / 2f;
        }

        private MoveInAnimation SetMoveToTarget(GameObject go, Vector3 target, float speed = 1f)
        {
            MoveInAnimation mtt = go.GetComponent<MoveInAnimation>();
            mtt.SetTargetAndSpeed(target, speed);
            return mtt;
        }

        private GameObject InitLogoHelper(string name, Transform parent, float width)
        {
            var obj = Object.Instantiate(Resources.Load<GameObject>("Logos/Prefabs/" + name), parent);
            obj.SetActive(false);
            RectTransform recttransform = obj.GetComponent<RectTransform>();
            recttransform.sizeDelta *= width / recttransform.sizeDelta.x;
            return obj;
        }

        protected override void UpdateLoadingProgressBar()
        {
            if (TColor == null)
            {
                return;
            }

            float tAmount = 0f;
            float s1Amount = 0f;
            float s2Amount = 0f;
            if (Progress < 1 / 3f)
            {
                tAmount = Progress * 3f;
            }
            else if (Progress < 2 / 3f)
            {
                tAmount = 1f;
                s1Amount = Progress * 3f - 1f;
            }
            else
            {
                tAmount = 1f;
                s1Amount = 1f;
                s2Amount = Progress * 3f - 2f;
            }

            TColor.GetComponent<Image>().fillAmount = tAmount;
            S1Color.GetComponent<Image>().fillAmount = s1Amount;
            S2Color.GetComponent<Image>().fillAmount = s2Amount;
            if (Math.Abs(Progress - 1) < float.Epsilon)
                DoFadingOut();
        }

        #region Private Fields

        private GameObject Wally, Gen, Yu, Laurence, Wen;

        private Vector3 genTarget, wallyTarget, laurenceTarget, wenTarget, yuTarget;
        private GameObject QuestionMark, ExclamationMark, LaurenceMark;

        private GameObject TBoundary, TColor, S1Boundary, S1Color, S2Boundary, S2Color;

        private readonly Canvas canvas;
        private float width, height, unitWidth;
        private MoveInAnimation genmtt, wallymtt, laurencemtt, wenmtt, yumtt;

        #endregion

        #region Animation Methods

        private void InstantiateCharacters()
        {
            Wally = InitLogoHelper(nameof(Wally), canvas.transform, unitWidth);
            Gen = InitLogoHelper(nameof(Gen), canvas.transform, unitWidth);
            Yu = InitLogoHelper(nameof(Yu), canvas.transform, unitWidth);
            Wen = InitLogoHelper(nameof(Wen), canvas.transform, unitWidth);
            Laurence = InitLogoHelper(nameof(Laurence), canvas.transform, unitWidth);
            Yu.GetComponent<RectTransform>().sizeDelta *= 0.82f;
        }

        private void SetUpInitialPositions()
        {
            //Moving horizontally Y = final
            Wally.transform.localPosition =
                new Vector3(width / 2f + unitWidth / 2f, GetTargetY(Wally, height), 0);
            Gen.transform.localPosition =
                new Vector3(-width / 2f - unitWidth, GetTargetY(Gen, height), 0);

            //Moving vertically X = final
            Laurence.transform.localPosition =
                new Vector3(0.1f * width, -height / 2f - unitWidth / 1.5f, 0);
            Wen.transform.localPosition =
                new Vector3(-0.125f * width, -height / 2f - unitWidth / 2f, 0);
            Yu.transform.localPosition =
                new Vector3(-0.125f * width, height / 2f + unitWidth / 2f, 0);
        }

        private void InstantiateSymbols()
        {
            QuestionMark = InitLogoHelper(nameof(QuestionMark), canvas.transform, unitWidth / 6);
            ExclamationMark = InitLogoHelper(nameof(ExclamationMark), canvas.transform, unitWidth / 6);
            LaurenceMark = InitLogoHelper(nameof(LaurenceMark), canvas.transform, unitWidth / 6);
        }

        private void InstantiateTSS()
        {
            TBoundary = InitLogoHelper(nameof(TBoundary), canvas.transform, unitWidth / 2);
            TColor = InitLogoHelper(nameof(TColor), TBoundary.transform, unitWidth / 2);

            S1Boundary = InitLogoHelper(nameof(S1Boundary), canvas.transform, unitWidth / 2);
            S1Color = InitLogoHelper(nameof(S1Color), S1Boundary.transform, unitWidth / 2);

            S2Boundary = InitLogoHelper(nameof(S2Boundary), canvas.transform, unitWidth / 2);
            S2Color = InitLogoHelper(nameof(S2Color), S2Boundary.transform, unitWidth / 2);
        }

        private void SetUpTSSPositionAndBar()
        {
            TBoundary.transform.localPosition = new Vector3(-0.144f * width, 0.1798f * height, 0f);
            S1Boundary.transform.localPosition = new Vector3(0, 0.1996f * height, 0f);
            S2Boundary.transform.localPosition = new Vector3(0.1404f * width, 0.17983f * height, 0f);

            TColor.GetComponent<Image>().fillAmount = 0;
            S1Color.GetComponent<Image>().fillAmount = 0;
            S2Color.GetComponent<Image>().fillAmount = 0;
        }

        private void SetUpTargetPositions()
        {
            genTarget = new Vector3(0.16f * width - width / 2f, GetTargetY(Gen, height), 0);
            wallyTarget = new Vector3(0.35f * width, GetTargetY(Wally, height), 0f);
            laurenceTarget = new Vector3(0.1f * width, GetTargetY(Laurence, height), 0f);
            wenTarget = new Vector3(-0.125f * width, GetTargetY(Wen, height), 0f);
            yuTarget = new Vector3(-0.125f * width, GetTargetY(Yu, height), 0f) + new Vector3(0f,
                           Wen.GetComponent<RectTransform>().sizeDelta.y -
                           0.16f * Yu.GetComponent<RectTransform>().sizeDelta.y, 0f);
        }

        private void SetUpSymbolPositions()
        {
            ExclamationMark.transform.localPosition = wallyTarget * 0.9f +
                                                      new Vector3(
                                                          Wally.GetComponent<RectTransform>().sizeDelta.x / 2f * 1.1f,
                                                          Wally.GetComponent<RectTransform>().sizeDelta.y / 2f * 1.1f,
                                                          0);
            QuestionMark.transform.localPosition = yuTarget + new Vector3(
                                                       Yu.GetComponent<RectTransform>().sizeDelta.x / 2.8f,
                                                       Yu.GetComponent<RectTransform>().sizeDelta.y / 2.5f, 0);
            LaurenceMark.transform.localPosition = laurenceTarget * 0.9f +
                                                   new Vector3(
                                                       Laurence.GetComponent<RectTransform>().sizeDelta.x / 2.3f,
                                                       Laurence.GetComponent<RectTransform>().sizeDelta.y / 9.5f, 0);
        }


        private void SetUpEventTrigering()
        {
            float speed = width / 1.75f;

            genmtt = SetMoveToTarget(Gen, genTarget, speed);
            wallymtt = SetMoveToTarget(Wally, wallyTarget, speed);
            wallymtt.OnFinished = (sender, args) =>
            {
                ExclamationMark.GetComponent<ZoomShowAnimation>().SetUpSpeed(2.5f);
                ExclamationMark.GetComponent<ZoomShowAnimation>().StartZoomIn();
            };
            laurencemtt = SetMoveToTarget(Laurence, laurenceTarget, speed);
            laurencemtt.OnFinished = (sender, args) =>
            {
                LaurenceMark.GetComponent<ZoomShowAnimation>().SetUpSpeed(2f);
                LaurenceMark.GetComponent<ZoomShowAnimation>().StartZoomIn();
            };
            wenmtt = SetMoveToTarget(Wen, wenTarget, speed);
            yumtt = SetMoveToTarget(Yu, yuTarget, speed);
            yumtt.OnFinished = (sender, args) =>
            {
                QuestionMark.GetComponent<ZoomShowAnimation>().SetUpSpeed(1.5f);
                QuestionMark.GetComponent<ZoomShowAnimation>().StartZoomIn();
            };

            wenmtt.OnFinished = (o, e) => { yumtt.StartMoving(); };
        }

        #endregion
    }
}