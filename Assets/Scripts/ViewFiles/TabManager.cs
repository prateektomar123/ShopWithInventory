using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject[] contentPanels;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color activeTabColor = Color.white;
    [SerializeField] private Color inactiveTabColor = new Color(0.8f, 0.8f, 0.8f);

    private void Start()
    {
        
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i; 
            tabButtons[i].onClick.AddListener(() => SelectTab(tabIndex));
        }

        
        SelectTab(0);
    }

    public void SelectTab(int tabIndex)
    {
        
        for (int i = 0; i < contentPanels.Length; i++)
        {
            contentPanels[i].SetActive(i == tabIndex);

           
            ColorBlock colors = tabButtons[i].colors;
            colors.normalColor = (i == tabIndex) ? activeTabColor : inactiveTabColor;
            tabButtons[i].colors = colors;
        }
    }
}