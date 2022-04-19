using System;
using System.Collections.Generic;
using System.IO;
using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File;

namespace PM.Navisworks.SearchSets.Setup
{
    internal class Program
    {
        private static readonly DateTime ProjectStartedDate = new DateTime(year: 2022, month: 1, day: 26);

        private const string Guid = "73F464EE-D046-44F6-A2FF-AFC9A840A899";
        const string ProjectName = "Navisworks Search Sets";

        //Change This when Building from different PC
        private const string ProjectLocation = @"C:\Users\piotr.kulicki\source\repos\NavisworksSearchSets\PM.Navisworks.SearchSets";

        public static void Main(string[] args)
        {
            var folders = new Dictionary<string, string>
            {
                { "2018", $@"{ProjectLocation}\bin\x64\Release_2018\net452" },
                { "2020", $@"{ProjectLocation}\bin\x64\Release_2020\net47" },
                { "2021", $@"{ProjectLocation}\bin\x64\Release_2021\net47" },
                { "2022", $@"{ProjectLocation}\bin\x64\Release_2022\net47" }
            };
            
            AutoElements.DisableAutoKeyPath = true;
            var feature = new Feature(ProjectName, true, false);
            var directories = CreateDirectories(feature, folders);
            var dir = new Dir(feature, @"%AppData%/Autodesk/ApplicationPlugins/PM.Navisworks.SearchSets.bundle", 
                new File(feature, "./PackageContents.xml"),
                new Dir(feature, "Contents")
                {
                    Dirs = directories
                });

            var project = new Project(ProjectName, dir)
            {
                Name = ProjectName,
                Description = "Navisworks Search Sets Creator",
                OutFileName = "NavisworksSearchSets",
                OutDir = "output",
                Platform = Platform.x64,
                UI = WUI.WixUI_Minimal,
                Version = GetVersion(),
                InstallScope = InstallScope.perUser,
                MajorUpgrade = MajorUpgrade.Default,
                GUID = new Guid(Guid),
                LicenceFile = "./Resources/EULA.rtf",
                ControlPanelInfo =
                {
                    ProductIcon = "./Resources/PM.ico",
                },
                BannerImage = "./Resources/Banner.bmp",
                BackgroundImage = "./Resources/Main.bmp",
            };
            
            project.AddRegValues(new RegValue(RegistryHive.CurrentUser, $"Software\\PM Group\\{ProjectName}", "Version",
                GetVersion().ToString()));
            project.AddRegValues(new RegValue(RegistryHive.CurrentUser, $"Software\\PM Group\\{ProjectName}", "Guid",
                Guid));
            project.BuildMsi();
        }

        private static Dir[] CreateDirectories(Feature feature, Dictionary<string, string> folders)
        {
            var dirs = new List<Dir>();
            foreach (var folder in folders)
            {
                var dir = new Dir(folder.Key,
                    new Files(feature,
                        $@"{folder.Value}\*.*"));
                dirs.Add(dir);
            }

            return dirs.ToArray();
        }

        private static Version GetVersion()
        {
            const int majorVersion = 1;
            const int minorVersion = 4;
            var daysSinceProjectStarted = (int)((DateTime.UtcNow - ProjectStartedDate).TotalDays);
            var minutesSinceMidnight = (int)DateTime.UtcNow.TimeOfDay.TotalMinutes;
            var version = $"{majorVersion}.{minorVersion}.{daysSinceProjectStarted}.{minutesSinceMidnight}";
            return new Version(version);
        }
    }
}