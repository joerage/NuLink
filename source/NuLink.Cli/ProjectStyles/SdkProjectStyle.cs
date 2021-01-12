using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Buildalyzer;
using NuGet.Configuration;

namespace NuLink.Cli.ProjectStyles
{
    public class SdkProjectStyle : ProjectStyle
    {
        public SdkProjectStyle(IUserInterface ui, ProjectAnalyzer project, XElement projextXml)
            : base(ui, project, projextXml)
        {
        }

        public override IEnumerable<PackageReferenceInfo> LoadPackageReferences(ProjectAnalyzer projectAnalyzer)
        {
            var packagesRootFolder = GetPackagesRootFolder();
            return GetPackages();
    
            IEnumerable<PackageReferenceInfo> GetPackages()
            {
                AnalyzerResults project = projectAnalyzer.Build();

                var packageReferenceInfo = new List<PackageReferenceInfo>();

                foreach(var result in project.Results)
                {
                    packageReferenceInfo.AddRange(result.PackageReferences.Select(packageReference =>
                    {
                        var packageId = packageReference.Key;
                        var version = packageReference.Value.First().Value;

                        var folder = GetPackageFolder(packageId, version);
                        return new PackageReferenceInfo(
                            packageId,
                            version,
                            rootFolderPath: folder,
                            libSubfolderPath: "lib");
                    }));
                }

                return packageReferenceInfo;
            }
    
            string GetPackageFolder(string packageId, string version)
            {
                var packageFolderPath = Path.Combine(
                    packagesRootFolder,
                    packageId.ToLower(),
                    version.ToLower());
    
                return packageFolderPath;
            }
        }

        private string GetPackagesRootFolder()
        {
            var settings = Settings.LoadDefaultSettings(null);
            return SettingsUtility.GetGlobalPackagesFolder(settings);
        }

        public static bool IsSdkStyleProject(XElement projectXml)
        {
            var sdkValue = projectXml.Attribute("Sdk")?.Value;
            return !String.IsNullOrEmpty(sdkValue);
        }
    }
}