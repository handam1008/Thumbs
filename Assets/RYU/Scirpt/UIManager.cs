using System;
using TMPro;
using UnityEngine;

namespace RYU.Scirpt
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        [SerializeField] private TextMeshProUGUI GoldText;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GoldManager.Instance.OnGoldChanged += () => GoldText.SetText("골드:" + GoldManager.Instance.Gold.ToString());
        }
    }
}