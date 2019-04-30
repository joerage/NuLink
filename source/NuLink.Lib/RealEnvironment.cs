using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using NuLink.Lib.Abstractions;

namespace NuLink.Lib
{
    public class RealEnvironment : IImmutableEnvironment, IEnvironmentEffect
    {
        public XElement LoadXml(string path)
        {
            throw new System.NotImplementedException();
        }

        public StreamReader OpenTextFile(string path)
        {
            return File.OpenText(path);
        }

        public string LoadTextFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> DownloadUrlAsText(string url)
        {
            throw new System.NotImplementedException();
        }

        public Stream CreateFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void CreateDirectory(string path)
        {
            throw new System.NotImplementedException();
        }

        public void SaveXml(XElement xml, string path)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTextFile(string contents, string ath)
        {
            throw new System.NotImplementedException();
        }

        public void MoveFile(string fromPath, string toPath)
        {
            throw new System.NotImplementedException();
        }

        public void MoveFolder(string fromPath, string toPath)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFolder(string path, bool recursiveForce)
        {
            throw new System.NotImplementedException();
        }

        public void CommitSource(ISourceRepository repo, CodeAuthor author, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}