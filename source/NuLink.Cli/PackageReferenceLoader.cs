using System;
using System.Collections.Generic;
using System.IO;
using Buildalyzer;
using NuLink.Cli.ProjectStyles;

namespace NuLink.Cli
{
    public class PackageReferenceLoader
    {
        private readonly IUserInterface _ui;
        private readonly string _consumerObjPath;

        public PackageReferenceLoader(IUserInterface ui, string consumerObjPath)
        {
            _ui = ui;
            _consumerObjPath = consumerObjPath;
        }

        public HashSet<PackageReferenceInfo> LoadPackageReferences(IEnumerable<ProjectAnalyzer> projects)
        {
            var results = new HashSet<PackageReferenceInfo>();

            foreach (var project in projects)
            {
                _ui.ReportMedium(() => $"Checking package references: {Path.GetFileName(project.ProjectFile.Path)}");

                var projectStyle = ProjectStyle.Create(_ui, project, _consumerObjPath);
                var projectPackages = projectStyle.LoadPackageReferences();
                results.UnionWith(projectPackages);
            }

            return results;
        }
        
    }
}