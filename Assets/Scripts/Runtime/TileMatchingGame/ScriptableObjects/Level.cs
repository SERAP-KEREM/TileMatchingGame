using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level/LevelData")]
    public class Level : ScriptableObject
    {
        [Tooltip("Shown in the HUD during gameplay. Falls back to the asset name if empty.")]
        public string DisplayName;

        public int BoardWidth; 
        public int BoardHeight;
        public int TileMatchPoints;
        public GoalSetup[] LevelGoals;

        [System.Serializable]
        public struct GoalSetup
        {
            public GoalsEnum goalEnum;
            public int maxPoints;
            public TileColor tileColor;
            public int tileQuantity;
        }

        public enum GoalValueType
        {
            Int,
            Float,
            String
        }

        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
            {
                return DisplayName.Trim();
            }

            Match levelNumberMatch = Regex.Match(name, @"^Level[_\s]*(\d+)$", RegexOptions.IgnoreCase);
            if (levelNumberMatch.Success)
            {
                return $"Level {levelNumberMatch.Groups[1].Value}";
            }

            return name.Replace('_', ' ');
        }
    }
}