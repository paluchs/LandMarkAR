using Controllers;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace UI.Menus.Home
{
    public class CreateExperimentMenu : MonoBehaviour
    {
        public MRTKTMPInputField nameInputField;

        [SerializeField] private TextMeshProUGUI menuTitle;
        [SerializeField] private HomeController homeController;
    
        
        public void BackToMainMenuClicked()
        {
            homeController.BackFromCreateExperimentMenu();
        }

        public void CreateExperimentClicked()
        {
            if (homeController.ExistsExperimentName(nameInputField.text))
            {
                menuTitle.text = "This Experiment Name already exists! Take another Name";
                menuTitle.canvasRenderer.SetColor(Color.red);
                return;
            }

            if (nameInputField.text == "")
            {
                menuTitle.text = "Type a name for the experiment!";
                menuTitle.canvasRenderer.SetColor(Color.red);
                return;
            }
            AppManager.OpenNewExperiment(nameInputField.text);
        }
    }
}
