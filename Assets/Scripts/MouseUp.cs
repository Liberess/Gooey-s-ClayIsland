using UnityEngine; 
using UnityEngine.EventSystems; //지시문 추가 

public class MouseUp : MonoBehaviour 
{ 
    void Update() 
    { 
        Debug.Log(EventSystem.current.IsPointerOverGameObject()); //UI 상에 마우스 올려져있으면 True, 아니면 False 
    }
}