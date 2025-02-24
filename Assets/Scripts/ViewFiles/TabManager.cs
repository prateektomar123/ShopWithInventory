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
        // Set up button listeners
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i; // Store the index for the lambda
            tabButtons[i].onClick.AddListener(() => SelectTab(tabIndex));
        }

        // Default to first tab
        SelectTab(0);
    }

    public void SelectTab(int tabIndex)
    {
        // Activate the selected panel, deactivate others
        for (int i = 0; i < contentPanels.Length; i++)
        {
            contentPanels[i].SetActive(i == tabIndex);

            // Update button visuals
            ColorBlock colors = tabButtons[i].colors;
            colors.normalColor = (i == tabIndex) ? activeTabColor : inactiveTabColor;
            tabButtons[i].colors = colors;
        }
    }
}