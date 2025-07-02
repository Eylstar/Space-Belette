using UnityEngine;

public class Showcase : MonoBehaviour
{
    string TxtSample = "This is a sample text to demonstrate the Showcase functionality.";
    [Button]
    void VarSample() 
    {
        Debug.Log(TxtSample);
    }
    [Button]
    void StringSample(string input)
    {
        Debug.Log("Received string: " + input);
    }
    [Button]
    void IntSample(int number)
    {
        Debug.Log("Received integer: " + number);
    }
    [Button]
    void GameObjSample(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Received GameObject: " + obj.name);
        }
        else
        {
            Debug.Log("Received null GameObject");
        }
    }
    [Button]
    void ScriptSample(Object obj) 
    {
        if (obj != null)
        {
            Debug.Log("Received Scrip: " + obj.name);
        }
        else
        {
            Debug.Log("Received null Script");
        }
    }
    [Button]
    void DemoOnScriptable(int XpValue) 
    {
       var player = Resources.Load<PlayerStatsSo>("ScriptableObjects/PlayerStatsSO");
        player.AddExperience(XpValue);
    }
}