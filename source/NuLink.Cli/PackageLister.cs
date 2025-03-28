using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Buildalyzer;
using NuGet.Common;
using NuGet.ProjectModel;

namespace NuLink.Cli
{
    public class PackageLister
    {
        private readonly IUserInterface _ui;

        public PackageLister(IUserInterface ui)
        {
            _ui = ui;
        }

        public List<PackageReference> Analyze(IProjectAnalyzer projectAnalyzer)
        {
            try
            {
                // Get direct references from the project file.
                var project = projectAnalyzer.Build();
                var directReferences = project.Results.First().PackageReferences;
                
                // Get the assets file path and ensure it exists.
                string assetsFile = GetAssetsFilePath(GetProjectOutputPath(projectAnalyzer));
                
                if (!File.Exists(assetsFile))
                {
                    _ui.ReportMedium(() => $"Asset file not found at '{assetsFile}'. Attempting to restore the project...");
                    if (RunDotNetRestore(projectAnalyzer.ProjectFile.Path) != 0)
                    {
                        _ui.ReportWarning(() => $"Failed to restore project dependencies. Will try to continue with existing files.");
                    }
                }
                
                if (!File.Exists(assetsFile))
                {
                    _ui.ReportWarning(() => $"Asset file not found at '{assetsFile}' even after restore attempt.");
                    return [];
                }
                
                var lockFile = LockFileUtilities.GetLockFile(assetsFile, NullLogger.Instance);
                if (lockFile == null)
                {
                    _ui.ReportWarning(() => $"Lock file found at '{assetsFile}' but could not be parsed.");
                    return [];
                }
                
                return [.. lockFile.Targets
                            .SelectMany(targetFramework => 
                                targetFramework.Libraries
                                            .Where(library => library.Type == "package" && directReferences.ContainsKey(library.Name))
                                            .Select(library => new PackageReference
                                            {
                                                PackageId = library.Name,
                                                Version = library.Version.ToString(),
                                                TargetFramework = targetFramework.TargetFramework.ToString()
                                            }))];
            }
            catch (Exception ex)
            {
                _ui.ReportWarning(() => $"Error reading package references: {ex.Message}");
                return [];
            }
        }

        private static string GetProjectOutputPath(IProjectAnalyzer projectAnalyzer)
        {
            var buildResult = projectAnalyzer.Build();
            var projectDirectory = Path.GetDirectoryName(projectAnalyzer.ProjectFile.Path);
            
            return buildResult.Results
                .Select(result => result.Properties.TryGetValue("BaseIntermediateOutputPath", out var path) ? path : null)
                .Where(path => !string.IsNullOrEmpty(path))
                .Select(path => Path.IsPathRooted(path) 
                    ? path 
                    : Path.GetFullPath(Path.Combine(projectDirectory, path)))
                .FirstOrDefault()
                // Fallback to conventional output path if no property found.
                ?? Path.Combine(projectDirectory, "bin", "Debug");
        }

        private int RunDotNetRestore(string projectPath)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"restore \"{projectPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                _ui.ReportError(() => $"Failed to run 'dotnet restore': {ex.Message}");
                return -1;
            }
        }

        string GetAssetsFilePath(string outputPath)
        {
            // Get the assets file path
            return Path.Combine(outputPath, "project.assets.json");
        }
    }
    
    public class PackageReference
    {
        public string PackageId { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }
    }
}