using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.WorldLocking.Tools;
using UI.Menus;
using UI.Menus.Home;
using UnityEngine;

namespace Controllers
{
    public class HomeController : MonoBehaviour
    {
            
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject loadExperimentMenu;
        [SerializeField] private GameObject createExperimentMenu;
        
        public List<string> ExperimentNames { get; private set; }

        public void Start()
        {
            SimpleConsole.AddLine(8, "Start of HomeController called");
            LoadExperimentNames();
        }


        // ----------- Main Menu Navigation --------------
        public void GoToLoadExperimentMenu()
        {
            mainMenu.SetActive(false);
            loadExperimentMenu.SetActive(true);
        }
    
        public void GoToCreateExperimentMenu()
        {
            mainMenu.SetActive(false);
            createExperimentMenu.SetActive(true);
            createExperimentMenu.GetComponent<CreateExperimentMenu>().nameInputField.ActivateInputField();
        }

        public void BackFromLoadExperimentMenu()
        {
            loadExperimentMenu.SetActive(false);
            LoadExperimentNames();
            mainMenu.SetActive(true);
        }

        public void BackFromCreateExperimentMenu()
        {
            createExperimentMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        
        private void LoadExperimentNames()
        {
            ExperimentNames = new List<string>();

            var info = new DirectoryInfo(Application.persistentDataPath);
            var fileInfo = info.GetFiles();
            if (fileInfo.Length <= 0) return;
        
            foreach (var file in fileInfo)
            {
                var fileName = file.Name;
                if (fileName[^5..] == ".json")
                {
                    ExperimentNames.Add(fileName[..^5]);
                }
                
            }
        }
        public bool ExistsExperimentName(string experimentName)
        {
            return ExperimentNames.Contains(experimentName);
        }

    }
}