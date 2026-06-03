using System;
using System.Linq;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Runtime.TileMatchingGame.View
{
    public class GoalsPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _goalsText;
        [SerializeField] private Button _closeButton;

        public event Action OnCloseRequested;

        private void Awake()
        {
            ResolveReferences();
            Hide();
        }

        private void OnEnable()
        {
            BindCloseButton();
        }

        private void OnDisable()
        {
            UnbindCloseButton();
        }

        public void Show(IGoalManager goalManager)
        {
            Refresh(goalManager);
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void Refresh(IGoalManager goalManager)
        {
            ResolveReferences();

            if (_goalsText != null)
            {
                _goalsText.text = BuildGoalsText(goalManager);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public static string BuildGoalsText(IGoalManager goalManager)
        {
            if (goalManager == null || goalManager.CurrentLevelGoals == null || !goalManager.CurrentLevelGoals.Any())
            {
                return "<b>Level Goals</b>\n\nNo objectives configured for this level.";
            }

            var lines = goalManager.CurrentLevelGoals.Select(goal =>
                $"<b>{goal.Goal}</b>\n{goal.GetDescription()}\nProgress: {goal.GetProgress()}");

            return "<b>Level Goals</b>\n\n" + string.Join("\n\n", lines);
        }

        public static string BuildGoalsSummaryText(IGoalManager goalManager)
        {
            if (goalManager == null || goalManager.CurrentLevelGoals == null || !goalManager.CurrentLevelGoals.Any())
            {
                return "Goals: none configured";
            }

            var lines = goalManager.CurrentLevelGoals.Select(goal =>
                $"{goal.Goal}: {goal.GetProgress()}");

            return string.Join("  |  ", lines);
        }

        private void ResolveReferences()
        {
            if (_goalsText == null)
            {
                _goalsText = GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (_closeButton == null)
            {
                foreach (var button in GetComponentsInChildren<Button>(true))
                {
                    if (button.gameObject.name == "ExitButton")
                    {
                        _closeButton = button;
                        break;
                    }
                }

                if (_closeButton == null)
                {
                    _closeButton = GetComponentInChildren<Button>(true);
                }
            }
        }

        private void BindCloseButton()
        {
            if (_closeButton == null)
            {
                return;
            }

            _closeButton.onClick.RemoveListener(HandleCloseClicked);
            _closeButton.onClick.AddListener(HandleCloseClicked);
        }

        private void UnbindCloseButton()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        private void HandleCloseClicked()
        {
            OnCloseRequested?.Invoke();
        }
    }
}
