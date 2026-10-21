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

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                AddGold(500);
            }
        }

        public void AddGold(int gold)
        {
            Gold += gold;
            OnGoldChanged?.Invoke();
        }
    }
}