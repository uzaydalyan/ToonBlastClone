using System;
using System.Collections.Generic;
using Items;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ProgressManager : MonoBehaviour
    {
        public static ProgressManager Instance;
        [SerializeField] private int _moveCount;
        [SerializeField] private ItemType[] itemTypes;
        [SerializeField] private int[] goalCounts;
        [SerializeField] private GameObject _goalPrefab;
        [SerializeField] private HorizontalLayoutGroup _goalsParent;
        [SerializeField] private Text _moveCountText;

        private int _remainingMoveCount;
        private List<Text> _goalTexts = new(); 
        
        private List<Goal> _goals = new();

        private void Awake()
        {
            Instance = this;
            InitGame();
        }

        public void InitGame()
        {
            _remainingMoveCount = _moveCount;
            _moveCountText.text = _remainingMoveCount.ToString();
            
            for (int i = 0; i < itemTypes.Length; i++)
            {
                Goal goal = new Goal(itemTypes[i], goalCounts[i]);
                _goals.Add(goal);

                GameObject newGoal = Instantiate(_goalPrefab, _goalsParent.transform, false);
                newGoal.GetComponentInChildren<Text>().text = goalCounts[i].ToString();
                newGoal.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + itemTypes[i]);
                _goalTexts.Add(newGoal.GetComponentInChildren<Text>());
            }
        }

        public void DecreaseMoveCount()
        {
            _remainingMoveCount--;
            _moveCountText.text = _remainingMoveCount.ToString();
        }

        public GameStatus GetGameStatus()
        {
            if (CheckIfGoalsReached())
            {
                return GameStatus.Won;
            } else if (_remainingMoveCount == 0)

            {
                return GameStatus.Lost;
            }
            else
            {
                return GameStatus.Present;
            }
        }

        private bool CheckIfGoalsReached()
        {
            foreach (var goal in _goals)
            {
                if (goal.goal != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void DecreaseGoal(Item item)
        {
            int i = 0;
            
            foreach (var goal in _goals)
            {
                if (goal.itemType == item.type)
                {
                    goal.goal = goal.goal == 0 ? 0 : goal.goal-1;
                    _goalTexts[i].text = goal.goal.ToString();
                    AnimationManager.Instance.GoalAnimations(item.transform.position, item.type, _goalTexts[i].transform.position);
                }
                i++;
            }
        }

        public void ResetProgress()
        {
            _goals.Clear();
            _remainingMoveCount = _moveCount;
            _moveCountText.text = _remainingMoveCount.ToString();
            
            for (int i = 0; i < itemTypes.Length; i++)
            {
                Goal goal = new Goal(itemTypes[i], goalCounts[i]);
                _goals.Add(goal);
                _goalTexts[i].text = goalCounts[i].ToString();
            }
        }
    }
}