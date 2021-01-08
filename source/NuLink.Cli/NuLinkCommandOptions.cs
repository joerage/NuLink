using System;

namespace NuLink.Cli
{
    public class NuLinkCommandOptions
    {
        public NuLinkCommandOptions(
            string consumerProjectPath, 
            string packageId = null, 
            bool dryRun = false, 
            bool bareUI = false,
            string localProjectPath = null,
            string consumerObjPath = null)
        {
            ConsumerProjectPath = consumerProjectPath;
            PackageId = packageId;
            ConsumerObjPath = consumerObjPath;
            DryRun = dryRun;
            BareUI = bareUI;
            LocalProjectOutputPath = localProjectPath;
            ProjectIsSolution = ConsumerProjectPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase);
        }

        public string ConsumerProjectPath { get; }
        public bool ProjectIsSolution { get; }
        public string PackageId { get; }
        public string LocalProjectOutputPath { get; }
        public string ConsumerObjPath { get; }
        public bool DryRun { get; }
        public bool BareUI { get; }
    }
}