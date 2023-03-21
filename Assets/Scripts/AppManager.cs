using System.IO;
using System.Threading.Tasks;
using Microsoft.MixedReality.WorldLocking.Tools;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AppManager {
    // ------------ Scene Management -------------------
    public static void GoToStartScene()
    {
        SceneManager.LoadScene("Scenes/StartScene");
    }

    public static void OpenExistingExperiment(string experimentName)
    {
        PlayerPrefs.SetString("openExperimentName", experimentName);
        SceneManager.LoadScene("Scenes/ExperimentScene");
        // StartCoroutine(LoadExperiment(GetExperimentFromFile(experimentName)));
    }
    
    public static void OpenNewExperiment(string experimentName)
    {
        SaveExperimentToFile(new Experiment(experimentName));
        OpenExistingExperiment(experimentName);
        // StartCoroutine(LoadExperiment(new Experiment(experimentName)));
    }
    
    // ----------------- File Management --------------------
    
    /// <summary>
    /// Serializes an experiment to a json (i.e. a list of ASA-IDs)
    /// </summary>
    /// <param name="experiment"></param>
    public static void SaveExperimentToFile(Experiment experiment)
    {
        SimpleConsole.AddLine(8, "Experiment Saved to file!");
        File.WriteAllText($"{Application.persistentDataPath}/{experiment.Name}.json", JsonConvert.SerializeObject(experiment));
    }

    /// <summary>
    /// Deserializes an experiment from a json
    /// </summary>
    /// <param name="experimentName"></param>
    /// <returns></returns>
    public static async Task<Experiment> ReadExperimentFromFile(string experimentName)
    {
        var json = await File.ReadAllTextAsync($"{Application.persistentDataPath}/{experimentName}.json");
        return JsonConvert.DeserializeObject<Experiment>(json);
    }
    
    // ----------------- Development Functions --------------------
    /// <summary>
    /// Creates two experiments as examples
    /// </summary>
    public static void CreateExampleExperiments()
    {
        Experiment test = new Experiment("Eye tracking experiment"); 
        Experiment test1 = new Experiment("Bertin variable Experiment");
        
        File.WriteAllText($"{Application.persistentDataPath}/{test.Name}.json", JsonConvert.SerializeObject(test));
        File.WriteAllText($"{Application.persistentDataPath}/{test1.Name}.json", JsonConvert.SerializeObject(test1));
    }
    
    /// <summary>
    /// Deletes all existing experiments
    /// </summary>
    public static void DeleteExperimentFiles()
    {
        SimpleConsole.AddLine(8, "Deleting existing experiment Files for development purposes!");
        var info = new DirectoryInfo(Application.persistentDataPath);
        var fileInfo = info.GetFiles();
        if (fileInfo.Length <= 0) return;
        
        foreach (var file in fileInfo)
        {
            var fileName = file.Name;
            if (fileName[^5..] == ".json")
            {
                File.Delete(Path.Combine(Application.persistentDataPath, fileName));
            }
        }
    }
    


}


//Filepicker code:
// Todo: Use this for export/import function
// # if WINDOWS_UWP
//     var picker = new Windows.Storage.Pickers.FileOpenPicker();
//     savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
//     // Dropdown of file types the user can save the file as
//     savePicker.FileTypeFilter.Add(".json");
//
//     currentFile = await picker.PickSingleFileAsync();
//     if (currentFile != null)
//     {
//         // Application now has read/write access to the picked file
//         json = await Windows.Storage.FileIO.ReadTextAsync(file);
//         OpenExistingExperiment(json);
//     }
//     else
//     {
//         BackFromLoadExperimentMenu();
//     }      
// #else
        
// #endif

// Todo: Use this for export/import function
// # if WINDOWS_UWP 
//     // Prevent updates to the remote version of the file until
//     // we finish making changes and call CompleteUpdatesAsync.
//     Windows.Storage.CachedFileManager.DeferUpdates(currentFile);
//     // write to file
//     await Windows.Storage.FileIO.WriteTextAsync(currentFile, JsonConvert.SerializeObject(experiment));
//     // Let Windows know that we're finished changing the file so
//     // the other app can update the remote version of the file.
//     // Completing updates may require Windows to ask for user input.
//     Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(currentFile);
//     if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
//     {
//         
//     }
//     else
//     {
//         
//     }
// #else

// Todo: Use this for export/import function
// # if WINDOWS_UWP
//     var savePicker = new Windows.Storage.Pickers.FileSavePicker();
//     savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
//     // Dropdown of file types the user can save the file as
//     savePicker.FileTypeChoices.Add("json", new List<string>() { ".json" });
//     // Default file name if the user does not type one in or select a file to replace
//     savePicker.SuggestedFileName = "NewExperiment";
//
//
//     currentFile = await savePicker.PickSaveFileAsync();
//     if (currentFile != null)
//     {
//         var experimentName = currentFile.Name[..^5];
//         OpenNewExperiment(experimentName);
//     }
//     else
//     {
//         BackFromCreateExperimentMenu();
//     }
// #else