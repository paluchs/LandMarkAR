using Controllers;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus.Home
{
    public class LoadExperimentMenu : MonoBehaviour
    {
        [SerializeField] private GameObject scrollContent;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private TextMeshProUGUI menuTitle;
        [SerializeField] private HomeController homeController;
        [SerializeField] private Interactable deleteAllButton;

        public void Start()
        {
            deleteAllButton.OnClick.AddListener(AppManager.DeleteExperimentFiles);
        }

        public void OnEnable()
        {
            InstantiateExperimentSelectionList();
        }


        private void InstantiateExperimentSelectionList()
        {
            if (homeController.ExperimentNames.Count > 0)
            {
                foreach (string experimentName in homeController.ExperimentNames)
                {
                    var button = InstantiateLoadExperimentButton(experimentName);
                }
            }
            else
            {
                menuTitle.text = "No existing experiments. Go back and create a new one.";
            }
        }

        private void DestroyExperimentSelectionList()
        {
            if (scrollContent.transform.childCount <= 0) return; 
            
            foreach (Transform button in scrollContent.transform)
            {
                Destroy(button.gameObject);
            }
        }

        private GameObject InstantiateLoadExperimentButton(string experimentName)
        {
            var basicButton = Instantiate(buttonPrefab, parent: scrollContent.transform);
        
            // Add Label and onClickListener to Button
            basicButton.GetComponent<Button>().onClick.AddListener(() => ExperimentSelected(experimentName));
            basicButton.GetComponentInChildren<TextMeshProUGUI>().text = experimentName;

            return basicButton;
        }

        private void ExperimentSelected(string experimentName)
        {
            DestroyExperimentSelectionList();
            AppManager.OpenExistingExperiment(experimentName);
        }

        public void BackToMainMenuClicked()
        {
            DestroyExperimentSelectionList();
            homeController.BackFromLoadExperimentMenu();
        }
    }
}
