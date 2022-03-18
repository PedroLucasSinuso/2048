using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonGeneric : MonoBehaviour
{

    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private TextMeshProUGUI _playerName;


    private void Start() {
        _playerName.text = PlayerPrefs.GetString("playerName","");
        
    }

    public void LoadScene(int scene){
        SceneManager.LoadScene(scene);
    }
    
    public void SetActive(GameObject gameobj){
        gameobj.SetActive(!gameobj.activeSelf);
    }

    public void CleanPlayerPrefs(){
        Debug.Log("Changed");
        PlayerPrefs.DeleteAll();
    }

    public void SetPlayerName(){
        PlayerPrefs.SetString("playerName",_playerNameField.text);
        _playerName.text = _playerNameField.text;
    }

    
}

