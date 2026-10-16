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
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            ThumbDown();
        if (Keyboard.current.eKey.wasPressedThisFrame)
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
