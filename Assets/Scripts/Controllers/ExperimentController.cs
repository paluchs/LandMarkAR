using System;
using System.Collections.Generic;
using AssetFactories;
using Controllers.Asset;
using Controllers.Functional;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.WorldLocking.Tools;
using Models;
using UI.Menus.Asset;
using UnityEngine;

namespace Controllers
{
    public class ExperimentController : MonoBehaviour
    {
        private Experiment _experiment;
        private readonly List<GameObject> _foundOrCreatedAssetContainers = new();

        [Header("AzureSpatialAnchors")]
        [SerializeField] private AzureSpatialAnchorsController azureSpatialAnchorsController;
        
        [Header("UserCameraReference")]
        [SerializeField] public PlayerController playerController;
        
        [Header("Menus")]
        [SerializeField] private GameObject handMenu;
        [SerializeField] public AssetMenu experiment3DAssetMenu;
        [SerializeField] public AssetMenu experimentTextMenu;
        [SerializeField] public AssetMenu experimentGltfMenu;
        
        // State
        private bool _assetsLocked;

        
        /// <summary>
        /// Makes sure that the user cannot select another asset or change the experiment settings while editing an Asset
        /// </summary>
        private bool _assetSelected;
        public bool AssetSelected
        {
            get => _assetSelected;
            set
            {
                _assetSelected = value;
                if (value)
                {
                    HideHandMenu();
                    DisableSelectionForEachExperimentAsset();
                }
                else
                {
                    ShowHandMenu();
                    EnableSelectionForEachExperimentAsset();
                }
            }
        }

        // Load the Experiment on Start
        public void Awake()
        {
            LoadExperimentFromFile();
        }

        public void Start()
        {
            azureSpatialAnchorsController.AddAnchorLocatedEvent(AnchorLocatedLoadAsset);
        }

        private void SaveExperimentToFile()
        {
            AppManager.SaveExperimentToFile(_experiment);
        }
        
        private async void LoadExperimentFromFile()
        {
            _experiment = await AppManager.ReadExperimentFromFile(PlayerPrefs.GetString("openExperimentName"));

            if (_experiment.createdAnchorIDs.Count <= 0) return;
            
            // If the experiment already contains spatial anchors locate them
            SimpleConsole.AddLine(8, "Experiment already contains spatial anchors...");
            await azureSpatialAnchorsController.UpdateLocateAzureSpatialAnchors(_experiment.createdAnchorIDs);
            SimpleConsole.AddLine(8, "UpdatedAzureLocateSpatialAnchors...");
            
            SaveExperimentToFile();
        }
        
        public void CloseExperiment()
        {
            SaveExperimentToFile();
            AppManager.GoToStartScene();
        }
        
        /// <summary>
        /// Instantiates the Asset as a 3D GameObject, but does not yet create an ASA
        /// </summary>
        /// <param name="assetFactory"></param>
        public GameObject InstantiateAsset(AssetFactory assetFactory)
        {
            if (_assetsLocked) return null;
            
            var position = playerController.GetPositionTowardsHead();
            var orientationTowardsHead = playerController.GetOrientationTowardsHead(position);
            
            var assetContainer = assetFactory.InstantiateAsset(position, orientationTowardsHead, transform);

            return assetContainer;
        }
        
        /// <summary>
        /// Creates the ASA (i.e., saves the asset) and adds it to the experiment.
        /// The Experiment is then also saved
        /// </summary>
        /// <param name="assetContainer"></param>
        public async void AddAsset(GameObject assetContainer)
        {
            var assetController = assetContainer.GetComponent<AssetController>();
            foreach (var kvp in assetController.GetAnchorProps())
            {
                SimpleConsole.AddLine(8, $"{kvp.Key}, {kvp.Value}");
            }
            var anchorId = await azureSpatialAnchorsController.CreateAndSaveAzureSpatialAnchor(assetContainer, assetController.GetAnchorProps());
            
            _experiment.createdAnchorIDs.Add(anchorId);
            _foundOrCreatedAssetContainers.Add(assetContainer);
            
            await azureSpatialAnchorsController.UpdateLocateAzureSpatialAnchors(_experiment.createdAnchorIDs);
            
            SaveExperimentToFile();
        }
        
        
        /// <summary>
        /// Removes the asset from the experiment and deletes the ASA
        /// The 3D GameObject is still there
        /// </summary>
        /// <param name="assetContainer"></param>
        public async void RemoveAsset(GameObject assetContainer)
        {

            var anchor = assetContainer.GetComponent<CloudNativeAnchor>();
            
            if (anchor is not null)
            {
                _experiment.createdAnchorIDs.Remove(anchor.CloudAnchor.Identifier);
                _foundOrCreatedAssetContainers.Remove(assetContainer);
                await azureSpatialAnchorsController.DeleteAzureSpatialAnchor(assetContainer);
            }
            
            await azureSpatialAnchorsController.UpdateLocateAzureSpatialAnchors(_experiment.createdAnchorIDs);
            
            // var allComponents = assetContainer.GetComponents<MonoBehaviour>();
            // foreach (var component in allComponents) {
            //     if (component.GetType() != typeof(AssetController))
            //     {
            //         SimpleConsole.AddLine(8, component.GetType().ToString());
            //     }
            // }
        }
        
        /// <summary>
        /// Called when a spatial anchor is detected by the AzureSpatialAnchorController.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnchorLocatedLoadAsset(object sender, AnchorLocatedEventArgs args)
        {
            SimpleConsole.AddLine(8, $"ASA - Anchor recognized as a possible anchor {args.Identifier} {args.Status}");
            
            if (args.Status == LocateAnchorStatus.Located)
            {
                // Creating and adjusting GameObjects have to run on the main thread. We are using the UnityDispatcher to make sure this happens.
                UnityDispatcher.InvokeOnAppThread(() =>
                {
                    // Read out Cloud Anchor values
                    var cloudSpatialAnchor = args.Anchor;
                    var parent = transform;
                    
                    //Create GameObject
                    try
                    {
                        // Create the AssetFactory
                        var assembly = typeof(AssetFactory).Assembly;
                        var t = assembly.GetType($"AssetFactories.{cloudSpatialAnchor.AppProperties["factory-type"]}");
                        var assetFactory = (AssetFactory)Activator.CreateInstance(t);
                        
                        var assetContainer = assetFactory.InstantiateAsset(Vector3.zero, Quaternion.identity, parent);
                        
                        // Add the spatial Anchor
                        assetContainer.AddComponent<CloudNativeAnchor>().CloudToNative(cloudSpatialAnchor);
                        var assetController = assetContainer.GetComponent<AssetController>();
                        
                        // Set the properties to the visual
                        assetController.SetAnchorProps(cloudSpatialAnchor.AppProperties);
                        
                        // Make sure the object is known
                        _foundOrCreatedAssetContainers.Add(assetContainer);
                    }
                    catch (Exception e)
                    {
                        // Todo Create AssetNotFound Type
                        SimpleConsole.AddLine(8, e.Message);
                        var anchorGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        anchorGameObject.transform.localScale = Vector3.one * 0.1f;
                        anchorGameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Legacy Shaders/Diffuse");
                        anchorGameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                        anchorGameObject.AddComponent<CloudNativeAnchor>().CloudToNative(cloudSpatialAnchor);
                    }
                });
            }
        }

        public void ToggleAssetSelection()
        {
            if (_assetsLocked)
            {
                EnableSelectionForEachExperimentAsset();
                _assetsLocked = false;
            }
            else
            {
                DisableSelectionForEachExperimentAsset();
                _assetsLocked = true;
            }
        }
        
        
        private void EnableSelectionForEachExperimentAsset()
        {
            foreach (var asset in _foundOrCreatedAssetContainers)
            {
                asset.GetComponent<AssetController>().EnableSelectInteraction();
            }
        }
    
        private void DisableSelectionForEachExperimentAsset()
        {
            foreach (var asset in _foundOrCreatedAssetContainers)
            {
                asset.GetComponent<AssetController>().DisableSelectInteraction();
            }
        }

        private void HideHandMenu()
        {
            handMenu.SetActive(false);
        }

        private void ShowHandMenu()
        {
            handMenu.SetActive(true);
        }
    }
}
