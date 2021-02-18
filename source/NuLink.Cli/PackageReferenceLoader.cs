using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using NuGet.Configuration;

namespace NuLink.Cli
{
    public class PackageReferenceLoader
    {
        private readonly IUserInterface _ui;

        public PackageReferenceLoader(IUserInterface ui)
        {
            _ui = ui;
        }

        public PackageReferenceInfo LoadPackageReference(IEnumerable<ProjectAnalyzer> projects, string packageId)
        {
            return LoadPackageReferences(projects, packageId).FirstOrDefault(package => package.PackageId == packageId);
        }

        public HashSet<PackageReferenceInfo> LoadPackageReferences(IEnumerable<ProjectAnalyzer> projects, string packageId)
        {
            var results = new HashSet<PackageReferenceInfo>();

            foreach (var project in projects)
            {
                _ui.ReportMedium(() => $"Searching package reference in: {Path.GetFileName(project.ProjectFile.Path)}");

                var projectPackages = LoadPackageReferences(project);
                results.UnionWith(projectPackages);
                var package = projectPackages.FirstOrDefault(package => package.PackageId == packageId);
                if (package != null)
                {
                    // We have found the package we needed, no need to continue searching.
                    _ui.ReportMedium(() => $"Package found in location: {package.LibFolderPath}");
                    break;
                }
            }

            return results;
        }

        private IEnumerable<PackageReferenceInfo> LoadPackageReferences(ProjectAnalyzer projectAnalyzer)
        {
            AnalyzerResults project = projectAnalyzer.Build();

            var packageReferenceInfo = new List<PackageReferenceInfo>();
            var packagesRootFolder = GetPackagesRootFolder();

            foreach (var result in project.Results)
            {
                packageReferenceInfo.AddRange(result.PackageReferences.Select(packageReference =>
                {
                    var packageId = packageReference.Key;
                    var version = packageReference.Value.First().Value;
                    var packageFolderPath = Path.Combine(packagesRootFolder, packageId.ToLower(), version.ToLower());

                    return new PackageReferenceInfo(
                        packageId,
                        version,
                        rootFolderPath: packageFolderPath,
                        libSubfolderPath: "lib");
                }));
            }

            return packageReferenceInfo;
        }

        private string GetPackagesRootFolder()
        {
            var settings = Settings.LoadDefaultSettings(null);
            return SettingsUtility.GetGlobalPackagesFolder(settings);
        }



    }
}