using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RYU.Scirpt
{
    public class GoldManager : MonoBehaviour
    {
        public static GoldManager Instance;
        [field:SerializeField] public int Gold { get; private set; }
        public event Action OnGoldChanged;

        public GameObject panel;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            panel.SetActive(false);
        }

        private void Update()
        {
            if (Gold > 2000)
            {
                panel.SetActive(true);
            }

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                AddGold(2000);
            }
        }

        public void AddGold(int gold)
        {
            Gold += gold;
            OnGoldChanged?.Invoke();
        }
    }
}