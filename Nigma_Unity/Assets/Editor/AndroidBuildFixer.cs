using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Nigma.Editor
{
    public class AndroidBuildFixer : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log("[AndroidBuildFixer] Interceptando el proyecto de Gradle para arreglar conflictos de Kotlin...");
            
            // path is typically the root of the exported gradle project
            string launcherGradlePath = Path.Combine(path, "../launcher/build.gradle");
            string unityLibraryGradlePath = Path.Combine(path, "build.gradle");

            // Unity 2022+ path mapping: 
            // 'path' is usually Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary
            // So launcher is at path/../launcher/build.gradle

            if (File.Exists(unityLibraryGradlePath))
            {
                string content = File.ReadAllText(unityLibraryGradlePath);
                if (!content.Contains("resolutionStrategy"))
                {
                    string injection = @"
configurations.all {
    resolutionStrategy {
        // Force the kotlin stdlib to a single version to prevent duplicate class errors
        force 'org.jetbrains.kotlin:kotlin-stdlib:1.8.22'
        force 'org.jetbrains.kotlin:kotlin-stdlib-jdk7:1.8.22'
        force 'org.jetbrains.kotlin:kotlin-stdlib-jdk8:1.8.22'
    }
}
";
                    content += "\n" + injection;
                    File.WriteAllText(unityLibraryGradlePath, content);
                    Debug.Log("[AndroidBuildFixer] Resolución de Kotlin inyectada en unityLibrary/build.gradle");
                }
            }

            if (File.Exists(launcherGradlePath))
            {
                string content = File.ReadAllText(launcherGradlePath);
                if (!content.Contains("resolutionStrategy"))
                {
                    string injection = @"
configurations.all {
    resolutionStrategy {
        force 'org.jetbrains.kotlin:kotlin-stdlib:1.8.22'
        force 'org.jetbrains.kotlin:kotlin-stdlib-jdk7:1.8.22'
        force 'org.jetbrains.kotlin:kotlin-stdlib-jdk8:1.8.22'
    }
}
";
                    content += "\n" + injection;
                    File.WriteAllText(launcherGradlePath, content);
                    Debug.Log("[AndroidBuildFixer] Resolución de Kotlin inyectada en launcher/build.gradle");
                }
            }
        }
    }
}
