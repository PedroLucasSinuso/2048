using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector2 _startPos, _curPos, _endPos;
    private float _swipeRange;
  
    public Vector2 InputType(){
        if(Input.GetKeyDown(KeyCode.Mouse0)) MouseSwipe();
        return KeyboardSwipe();
    }
    private Vector2 MouseSwipe(){
        
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            Debug.Log("Mouse down");
            _startPos = Input.mousePosition;
        }
        if(Input.GetKeyUp(KeyCode.Mouse0)){
            Debug.Log("Mouse up");
            _curPos = Input.mousePosition;
            Vector2 distance = _curPos - _startPos;

            if(distance.x < -_swipeRange) return Vector2.left;
            if(distance.x > _swipeRange) return Vector2.right;
            if(distance.x > _swipeRange) return Vector2.up;
            if(distance.x < -_swipeRange) return Vector2.down;
        }
        return Vector2.zero;

    }

    private Vector2 KeyboardSwipe(){
        if(Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2.left;
        if(Input.GetKeyDown(KeyCode.RightArrow)) return Vector2.right;
        if(Input.GetKeyDown(KeyCode.UpArrow)) return Vector2.up;
        if(Input.GetKeyDown(KeyCode.DownArrow)) return Vector2.down;
        return Vector2.zero;
    }
}
