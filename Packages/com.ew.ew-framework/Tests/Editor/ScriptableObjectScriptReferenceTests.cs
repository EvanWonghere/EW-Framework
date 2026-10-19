using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EW_Framework.Tests.Editor
{
    /// <summary>
    /// A ScriptableObject asset or a component only gets a valid m_Script when Unity finds a MonoScript for its class.
    /// A .cs file yields one MonoScript; when several classes share a file only one of them gets it (in Unity 6000.4 the
    /// first one), so every concrete SO / MonoBehaviour type must live in its own file.
    /// </summary>
    public class ScriptableObjectScriptReferenceTests
    {
        private const string PackageRoot = "Packages/com.ew.ew-framework";

        private static bool IsConcreteFrameworkType(Type t) =>
            t.Namespace != null && t.Namespace.StartsWith("EW_Framework.")
            && !t.Namespace.Contains(".Tests")
            && !t.IsAbstract && !t.IsGenericTypeDefinition;

        private static IEnumerable<Type> FrameworkScriptableObjectTypes() =>
            TypeCache.GetTypesDerivedFrom<ScriptableObject>()
                .Where(t => IsConcreteFrameworkType(t)
                            && !typeof(UnityEditor.Editor).IsAssignableFrom(t)
                            && !typeof(EditorWindow).IsAssignableFrom(t))
                .OrderBy(t => t.FullName);

        [Test]
        public void EveryConcreteScriptableObject_HasItsOwnMonoScript()
        {
            var missing = new List<string>();
            foreach (Type type in FrameworkScriptableObjectTypes())
            {
                var instance = ScriptableObject.CreateInstance(type);
                try
                {
                    MonoScript script = MonoScript.FromScriptableObject(instance);
                    if (script == null || script.GetClass() != type) missing.Add(type.FullName);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(instance);
                }
            }

            Assert.IsEmpty(missing, "No MonoScript resolves to these types:\n" + string.Join("\n", missing));
        }

        [Test]
        public void EveryConcreteMonoBehaviour_HasItsOwnMonoScript()
        {
            var scripted = new HashSet<Type>(MonoImporter.GetAllRuntimeMonoScripts()
                .Select(script => script.GetClass())
                .Where(t => t != null));

            List<string> missing = TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
                .Where(IsConcreteFrameworkType)
                .Where(t => !scripted.Contains(t))
                .Select(t => t.FullName)
                .OrderBy(n => n)
                .ToList();

            Assert.IsEmpty(missing, "These components cannot be added in the Inspector:\n" + string.Join("\n", missing));
        }

        [Test]
        public void SampleAssets_ReferenceTheScriptOfTheirOwnClass()
        {
            string samplesDir = Path.Combine(Path.GetFullPath(PackageRoot), "Samples~");
            Assert.That(Directory.Exists(samplesDir), "Samples~ is missing from the package");
            string[] assetFiles = Directory.GetFiles(samplesDir, "*.asset", SearchOption.AllDirectories);
            Assert.IsNotEmpty(assetFiles);

            var guidPattern = new Regex(@"m_Script: \{fileID: 11500000, guid: ([0-9a-f]{32}), type: 3\}");
            var classPattern = new Regex(@"m_EditorClassIdentifier: .*[.:]([A-Za-z_][A-Za-z0-9_]*)\s*$", RegexOptions.Multiline);
            var problems = new List<string>();

            foreach (string file in assetFiles)
            {
                string text = File.ReadAllText(file);
                if (!text.Contains("m_Script:")) continue;

                string name = Path.GetFileName(file);
                string expectedClass = classPattern.Match(text).Groups[1].Value;
                Match guid = guidPattern.Match(text);
                if (!guid.Success)
                {
                    problems.Add($"{name}: m_Script is not a script reference (expected {expectedClass})");
                    continue;
                }

                string resolved = ResolveScriptClass(guid.Groups[1].Value, samplesDir);
                if (resolved != expectedClass)
                    problems.Add($"{name}: script resolves to {resolved ?? "<none>"}, expected {expectedClass}");
            }

            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        /// <summary>
        /// Package scripts are imported, so ask Unity which class their MonoScript holds. Sample scripts under Samples~
        /// are not imported until the sample is; for those, require the file to be named after the class.
        /// </summary>
        private static string ResolveScriptClass(string guid, string samplesDir)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
            if (script != null) return script.GetClass()?.Name;

            foreach (string meta in Directory.GetFiles(samplesDir, "*.cs.meta", SearchOption.AllDirectories))
            {
                if (File.ReadAllText(meta).Contains("guid: " + guid))
                    return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(meta));
            }
            return null;
        }
    }
}
