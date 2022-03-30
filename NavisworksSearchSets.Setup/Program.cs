using System;
using System.Diagnostics;
using System.IO;
using WixSharp;
using WixSharp.CommonTasks;

namespace NavisworksSearchSets.Setup
{
    internal class Program
    {
        private static readonly DateTime ProjectStartedDate = new DateTime(year: 2022, month: 1, day: 26);

        const string ProjectName = "Navisworks Search Sets";
        public static void Main(string[] args)
        {

            var project = new Project()
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
                LicenceFile = "EULA.rtf",
                GUID = new Guid(("73F464EE-D046-44F6-A2FF-AFC9A840A899")),
                ControlPanelInfo =
                {
                    Manufacturer = "PM Group",
                    ProductIcon = @"PM.ico"
                },
                BannerImage = "Banner.bmp",
                BackgroundImage = "Main.bmp"
            };
            
            project.AddDir(
                new Dir(GetInstallationDirectory(), new Files($@"C:\Users\piotr.kulicki\source\repos\NavisworksSearchSets\NavisworksSearchSets\bin\x64\Debug\*.*")));
            project.BuildMsi();
        }
        
        private static string GetInstallationDirectory()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Autodesk", "ApplicationPlugins", "NavisworksSearchSets.bundle");
        }
        
        private static Version GetVersion()
        {
            const int majorVersion = 1;
            const int minorVersion = 0;
            var daysSinceProjectStarted = (int)((DateTime.UtcNow - ProjectStartedDate).TotalDays);
            var minutesSinceMidnight = (int)DateTime.UtcNow.TimeOfDay.TotalMinutes;
            var version = $"{majorVersion}.{minorVersion}.{daysSinceProjectStarted}.{minutesSinceMidnight}";
            return new Version(version);
        }
    }
}