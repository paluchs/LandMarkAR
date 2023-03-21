using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Controllers.Behavioral;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Task = System.Threading.Tasks.Task;

namespace Controllers.Functional
{
    public class AzureSpatialAnchorsController : MonoBehaviour
    {
        
        // Spatial Anchors
        [SerializeField] private SpatialAnchorManager spatialAnchorManager;

        // Progress
        [SerializeField] private ProgressIndicatorController progressIndicatorController;
        
        // Experiment
        // [SerializeField] private ExperimentController experimentController;
        
        // private void Start()
        // {
        //     // spatialAnchorManager.LogDebug += (sender, args) => SimpleConsole.AddLine(8, $"ASA - Debug: {args.Message}");
        //     // spatialAnchorManager.Error += (sender, args) => SimpleConsole.AddLine(8, $"ASA - Error: {args.ErrorMessage}");
        //     
        //     // spatialAnchorManager.AnchorLocated += experimentController.AnchorLocatedLoadAsset;
        // }
        
        private void OnDestroy()
        {
            spatialAnchorManager.StopSession();
            spatialAnchorManager.DestroySession();
        }
        
        public void AddAnchorLocatedEvent(AnchorLocatedDelegate anchorLocatedEvent)
        {
            spatialAnchorManager.AnchorLocated += anchorLocatedEvent;
        }

        public async Task<string> CreateAndSaveAzureSpatialAnchor(GameObject anchor, Dictionary<string, string> anchorProps)
        {
            SimpleConsole.AddLine(8, "Initializing Process to Upload Spatial Anchor");
            progressIndicatorController.message = "Initializing Process to Upload Spatial Anchor";
            
            // The progress indicator has to be started on the main thread
            UnityDispatcher.InvokeOnAppThread(() =>
            {
                progressIndicatorController.RunProgressBehavior();
            });
            
            progressIndicatorController.processRunning = true;
            
            if (!spatialAnchorManager.IsSessionStarted)
            {
                await spatialAnchorManager.StartSessionAsync();
            }
            
            //Add and configure ASA components
            CloudNativeAnchor cloudNativeAnchor = anchor.AddComponent<CloudNativeAnchor>();
            
            await cloudNativeAnchor.NativeToCloud();
            CloudSpatialAnchor cloudSpatialAnchor = cloudNativeAnchor.CloudAnchor;

            foreach (KeyValuePair<string, string> kvp in anchorProps)
            {
                cloudSpatialAnchor.AppProperties.Add(kvp);
            }

            // TODO Remove once we go live
            cloudSpatialAnchor.Expiration = DateTimeOffset.Now.AddDays(3);
            
            // Wait for spatial anchor manager to gather enough information to publish the anchor and show progress bar
            while (!spatialAnchorManager.IsReadyForCreate)
            {
                float createProgress = spatialAnchorManager.SessionStatus.RecommendedForCreateProgress;

                SimpleConsole.AddLine(8, $"ASA - Move your device to capture more environment data: {createProgress:0%}");
                // Update the progress of the progress indicator
                Debug.Log($"ASA - Move your device to capture more environment data: {createProgress:0%}");
                progressIndicatorController.message =
                    $"Creating Spatial Anchor... \n Move your device to capture more environment data.";
                progressIndicatorController.progress = createProgress;
            
                await Task.Yield();
            }
            
            // Update the Message to show that cloud Anchor is being uploaded
            progressIndicatorController.progress = 1;
            progressIndicatorController.message = $"Saving cloud anchor... ";
            SimpleConsole.AddLine(8, $"Saving cloud anchor... ");
        
            try
            {
                // Now that the cloud spatial anchor has been prepared, try the actual save here.
                await spatialAnchorManager.CreateAnchorAsync(cloudSpatialAnchor);
        
                bool saveSucceeded = cloudSpatialAnchor != null;
                if (!saveSucceeded)
                {
                    progressIndicatorController.message = $"ASA - Failed to save, but no exception was thrown.";
                    Debug.LogError("ASA - Failed to save, but no exception was thrown.");
                    SimpleConsole.AddLine(8, "ASA - Failed to save, but no exception was thrown.");
                    return null;
                }

                progressIndicatorController.message = $"Saved cloud anchor!";
                Debug.Log($"ASA - Saved cloud anchor with ID: {cloudSpatialAnchor.Identifier}");
                SimpleConsole.AddLine(8, $"ASA - Saved cloud anchor with ID: {cloudSpatialAnchor.Identifier}");
            }
            catch (Exception exception)
            {
                progressIndicatorController.message = "Failed to save anchor: \n" + exception;
                Debug.Log("ASA - Failed to save anchor: " + exception);
                // SimpleConsole.AddLine(8, "ASA - Failed to save anchor: " + exception.ToString());
                Debug.LogException(exception);
            }

            progressIndicatorController.processRunning = false;
            
            return cloudSpatialAnchor.Identifier;
        }

        public async Task DeleteAzureSpatialAnchor(GameObject anchor)
        {
            if (!spatialAnchorManager.IsSessionStarted)
            {
                await spatialAnchorManager.StartSessionAsync();
            }
            
            CloudNativeAnchor cloudNativeAnchor = anchor.GetComponent<CloudNativeAnchor>();
            
            if (cloudNativeAnchor == null) return;
            
            SimpleConsole.AddLine(8, "Deleting spatial anchor..");
            await spatialAnchorManager.DeleteAnchorAsync(cloudNativeAnchor.CloudAnchor);
            Destroy(cloudNativeAnchor);
            Destroy(anchor.GetComponent<ARAnchor>());
            SimpleConsole.AddLine(8, "Removed and deleted old anchor!");
        }


        public async Task UpdateLocateAzureSpatialAnchors(List<string> anchorIDs)
        {
            SimpleConsole.AddLine(8, $"Number of anchorIds to Update: {anchorIDs.Count}");
            if (!spatialAnchorManager.IsSessionStarted)
            {
                SimpleConsole.AddLine(8, "Starting new Session");
                await spatialAnchorManager.StartSessionAsync();
            }

            // Remove all existing watchers if there are some
            if (spatialAnchorManager.Session.GetActiveWatchers().Count > 0)
            {
                SimpleConsole.AddLine(8, $"Removing existing watchers");
                foreach (var watcher in spatialAnchorManager.Session.GetActiveWatchers())
                {
                    watcher.Stop();
                }
            }

            if (anchorIDs.Count > 0)
            {
                //Create a new watcher to look for all stored anchor IDs
                Debug.Log($"ASA - Creating watcher to look for {anchorIDs.Count} spatial anchors");
                SimpleConsole.AddLine(8, $"ASA - Creating watcher to look for {anchorIDs.Count} spatial anchors");
                var anchorLocateCriteria = new AnchorLocateCriteria
                {
                    Identifiers = anchorIDs.ToArray()
                };
                spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
                Debug.Log($"ASA - Watcher created!");
            }
        }
    }
}
