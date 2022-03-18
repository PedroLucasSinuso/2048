using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] private string _onStart,_onEnable;
    [SerializeField] private Animator _animator;
    [SerializeField] private TMP_InputField _playerNameField;

    private void Start() {
        if(_onStart != null) _animator.Play(_onStart);
    }
    private void OnEnable() {
        if(_onEnable != null) _animator.Play(_onEnable);
        
    }

    
}
