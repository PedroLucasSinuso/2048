using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoardController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _width = 4; 
    [SerializeField] private int _height = 4;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _winCondition = 2048;

    [Header("Prefabs")]
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _board;


    [Header("UI References")]
    [SerializeField] private Score _scoreArea;
    [SerializeField] private GameObject _winScreen,_loseScreen;
    [SerializeField] private GameObject _gameOverScreen;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private BlockType GetBlockTypeByValue(int value) => _types.First(t=> t.Value == value);

    private GameState _state;
    private int _round;

    private void Start() => ChangeState(GameState.GenerateLevel);

    private void Update() {
        if(_state != GameState.WaitingInput) return;

        if(Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        if(Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
        if(Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
        if(Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);


    }

    

    private void ChangeState(GameState newState){
        _state = newState;

        switch(newState){

            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.ProcessInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                
                _winScreen.SetActive(true);
                break;
            case GameState.Lose:
                
                _loseScreen.SetActive(true);
                break;
        }
    }


    void GenerateGrid(){
        //Instantiante the node grid and add them into the node list
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();

        //Normalizing positions based on parent anchor
        var startPosX = transform.position.x+(_width/2)*-1;
        var startPosY = transform.position.y+(_height/2)*-1;
        var finalPosX = transform.position.x+(_width/2);
        var finalPosY = transform.position.y+(_height/2);

        for (float x = startPosX; x < finalPosX; x++){
            for (float y = startPosY; y < finalPosY; y++){

                var node = Instantiate(_nodePrefab,new Vector2(x,y),Quaternion.identity,transform);
                _nodes.Add(node);

            }
        }

        
        SpawnBoard();

        ChangeState(GameState.SpawningBlocks);

        
    }

    void SpawnBoard(){
        if(_board == null) return;
        var board = Instantiate(_board,transform.position,Quaternion.identity,transform);
        board.transform.position = new Vector2(board.transform.position.x-0.05f, board.transform.position.y-0.05f);
        board.size = new Vector2(_width+0.5f,_height+0.5f);
    }

    void SpawnBlocks(int amount){

        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList(); 

        foreach (var node in freeNodes.Take(amount)){       
            SpawnBlock(node, Random.value > 0.8f? 4 : 2);
        }
        

        //If are not free nodes
        if(freeNodes.Count() == 1){
            //Loop for all blocks and confirms if anyone can merge
            var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            foreach (var block in orderedBlocks){
                var nodeUp = GetNodeAtPosition(block.Node.Pos+Vector2.up);
                var nodeRight = GetNodeAtPosition(block.Node.Pos+Vector2.right);
                
                if(nodeUp != null && block.CanMerge(nodeUp.OccupiedBlock.Value)){
                    ChangeState(GameState.WaitingInput);
                    Debug.Log("Entrei e mudei p/ Waiting Input up");
                    return;
                }
        
                if(nodeRight != null && block.CanMerge(nodeRight.OccupiedBlock.Value)){
                    ChangeState(GameState.WaitingInput);
                    Debug.Log("Entrei e mudei p/ Waiting Input right");
                    return;
                }
                
            }
            //If anybody can merge than lose
            ChangeState(GameState.Lose);
            return;
        }

        

        ChangeState(_blocks.Any(b=> b.Value == _winCondition)? GameState.Win : GameState.WaitingInput);
    }

    void SpawnBlock(Node node, int value){

            var block = Instantiate(_blockPrefab,node.Pos,Quaternion.identity, transform);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            _blocks.Add(block);
    }

    
    void Move(List<Block> orderedBlocks, Vector2 dir){
        foreach (var block in orderedBlocks){
            var next = block.Node;
            do{
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);

                if(possibleNode != null){
                        
                    // if is possible to merge, than merge
                    if(possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value)){
                            
                        block.MergeBlock(possibleNode.OccupiedBlock);

                }
                    //otherwise, can we move to this spot? 
                    else if(possibleNode.OccupiedBlock == null) next = possibleNode;
                }
            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

            
        foreach (var block in orderedBlocks){
                
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;
                
            sequence.Insert(0,block.transform.DOMove(movePoint,_travelTime));

            sequence.OnComplete(()=>{

                foreach (var block in orderedBlocks.Where(b=>b.MergingBlock != null)){                  
                    MergeBlocks(block.MergingBlock, block);
                }
                            
                ChangeState(GameState.SpawningBlocks);
                    
            });               
        }
    }
    
    void Shift(Vector2 dir){
        ChangeState(GameState.ProcessInput);
        
        //Ordering the blocks list to properly calculate their position
        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if(dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();



        ChangeState(StateBasedOnMove(orderedBlocks,dir));
    
        if(_state == GameState.Moving) Move(orderedBlocks,dir);
        
    }

    GameState StateBasedOnMove(List<Block> orderedBlocks, Vector2 dir){
        
        foreach (var block in orderedBlocks){
            var nextNode = GetNodeAtPosition(block.Node.Pos + dir);
            var nextBlock = nextNode == null ? null: nextNode.OccupiedBlock;
            var canMerge = nextBlock == null ? false: block.CanMerge(nextBlock.Value);            

            if(nextNode != null && nextBlock == null || canMerge) return GameState.Moving;
            
        }
        return GameState.WaitingInput;
    }
    
    void MergeBlocks(Block baseBlock, Block mergingBlock){
        SpawnBlock(baseBlock.Node, baseBlock.Value*2);
        
        _scoreArea.ScoreUp(baseBlock.Value*2);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block){
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 pos){
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }

    

}

    

[System.Serializable]
public struct BlockType{
    public Sprite Sprite;
    public int Value;
    public Color Color;
}

public enum GameState{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    ProcessInput,
    Moving,
    Win,
    Lose
}
