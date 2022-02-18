using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{   
    public Vector2 Pos => transform.position; 
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    public int Value;
    public Node Node;
    public Block MergingBlock;
    public bool Merging;

    public void Init(BlockType type){
        Value = type.Value;
        if(type.Sprite != null) _renderer.sprite = type.Sprite;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
    }

    public void SetBlock(Node node){
        if(Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock (Block blockToMergeWith){

        MergingBlock = blockToMergeWith;

        Node.OccupiedBlock = null;

        blockToMergeWith.Merging = true;

    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null;

    
}
