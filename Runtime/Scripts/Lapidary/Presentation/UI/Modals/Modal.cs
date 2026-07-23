using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Moonstone.Lapidary.Presentation.UI.Modals
{
    public class Modal : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject root = default;
        [SerializeField] CanvasGroup canvasGroup = default;
        [SerializeField] Button backdropButton = default;
        [SerializeField] Button closeButton = default;
        [SerializeField] Button confirmButton = default;
        [SerializeField] Button cancelButton = default;
        [SerializeField] TMP_Text titleText = default;
        [SerializeField] TMP_Text messageText = default;
        [SerializeField] TMP_Text confirmText = default;
        [SerializeField] TMP_Text cancelText = default;

        [Header("Legacy Text")]
        [SerializeField] Text legacyTitleText = default;
        [SerializeField] Text legacyMessageText = default;
        [SerializeField] Text legacyConfirmText = default;
        [SerializeField] Text legacyCancelText = default;

        [Header("Behavior")]
        [SerializeField] bool hideOnAwake = true;
        [SerializeField] bool closeOnBackdrop = true;
        [SerializeField] bool closeOnEscape = true;
        [SerializeField] bool disableInteractionDuringTransition = true;
        [SerializeField] bool useUnscaledTime = true;
        [SerializeField, Min(0f)] float fadeDuration = 0.15f;

        [Header("Events")]
        [SerializeField] UnityEvent shown = new();
        [SerializeField] ModalResultEvent hidden = new();

        Coroutine transition;
        bool isVisible;
        ModalResult lastResult = ModalResult.None;

        public event Action Shown;
        public event Action<ModalResult> Hidden;

        public bool IsVisible => isVisible;
        public ModalResult LastResult => lastResult;
        public UnityEvent ShownEvent => shown;
        public ModalResultEvent HiddenEvent => hidden;

        protected virtual void Awake()
        {
            ResolveReferences();
            BindButtons();

            if (hideOnAwake)
                SetHiddenImmediate(ModalResult.None, false);
            else
                SetShownImmediate(false);
        }

        protected virtual void Update()
        {
            if (isVisible && closeOnEscape && Input.GetKeyDown(KeyCode.Escape))
                Dismiss();
        }

        protected virtual void OnDestroy()
        {
            UnbindButtons();
        }

        public void Show()
        {
            Show(null, null);
        }

        public void Show(string title, string message)
        {
            SetTitle(title);
            SetMessage(message);
            ShowInternal(true);
        }

        public void ShowImmediate()
        {
            ResolveReferences();
            StopTransition();
            SetShownImmediate(true);
        }

        public void Hide()
        {
            Hide(ModalResult.Dismissed);
        }

        public void Confirm()
        {
            Hide(ModalResult.Confirmed);
        }

        public void Cancel()
        {
            Hide(ModalResult.Cancelled);
        }

        public void Dismiss()
        {
            Hide(ModalResult.Dismissed);
        }

        public void Hide(ModalResult result)
        {
            HideInternal(result, true);
        }

        public void HideImmediate()
        {
            HideImmediate(ModalResult.Dismissed);
        }

        public void HideImmediate(ModalResult result)
        {
            ResolveReferences();
            StopTransition();
            SetHiddenImmediate(result, true);
        }

        public void SetTitle(string title)
        {
            SetText(titleText, legacyTitleText, title);
        }

        public void SetMessage(string message)
        {
            SetText(messageText, legacyMessageText, message);
        }

        public void SetConfirmText(string text)
        {
            SetText(confirmText, legacyConfirmText, text);
        }

        public void SetCancelText(string text)
        {
            SetText(cancelText, legacyCancelText, text);
        }

        static void SetText(TMP_Text tmpText, Text legacyText, string text)
        {
            if (text == null)
                return;

            if (tmpText != null)
                tmpText.text = text;

            if (legacyText != null)
                legacyText.text = text;
        }

        void ResolveReferences()
        {
            if (root == null)
                root = gameObject;

            if (canvasGroup == null)
                canvasGroup = root.GetComponent<CanvasGroup>();
        }

        void BindButtons()
        {
            if (backdropButton != null)
                backdropButton.onClick.AddListener(OnBackdropClicked);

            if (closeButton != null)
                closeButton.onClick.AddListener(Dismiss);

            if (confirmButton != null)
                confirmButton.onClick.AddListener(Confirm);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(Cancel);
        }

        void UnbindButtons()
        {
            if (backdropButton != null)
                backdropButton.onClick.RemoveListener(OnBackdropClicked);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(Dismiss);

            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(Confirm);

            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(Cancel);
        }

        void OnBackdropClicked()
        {
            if (closeOnBackdrop)
                Dismiss();
        }

        void ShowInternal(bool notify)
        {
            ResolveReferences();
            StopTransition();
            lastResult = ModalResult.None;

            if (root != null)
                root.SetActive(true);

            if (canvasGroup == null || fadeDuration <= 0f || !gameObject.activeInHierarchy)
            {
                SetShownImmediate(notify);
                return;
            }

            transition = StartCoroutine(FadeIn(notify));
        }

        void HideInternal(ModalResult result, bool notify)
        {
            ResolveReferences();

            if (!isVisible && (root == null || !root.activeSelf))
                return;

            StopTransition();

            if (canvasGroup == null || fadeDuration <= 0f || !gameObject.activeInHierarchy)
            {
                SetHiddenImmediate(result, notify);
                return;
            }

            transition = StartCoroutine(FadeOut(result, notify));
        }

        IEnumerator FadeIn(bool notify)
        {
            isVisible = true;
            SetCanvasState(0f, !disableInteractionDuringTransition, true);

            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += DeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            SetCanvasState(1f, true, true);
            transition = null;
            NotifyShown(notify);
        }

        IEnumerator FadeOut(ModalResult result, bool notify)
        {
            isVisible = false;
            lastResult = result;
            SetCanvasState(1f, !disableInteractionDuringTransition, true);

            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += DeltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            transition = null;
            SetHiddenImmediate(result, notify);
        }

        void SetShownImmediate(bool notify)
        {
            isVisible = true;
            lastResult = ModalResult.None;

            if (root != null)
                root.SetActive(true);

            SetCanvasState(1f, true, true);
            NotifyShown(notify);
        }

        void SetHiddenImmediate(ModalResult result, bool notify)
        {
            isVisible = false;
            lastResult = result;
            SetCanvasState(0f, false, false);

            if (root != null)
                root.SetActive(false);

            NotifyHidden(result, notify);
        }

        void SetCanvasState(float alpha, bool interactable, bool blocksRaycasts)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }

        void StopTransition()
        {
            if (transition == null)
                return;

            StopCoroutine(transition);
            transition = null;
        }

        void NotifyShown(bool notify)
        {
            if (!notify)
                return;

            shown.Invoke();
            Shown?.Invoke();
        }

        void NotifyHidden(ModalResult result, bool notify)
        {
            if (!notify)
                return;

            hidden.Invoke(result);
            Hidden?.Invoke(result);
        }

        float DeltaTime => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}
