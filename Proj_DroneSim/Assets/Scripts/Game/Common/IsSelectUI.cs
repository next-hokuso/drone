using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmnInputFieled_SelectTextClear : MonoBehaviour, ISelectHandler
{
    //================================================
    // [///]
    //================================================
    private InputField m_InputField = null;
    public void Start()
    {
        m_InputField = GetComponent<InputField>();
    }

    //================================================
    // [///] public
    //================================================
    public void OnSelect(BaseEventData eventData)
    {
        m_InputField.text = "";
    }
}
