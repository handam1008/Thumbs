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

   
    public void ThumbDown()
    {
        _animator.Play("ThumbDown",0, 0f);
        ServiceLocator.Get<IAudioService>().PlaySfx(thumbSound);
    }
    public void ThumbDownReverse()
    {
        _animator.Play("ThumbUp", 0, 0f);
        ServiceLocator.Get<IAudioService>().PlaySfx(thumbSound);
    }
}
