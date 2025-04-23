using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TempleStatue : MonoBehaviour
{
    [Serializable]
    private class LightTotemEye
    {
        [SerializeField] private LightTotem _lightTotem;
        [SerializeField] private GameObject _eye;
        
        public LightTotem LightTotem => _lightTotem;
        public GameObject Eye => _eye;
    }
    
    [SerializeField] private List<LightTotemEye> _lightTotemEyes;
    [SerializeField] private BoxCollider2D _activateCollider;
    [SerializeField] private Vector3 _moveOffset;
    [SerializeField] private float _moveDuration = 3.0f;
    [SerializeField] private Ease _moveEase = Ease.OutCubic;
    [SerializeField] private AudioClip _moveSfx;

    private void Update()
    {
        var activatedLightTotems = 0;
        
        foreach (var lightTotemEye in _lightTotemEyes)
        {
            if (!lightTotemEye.LightTotem.Activated) continue;
            
            lightTotemEye.Eye.SetActive(true);
            activatedLightTotems++;
        }
        
        if (activatedLightTotems != _lightTotemEyes.Count)
        {
            return;
        }
        
        Move();
        enabled = false;
    }

    private void Move()
    {
        transform.DOMove(transform.position + _moveOffset, _moveDuration).SetEase(_moveEase).OnComplete(() =>
        {
            _activateCollider.enabled = true;
        });
        
        GameManager.Instance.AudioManager.PlaySoundEffect(_moveSfx);
    }
}
