using TMPro;
using UnityEngine;

public class InfoPointHeadingController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _headingTextField;
    [SerializeField] private GameObject _headingPanelGameObject;

    private void Awake()
    {
        ClearInfoPointHeading();
    }

    void OnEnable()
    {
        Events.DisplayInfoPointHeading.Subscribe(DisplayInfoPointHeading);
        Events.ClearInfoPointHeading.Subscribe(ClearInfoPointHeading);
    }

    void OnDisable()
    {
        Events.DisplayInfoPointHeading.Unsubscribe(DisplayInfoPointHeading);
        Events.ClearInfoPointHeading.Unsubscribe(ClearInfoPointHeading);
    }


    void DisplayInfoPointHeading(string headingText)
    {
        if (headingText.Length > 0)
        {
            _headingTextField.text = headingText;
            _headingPanelGameObject.SetActive(true);
        }
    }

    void ClearInfoPointHeading()
    {
        _headingPanelGameObject.SetActive(false);
        _headingTextField.text = "";
    }
}
