using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TempleStatue : MonoBehaviour
{
    [SerializeField] private List<LightTotem> _lightTotems;
    [SerializeField] private BoxCollider2D _activateCollider;
    [SerializeField] private Vector3 _moveOffset;
    [SerializeField] private float _moveDuration = 3.0f;
    [SerializeField] private Ease _moveEase = Ease.OutCubic;

    private void Update()
    {
        var allTotemsActivated = _lightTotems.All(totem => totem.Activated);
        if (!allTotemsActivated)
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
    }
}
