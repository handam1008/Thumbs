using System;
using DevLib.ServiceLocator;
using DevLib.SoundSystem.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThumbAnimation : MonoBehaviour
{
    private Animator _animator;
    private bool IsHand;
    [SerializeField] private SoundClipSO thumbSound;
    [SerializeField] private RYU.Scirpt.Player.HandRaycast handRaycast;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            ThumbDown();
        // 전선을 보고 있을 때의 E는 미니게임 실행(MiniGameLauncher) 용이라 손 애니메이션을 건너뛴다
        if (Keyboard.current.eKey.wasPressedThisFrame && (handRaycast == null || handRaycast.Current == null))
            ThumbDownReverse();
        
    }

    private void ThumbDown()
    {
        _animator.Play("ThumbDown",0, 0f);
        ServiceLocator.Get<IAudioService>().PlaySfx(thumbSound);
    }
    private void ThumbDownReverse()
    {
        _animator.Play("ThumbUp", 0, 0f);
        ServiceLocator.Get<IAudioService>().PlaySfx(thumbSound);
    }
}
