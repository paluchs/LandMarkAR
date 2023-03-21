# LandMarkAR

## Get started
### Necessary Hardware/Software
You will need a Microsoft HoloLens 2 and a Windows Computer.

On the Windows Computer, install:
- Unity, with Editor Version 2021.3.4f1.
- Microsoft Visual Studio
- Mixed Reality Feature Tool 
  - (https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool)

Optional:
- Jetbrains Rider (If you prefer JetBrains IDEs over Visual Studio for coding)

### Opening the Project in Unity
I highly recommend to look at the MRTK examples (https://github.com/microsoft/MixedRealityToolkit-Unity) and the examples Hub before working on the project.
However, before opening the project in Unity you will have to add the necessary Packages:
- With the Mixed Reality Feature Tool:
  - Spatial Anchors:
    - Azure Spatial Anchors SDK Core (Version 2.13.0)
    - Azure Spatial Anchors SDK for Windows (Version 2.13.0)
  - Mixed Reality Toolkit:
    - Mixed Reality Toolkit Extensions (Version 2.8.2)
    - Mixed Reality Toolkit Foundation (Version 2.8.2)
    - Mixed Reality Toolkit Standard Assets (Version 2.8.2)
    - Mixed Reality Toolkit Tools (Version 2.8.2)
  - Platform Support:
    - Mixed Reality OpenXR Plugin (Version 1.5.0)
  - Spatial Audio:
    - Microsoft Spatializer (Version 2.0.37)
  - World Locking Tools:
    - WLT-ASA (Version 1.5.9)
    - WLT Core (Version 1.5.9)

The MRTK is currently in the process of being updated to Version 3. At some point it might be necesary or helpul to upgrade the project or integrated additional features of Version 3.

Moreover, you will need to add glTFast: https://github.com/prefrontalcortex/UnityGLTF
To prevent glTFast to conflict with the MRTKs importer you will have to disable the mrtk serializer: https://localjoost.github.io/Workaround-for-MRKT-27x-glTF-import-bug/

Finally, you will need to install Newtonsoft Json into the project via the unity package manager.

### Running the project and deploying to the HoloLens
Once you have opened the project you can run it by using the play button in Unity.
To deploy it to the HoloLens you will need to create a C++ build and run it from Visual Studio.
Here is a Guide to follow: 
- https://learn.microsoft.com/de-de/training/modules/learn-mrtk-tutorials/1-7-exercise-hand-interaction-with-objectmanipulator?ns-enrollment-type=learningpath&ns-enrollment-id=learn.azure.beginner-hololens-2-tutorials

Note that the MRTK is constantly changing and being developed, it therefore makes sense to go through the documentation yourself and find the necessary resources:
  - e.g. here: https://learn.microsoft.com/de-de/windows/mixed-reality/develop/unity/unity-development-overview?tabs=arr%2CD365%2Chl2

### Software Architecture and Contributing
The project follows an MVC architecture loosely, where the Unity Editor is the View and C# scripts manage the models and controllers.
Always attach a controller script to a View (i.e. GameObject in Unity)

For all relevant scripts there is generally a base class that can be extended should it be necessary.

#### Controllers 
The controllers manage different aspects of the application.  

*For example:*  
The AssetController class that controls all assets (e.g. has a function to save an asset as an ASA)
is an Abstract class that is then extended for example to the Asset3DController class used to place primitives.  
Extend these base classes to add new functionality.

#### Abstract Factory
To instantiate new objects you will need to create an Abstract Factory (See abstract factory pattern)

#### UI Flow
The UI folder contains all the necessary scripts to control the menu flow. To create a new menu for an Asset
you can extend the abstract AssetMenu class.

#### Resources
The resources folder contains all the necessary resources that are loaded at runtime.  
*For example:*
To add new 3D primitives to the project, all you have to do is to add .fbx files to the Asset3DMeshes folder in the Resources folder.
The AssetManager class can then be used to get the Assets at runtime.
Since all Managers in this project are static classes you can call them anywhere in the project without instantiating them.

To understand the relationships between the C# scripts and the Unity GameObjects it is best to open the Experiment Scene in Unity and look at the scripts that are attached
to each game object.


## Q/A

### I'm not familiar with Unity or the MRTK, where can I get started?
If you're not familiar with Unity and/or the MRTK for Unity and development for the HoloLens but have
some experience with software engineering, you can use the following tutorials as an overview:

#### For Unity:
On Unity learn you can find lots of great courses: https://learn.unity.com/
To get started with this project I'd recommend to at least complete the:
- Unity Essentials Path
- Junior Programmer Path

#### For the MRTK:
This course is a great way to start:
- https://learn.microsoft.com/en-us/training/modules/learn-mrtk-tutorials/1-1-introduction

#### HoloLens Development with Unity and the MRTK
- https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/unity-development-overview?tabs=arr%2CD365%2Chl2

### Is there guidance on how I should test the app?
Yes, see here:
https://learn.microsoft.com/en-us/windows/mixed-reality/develop/advanced-concepts/testing-your-app-on-hololens

### I have another question
No problem, send me an email: patrick.lu@bluewin.ch
