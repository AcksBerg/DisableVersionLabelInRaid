using BepInEx;
using HarmonyLib;
using EFT.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin("com.acksberg.disableversionlabelinraid", "AcksBerg-DisableVersionLabelInRaid", "1.0.0")]
public sealed class DisableVersionLabel : BaseUnityPlugin
{
    private const string TargetName = "AlphaLabel";
    private const string MenuSceneName = "MenuUIScene";

    private static GameObject _alphaLabel;
    private static bool _menuSceneLoaded;

    private void Awake()
    {
        new Harmony("acks.disableversionlabel").PatchAll();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        Logger.LogInfo("DisableVersionLabel loaded");
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (scene.name == MenuSceneName)
        {
            _menuSceneLoaded = true;
            Apply();
        }
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == MenuSceneName)
        {
            _menuSceneLoaded = false;
            Apply();
        }
    }

    private static void Apply()
    {
        if (_alphaLabel == null)
            return;

        if (_alphaLabel.activeSelf != _menuSceneLoaded)
            _alphaLabel.SetActive(_menuSceneLoaded);
    }

    [HarmonyPatch(typeof(LocalizedText), "OnEnable")]
    private static class Patch
    {
        private static void Postfix(LocalizedText __instance)
        {
            if (__instance?.gameObject?.name != TargetName)
                return;

            if (_alphaLabel == null)
                _alphaLabel = __instance.gameObject;

            Apply();
        }
    }
}
