using System;
using System.Collections;
using AssetFactories;
using Controllers;
using Controllers.Asset;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;


#if WINDOWS_UWP
using GLTFast;
using UnityEngine.WSA;
// using Microsoft.MixedReality.WorldLocking.Tools;
#endif

namespace UI.Menus
{
    public class HandMenu : MonoBehaviour
    {
        [SerializeField] private ExperimentController experimentController;
        [SerializeField] private GameObject simpleConsole;
        [SerializeField] private GameObject eyeGazeCursor;

        [SerializeField] private Interactable addAssetButton;
        [SerializeField] private Interactable settingsButton;
        [SerializeField] private Interactable quitExperimentButton;
    
        [SerializeField] private Interactable add3DPrimitiveButton;
        [SerializeField] private Interactable add3DTextButton;
        [SerializeField] private Interactable add3DLineButton;
        [SerializeField] private Interactable add3DObjectButton;
        
        [SerializeField] private Interactable lockAssetsToggle;
        [SerializeField] private Interactable showEyeTrackerToggle;
        [SerializeField] private Interactable showSimpleConsoleToggle;
        
        [SerializeField] private GameObject addAssetMenu;
        [SerializeField] private GameObject settingsMenu;


        private bool _elementCreated;
        
        // Start is called before the first frame update
        void Start()
        {
            addAssetButton.OnClick.AddListener(OpenAddAssetMenu);
            settingsButton.OnClick.AddListener(OpenSettingsMenu);
            quitExperimentButton.OnClick.AddListener(experimentController.CloseExperiment);
            
            // Add assets
            add3DPrimitiveButton.OnClick.AddListener(Add3DPrimitive);
            add3DTextButton.OnClick.AddListener(Add3DText);
            add3DLineButton.OnClick.AddListener(Add3DLine);
            add3DObjectButton.OnClick.AddListener(AddGltf);
            
            // Settings
            lockAssetsToggle.OnClick.AddListener(experimentController.ToggleAssetSelection);
            showEyeTrackerToggle.OnClick.AddListener(ToggleShowEyeTracker);
            showSimpleConsoleToggle.OnClick.AddListener(ToggleShowSimpleConsole);
        }

        public void OnHandLowered()
        {
            addAssetMenu.SetActive(false);
            settingsMenu.SetActive(false);
        }

        private void OpenAddAssetMenu()
        {
            addAssetMenu.SetActive(true);
            settingsMenu.SetActive(false);
        }

        private void OpenSettingsMenu()
        {
            addAssetMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }

        public void Add3DPrimitive()
        {
            var assetContainer = experimentController.InstantiateAsset(new Asset3DFactory());
            // Invoke the selection onClick so that the user can start to adjust the asset to their liking
            assetContainer.GetComponent<Interactable>().OnClick.Invoke();
        }

        public void Add3DText()
        {
            var assetContainer = experimentController.InstantiateAsset(new AssetTextFactory());
            // Invoke the selection onClick so that the user can start to adjust the asset to their liking
            assetContainer.GetComponent<Interactable>().OnClick.Invoke();
        }
        
        public void AddGltf()
        {
            Debug.Log("Test");
            StartCoroutine(nameof(SelectLink));
        }
        
        private IEnumerator SelectLink()
        {
            var helper = new TextHelper(); 
            OpenGltfLink(helper);
            
            while (helper.text is null)
            {
                yield return new WaitForSeconds(0.1f);
            }

            var assetContainer = experimentController.InstantiateAsset(new AssetGltfFactory());
            var assetController = assetContainer.GetComponent<AssetGltfController>();

            SimpleConsole.AddLine(8, helper.text);
            assetController.GltfUrl = helper.text;

            // Invoke the selection onClick so that the user can start to adjust the asset to their liking
            assetContainer.GetComponent<Interactable>().OnClick.Invoke();
        }

        private void Add3DLine()
        {
            throw new NotImplementedException();
        }
        

        private void ToggleShowEyeTracker()
        {
            eyeGazeCursor.SetActive(!eyeGazeCursor.activeSelf);
        }

        private void ToggleShowSimpleConsole()
        {
            simpleConsole.SetActive(!simpleConsole.activeSelf);
        }
        
# if WINDOWS_UWP 
        private void OpenGltfLink(TextHelper helper)
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
            {
                SimpleConsole.AddLine(8, "Trying to load from .txt file..");
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                SimpleConsole.AddLine(8, "Created a text picker");
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Objects3D;
                // Dropdown of file types the user can save the file as
                picker.FileTypeFilter.Add(".txt");
                var currentFile = await picker.PickSingleFileAsync();
                SimpleConsole.AddLine(8, "Opened file picker..");
                UnityEngine.WSA.Application.InvokeOnAppThread( async () =>
                {
                    var link = "";
                    if (currentFile != null)
                    {
                        // Application now has read/write access to the picked file
                        link = await Windows.Storage.FileIO.ReadTextAsync(currentFile);
                    }
                    SimpleConsole.AddLine(8, link);
                    helper.text = link;
                }, true);
            }, false);
        }
#else
        private void OpenGltfLink(TextHelper helper)
        {
            helper.text = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";
            Debug.Log("Opening a .txt file only works on HoloLens");
        }
#endif

        private class TextHelper
        {
            public string text;
        }
        
        
//         private async void OpenglTF()
//         {
//             string filename = null;
//             byte[] glb = new byte[] { };
//
// # if WINDOWS_UWP
//             UnityEngine.WSA.Application.InvokeOnUIThread(async (filename, glb) =>
//             {
//                 SimpleConsole.AddLine(8, "Trying to load from .glTF file..");
//                 var picker = new Windows.Storage.Pickers.FileOpenPicker();
//                 SimpleConsole.AddLine(8, "Created a text picker");
//                 picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Objects3D;
//                 // Dropdown of file types the user can save the file as
//                 picker.FileTypeFilter.Add(".glb");
//                 var currentFile = await picker.PickSingleFileAsync();
//                 Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(currentFile);
//
//                 UnityEngine.WSA.Application.InvokeOnAppThread( async (filename, glb) =>
//                 {
//                     if (currentFile != null)
//                     {
//                         // Application now has read/write access to the picked file
//                         filename = currentFile.Name;
//                         var buffer = await Windows.Storage.FileIO.ReadBufferAsync(currentFile);
//                         using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
//                         {
//                             dataReader.ReadBytes(glb);
//                         }
//                     }
//                 }, true);
//             }, false);
// #endif
//
//
//
//             SimpleConsole.AddLine(8, $"Filename is {filename}");
//             // if (filename is null) return;
//
//             GameObject result = Importer.LoadFromBytes(glb);
//
//             experimentController.InstantiateAsset(new AssetGlbFactory(result));
//         }
    
    }
}
