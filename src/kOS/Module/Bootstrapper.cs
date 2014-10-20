﻿using System;
using System.IO;
using kOS.Persistence;
using UnityEngine;

namespace kOS.Module
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class Bootstrapper : MonoBehaviour
    {
        private readonly string legacyArchiveFolder = GameDatabase.Instance.PluginDataFolder + "/Plugins/PluginData/Archive/";
        private readonly string backupFolder = GameDatabase.Instance.PluginDataFolder + "/GameData/kOS/Backup";

        private bool backup = true;

        public void Start()
        {
            CheckForLegacyArchive();
            BuildLogger();
        }

        private void BuildLogger()
        {
            if (Safe.Utilities.Debug.Logger != null) return;
            Debug.Log("kOS: Init Logger");
            Safe.Utilities.Debug.Logger = new KSPLogger();
        }

        private void CheckForLegacyArchive()
        {
            if (!Directory.Exists(legacyArchiveFolder))
            {
                return;
            }

            if (Directory.Exists(Archive.ArchiveFolder))
            {
                return;
            }

            PopupDialog.SpawnPopupDialog(
                new MultiOptionDialog(
                    "The kOS v0.15 update has moved the archive folder to /Ships/Script/ and changed the file extension from *.txt to *.ks to be more inline with squad's current folder structure. Would you like us to attempt to migrate your existing scripts?",
                    () => backup = GUILayout.Toggle(backup, "Backup My scripts first"),
                    "kOS",
                    HighLogic.Skin, 
                    new DialogOption("Yes, Do it!", MigrateScripts, true),
                    new DialogOption("No, I'll do it myself", () => { }, true)
                    ),
                true,
                HighLogic.Skin
                );
        }


        private void MigrateScripts()
        {
            if (backup)
            {
                BackupScripts();
            }

            //TODO:Migrate
        }

        private void BackupScripts()
        {
            if (Directory.Exists(backupFolder))
            {
                Directory.Delete(backupFolder);
            }
            Directory.CreateDirectory(backupFolder);

            var files = Directory.GetFiles(legacyArchiveFolder);

            foreach (var fileName in files)
            {
                var fileInfo = new FileInfo(fileName);
                var newFileName = backupFolder + Path.DirectorySeparatorChar + fileInfo.Name;
                Safe.Utilities.Debug.Logger.Log("kOS: copying: " + fileName + " to: " + newFileName);
                File.Copy(fileName, newFileName);
            }
        }
    }
}