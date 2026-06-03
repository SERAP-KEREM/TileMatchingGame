using Assets.Scripts.Runtime.TileMatchingGame.Controller;
using Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Runtime.TileMatchingGame.Services
{
    public class LevelButtonFactory
    {
        private readonly LevelManager _levelManager;
        private readonly Button _levelButtonPrefab;
        private readonly RectTransform _levelButtonParent;

        public LevelButtonFactory(LevelManager levelManager, Button buttonPrefab, RectTransform levelButtonParent)
        {
            _levelManager = levelManager;
            _levelButtonPrefab = buttonPrefab;
            _levelButtonParent = levelButtonParent;
        }

        public void CreateButton(Level level, int levelIndex)
        {
            Button newButton = Object.Instantiate(_levelButtonPrefab, _levelButtonParent);
            TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
            text.text = level.GetDisplayName();
            newButton.onClick.AddListener(() => LevelButtonClickHandler(levelIndex));
        }

        public void LevelButtonClickHandler(int levelIndex)
        {
            _levelManager.SetLevel(levelIndex);
            _levelManager.LoadLevel();
        }
    }
}
