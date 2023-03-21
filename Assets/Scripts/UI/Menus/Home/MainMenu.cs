using Controllers;
using UnityEngine;

namespace UI.Menus.Home
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private HomeController homeController;

        public void OnNewExperimentClicked()
        {
            homeController.GoToCreateExperimentMenu();
        }

        public void OnLoadExperimentClicked()
        {
            homeController.GoToLoadExperimentMenu();
        }
    }
}
