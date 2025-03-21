//
// CameraShakeMenuItems.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using UnityEngine;
using UnityEditor;
using Thinksquirrel.CameraShakeInternal;

//! \cond PRIVATE
namespace Thinksquirrel.CameraShakeEditor
{
    static class CameraShakeMenuItems
    {
        // This variable controls the location of Camera Shake menu commands.
        // Do not include the "Camera Shake" folder in the name.
        // No trailing slashes allowed!
        public const string menuToolsLocation = "Tools/Thinksquirrel/Utilities";
        // This variable controls the location of Camera Shake menu windows.
        // Do not include the "Camera Shake" folder in the name.
        // No trailing slashes allowed!
        public const string menuWindowLocation = "Window/Utilities";

        [MenuItem(menuWindowLocation + "/Camera Shake/Get Update Notifications", false, 2000)]
        [MenuItem(menuToolsLocation + "/Camera Shake/Get Update Notifications", false, 2000)]
        internal static void RegisterCameraShake()
        {
            Application.OpenURL("https://www.thinksquirrel.com/#subscribe");
        }

        [MenuItem(menuWindowLocation + "/Camera Shake/Reference Manual", false, 1200)]
        [MenuItem(menuToolsLocation + "/Camera Shake/Reference Manual", false, 1200)]
        internal static void HelpWindow()
        {
            Application.OpenURL(CameraShakePreferences.ReferenceManualUrl());
        }

        [MenuItem(menuWindowLocation + "/Camera Shake/Support Forum", false, 1201)]
        [MenuItem(menuToolsLocation + "/Camera Shake/Support Forum", false, 1201)]
        internal static void SupportForumWindow()
        {
            Application.OpenURL(VersionInfo.SupportForumUrl());
        }
    }
}
//! \endcond

