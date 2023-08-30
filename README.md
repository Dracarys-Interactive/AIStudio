![Alt text](Images/dilogo2.png)

*Dracarys Interactive supplies Unity-based tools and templates for creating quality interactive 2D, 3D, and XR applications.*
# Welcome to *AI Studio*!
* [Introduction](#introduction)
* [Using *AI Studio*](#using-ai-studio)
* [*AI Studio* Dependencies](#ai-studio-dependencies)
* [Running the Simple Scenes](#running-the-simple-scenes)
* [Running the City Scenes](#running-the-city-scenes)
* [Running Avatar Wars](#running-avatar-wars)
* [Support](#support)

## Introduction
*AI Studio* provides a flexible [open-source framework](https://github.com/Skylands-Research-Institute/AI-Studio) for integrating [Unity](https://unity.com/) games and applications with AI systems that enable real-time, speech-driven dialogue and actions. The core framework may be integrated with your choice of speech and text recognition and generation models and APIs.

This framework is a byproduct of a case study exploring a step-by-step integration of [OpenAI](https://openai.com/)'s [GPT-3.5](https://openai.com/chatgpt) model into Unity 3D for the purpose of enhancing non-playing characters (NPCs) in video games and interactive applications. By leveraging GPT-3.5's natural language processing capabilities, we aimed to create NPCs capable of dynamic real-time interactions with players and each other! Our methodology involved integrating GPT-3.5, implementing speech-driven dialogues, and incorporating humanoid avatars with lip-syncing and emotive animations.

Key features of *AI Studio* include:
* Core action-driven architecture implementing multiple character dialogues which feeds asynchronous events into a central queue for processing by the main Unity thread
* Basic *Scriptable Object* framework for representing characters and character dialogues and providing AI model prompt engineering inputs
* Abstracted interfaces to key implementation boundaries including speech recognition and synthesis, natural language dialogue generation, and dialogue character animation and movement
* Feature extensions including streaming and non-streaming audio, continuous and discrete player voice recognition, conditional dialogue triggering, player synthetic voice option, programmatic chat injection, and basic non-player character movement
* Example scenes demonstrating integration with Microsoft Cognitive Services Speech SDK, OpenAI's chat completion models (e.g. GPT-3.5), Unity Multipurpose Avatars (UMA), SALSA LipSync Suite, and Ready Player Me avatars
* [YouTube videos](https://www.youtube.com/playlist?list=PLqvEk6ZnckUDIadG7NqBthi1HLx0I-1yL) documenting installation and example scenes

## Using *AI Studio*
*AI Studio* has several dependencies that require licensing and importing before all the sample scenes can be compiled and run. The target platform for running sample scenes is a Windows 10 or later PC running Unity 2020.3 or later. There are two sets of sample scenes included in *AI Studio*. “Simple” scenes demonstrate the core architecture and “City” scenes with additional dependencies add a game world and humanoid avatars. One “City” scene, *Avatar Wars*, adds an additional dependency on [Ready Player Me](https://readyplayer.me/).

## *AI Studio* Dependencies
The *AI Studio* package itself includes no third-party content except for a couple of Unity Starter Asset scripts. However, to run the sample scenes in *AI Studio*, several third-party licenses and packages must be acquired and installed by the user. These packages are not included in the *AI Studio* package and until installed the sample scenes will contain objects with missing component scripts. Once the required packages are installed, these missing component script errors will be resolved. Instructions for acquiring and installing these packages are included in this README and are also documented in a set of [YouTube videos](https://www.youtube.com/playlist?list=PLqvEk6ZnckUDIadG7NqBthi1HLx0I-1yL). The complete set of dependencies is shown below.

To run the “Simple” sample scenes, the following dependencies must be satisfied:
* An OpenAI API key.
* Integration for the OpenAI API in Unity. This is an open-source Unity package hosted in GitHub governed by the MIT license.
* A subscription key for the Microsoft Azure Speech Service.
* The Microsoft Cognitive Services Speech SDK for Unity package.

To run the “City” sample scenes except for “Avatar Wars”, these additional packages must be installed:
* The Unity Cinemachine package from the Unity Registry.
* The UMA 2 Unity Multipurpose Avatar package from the Unity Asset Store.
* The CITY package from the Unity Asset Store.
* The SALSA Lip Sync Suite from Unity Asset Store.
* The SALSA Lip Sync Suite OneClick UMA DCS package from the Crazy Minnow Studio website.
* A package of several Mixamo animations.

To run the “City” sample scene “Avatar Wars”, these additional dependencies must be satisfied:
* The Ready Player Me Unity SDK package.

The next section describes in more detail how to satisfy these dependencies and run the *AI Studio* sample scenes.

## Running the Simple Scenes
To run the Simple scenes which demonstrate the core architecture, the following licenses must be obtained, and accompanying Unity packages must be installed.

### *AI Studio*
* Download and install the SRI *AI Studio* package from the [Unity Asset Store](https://assetstore.unity.com/) or via [GitHub](https://github.com/Dracarys-Interactive/AIStudio.git) or [from here](https://1drv.ms/u/s!ArFvxkQ02ZoSidNi7o1DlpkHlpA2Kw?e=ezwyPd).
* You may get *missing Prefab* errors due to the dependencies *.../Prefabs/Environment.pref* has on the CITY package, but this can be ignored when running the Simple scenes. 

### OpenAI
* An [OpenAI](https://openai.com/) account and API key are required to run all sample scenes. Once you have obtained the key, you must create a folder *.openai* under your home folder with a file *auth.json* containing the following:

> { private_api_key : "YOUR_KEY_HERE" }

* Alternatively, you can modify the OpenAI Unity API authentication type from “Local File” to “String” and enter your OpenAI API key directly via the Unity Inspector.
* Add and import the [OpenAI Unity API package](https://github.com/hexthedev/OpenAi-Api-Unity) from GitHub.
* Add the *Scripting Define Symbol* **USE_COM_OPENAI_API_UNITY** to the *Player Project Settings* under *Script Compilation*.

### Microsoft Speech Services
* A subscription key for the [Microsoft Azure Speech Service](https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/overview#try-the-speech-service-for-free). 
* Once you have obtained the key, you must set the environment variables COGNITIVE_SERVICE_KEY and COGNITIVE_SERVICE_REGION.
* Alternatively, you can enter the key and region fields directly via the Inspector.
* Download and install the [Speech SDK for Unity Package](https://aka.ms/csspeech/unitypackage).
* Add the *Scripting Define Symbol* **USE_MICROSOFT_COGNITIVESERVICES_SPEECH** to the *Player Project Settings* under *Script Compilation*.

There are three Simple scenes that you can run under *Skylands Research Institure/AI Studio/Scenes*:
* Simple 2 NPC - This scene demonstrates dialogue between two NPCs.
* Simple 3P Player and 1 NPC - One third-person player and one non-player.
* Simple 3P Player and 1 NPC - One third-person player and two non-players.

This [YouTube video](https://youtu.be/_BZMSQyDK10) demonstrates how to create a Unity project from scratch that can run the Simple example scenes.

## Running the City Scenes
To run the City scenes which include a city game world and avatar characters, the following licenses must be obtained, and accompanying Unity packages must be installed.

### Cinemachine
* The Cinemachine package may be imported via the Unity Package Manager through the Unity Registry.
* Add the *Scripting Define Symbol* **USE_CINEMACHINE** to the *Player Project Settings* under *Script Compilation*.

### Unity Starter Assets
**NOTE! Unity Starter Assets First and Third-person package scripts are included in the AI Studio package so the following steps to import and modify these packages are for information only. It should not be necessary to import these packages. However you will need to add the *Scripting Define Symbol* STARTER_ASSETS_PACKAGES_CHECKED to the *Player Project Settings* under *Script Compilation*.**

### Starter Assets – Third Person Character Controller
* The Unity Asset Store [Starter Assets - Third Person Character Controller](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-urp-196526), a free asset, must be imported and a minor C# script change must be applied to make the controller work with UMA
* Accept the import of any dependencies.
* Changes to StarterAssets\Scripts\ThirdPersonController.cs
    * Expose _animator field (line 104): 
        > [SerializeField] private Animator _animator;
    * Use attached Animator if set (line 139 and 158):
        > if (!(_hasAnimator = _animator)) _hasAnimator = TryGetComponent(out _animator);
    * Make method OnFootstep public (line 372)
    * Make method OnLand public (line 384)
### Starter Assets – First Person Character Controller
* The Unity Asset Store [Starter Assets - First Person Character Controller](https://assetstore.unity.com/packages/essentials/starter-assets-first-person-character-controller-urp-196525), a free asset, must be imported.
### UMA 2 - Unity Multipurpose Avatar
* The [Unity Asset Store UMA 2 - Unity Multipurpose Avatar](https://assetstore.unity.com/packages/3d/characters/uma-2-unity-multipurpose-avatar-35611), a free asset, must be imported.
### CITY package
* The [Unity Asset Store CITY package](https://assetstore.unity.com/packages/3d/environments/urban/city-package-107224), a free asset, must be imported.
### SALSA LipSync Suite
* The [Unity Asset Store SALSA LipSync Suite](https://assetstore.unity.com/packages/tools/animation/salsa-lipsync-suite-148442), **a paid asset**, must be imported.
* **NOTE!** You will also need to download and install the *OneClick UMA DCS* add-on package [from here](https://crazyminnowstudio.com/unity-3d/lip-sync-salsa/downloads/).
* SALSA will *automatically* add the *Scripting Define Symbol* **CMS_SALSA** to the *Player Project Settings* under *Script Compilation*.
### Mixamo Animations
* Import a set of Mixamo animations [from here](https://1drv.ms/u/s!ArFvxkQ02ZoSjMgqJUb-9k_dxiaMKw?e=9tGPkr).

This [YouTube video](https://youtu.be/IrRu-i-r3As) demonstrates how to extend the Unity project configured to run the Simple scenes to run the City scenes with the exception of *Avatar Wars* which will be covered in the next section.

## Running *Avatar Wars*

The scene *City Scene Avatar Wars 2 NPC* requires the installation of *Ready Player Me*.

### Ready Player Me package
* Follow [these instructions](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart) to setup RPM and install the software into Unity. Make sure to also import all the loader samples.
* Import an RPM avatar [from here](https://1drv.ms/u/s!ArFvxkQ02ZoSh_MuEU9udKCNhTwmyg?e=VeciEY).

This [YouTube video](https://youtu.be/Y9N1owUCRYs) demonstrates how to extend the Unity project configured to run the City scenes to run  *Avatar Wars*.

## Support
If you have questions or issues with *AI Studio* please visit the [Dracarys Interactive website](http://dracarysinteractive.com) and use the *Contact* page to get in touch with us.