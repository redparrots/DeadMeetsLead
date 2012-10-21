using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ionic.Zip;

namespace Common
{

    public interface IFileSystem
    {
        bool FileExists(string path);
        void DeleteFile(string path);
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        string[] DirectoryGetFiles(string path, string searchPattern);
        ITransactionalFileSystem Transaction();
    }

    public interface ITransactionalFileSystem : IFileSystem
    {
        void Commit();
    }

    public class FileSystem : IFileSystem
    {
        static FileSystem()
        {
            instance = new FileSystem();
        }

        private static readonly IFileSystem instance;
        public static IFileSystem Instance { get { return instance; } }

        IPathHandler GetHandler(IPathHandler handler, string subPath, out string outSubPath)
        {
            outSubPath = null;
            var ss = subPath.Split(new char[] { '(' }, 2);
            if (ss.Length == 1)
            {
                if (handler.FileExists(subPath))
                {
                    outSubPath = subPath;
                    return handler;
                }
                else
                    return null;
            }
            else
            {
                var ending = ss[1].Split(')')[0];
                var nextSubPath = ss[1].Substring(ending.Length + 2);
                // Otherwise we have a zip file in the path (i.e. C:/MyStuff|.zip/something)
                // The rule then is that if C:/MyStuff/something exists, we go for that
                var rawPath = ss[0] + "/" + nextSubPath;
                var h = GetHandler(handler, rawPath, out outSubPath);
                if (h != null) return h;
                else
                {
                    // Otherwise we go for the zip file
                    return GetHandler(handler.OpenZipFile(ss[0] + ending), nextSubPath, out outSubPath);
                }
            }
        }

        interface IPathHandler
        {
            bool FileExists(string subPath);
            ZipPathHandler OpenZipFile(string subPath);
            void DeleteFile(string path);
            Stream OpenRead(string path);
            Stream OpenWrite(string path);
            string[] DirectoryGetFiles(string path, string searchPattern);
        }
        class PhysicalPathHandler : IPathHandler
        {
            public bool FileExists(string subPath)
            {
                if (subPath.EndsWith("/*")) 
                    return Directory.Exists(subPath.Substring(0, subPath.Length - 2));
                return File.Exists(subPath) || Directory.Exists(subPath);
            }

            public ZipPathHandler OpenZipFile(string subPath)
            {
                return new ZipPathHandler(subPath);
            }

            public void DeleteFile(string path)
            {
                File.Delete(path);
            }

            public Stream OpenRead(string path)
            {
                if (!File.Exists(path)) return null;
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            public Stream OpenWrite(string path)
            {
                return File.OpenWrite(path);
            }


            public string[] DirectoryGetFiles(string path, string searchPattern)
            {
                return Directory.GetFiles(path, searchPattern);
            }


        }
        class ZipPathHandler : IPathHandler
        {
            private ZipFile zipFile;
            public ZipPathHandler(string path)
            {
                this.zipFile = new ZipFile(path);
            }
            private ZipPathHandler(Stream stream)
            {
                this.zipFile = ZipFile.Read(stream);
            }

            public bool FileExists(string subPath)
            {
                return zipFile.ContainsEntry(subPath);
            }

            public ZipPathHandler OpenZipFile(string subPath)
            {
                //var stream = zipFile[subPath].OpenReader();
                //return new ZipPathHandler(stream);
                throw new NotImplementedException();
            }

            public void DeleteFile(string path)
            {
                throw new NotImplementedException();
            }

            public Stream OpenRead(string path)
            {
                return zipFile[path].OpenReader();
            }

            public Stream OpenWrite(string path)
            {
                var t = Path.GetTempFileName();
                var s = File.OpenWrite(t);
                if (zipFile.ContainsEntry(path)) zipFile.RemoveEntry(path);
                zipFile.AddEntry(path, name => File.OpenRead(t), (name, st) => { st.Close(); File.Delete(t); });
                return s;
            }

            public string[] DirectoryGetFiles(string path, string searchPattern)
            {
                return zipFile.SelectEntries(searchPattern, path).Select(ze => ze.FileName).ToArray();
            }
        }


        public bool FileExists(string path)
        {
            path = path.Replace('\\', '/');
            string subPath;
            var handler = GetHandler(new PhysicalPathHandler(), path, out subPath);
            if (handler == null) return false;
            return handler.FileExists(subPath);
        }

        public void DeleteFile(string path)
        {
            path = path.Replace('\\', '/');
            string subPath;
            var handler = GetHandler(new PhysicalPathHandler(), path, out subPath);
            handler.DeleteFile(subPath);
        }

        public Stream OpenRead(string path)
        {
            path = path.Replace('\\', '/');
            string subPath;
            var handler = GetHandler(new PhysicalPathHandler(), path, out subPath);
            return handler.OpenRead(subPath);
        }

        public Stream OpenWrite(string path)
        {
            path = path.Replace('\\', '/');
            string subPath;
            var handler = GetHandler(new PhysicalPathHandler(), path, out subPath);
            return handler.OpenWrite(subPath);
        }

        public string[] DirectoryGetFiles(string path, string searchPattern)
        {
            path = path.Replace('\\', '/');
            string subPath;
            var handler = GetHandler(new PhysicalPathHandler(), path, out subPath);
            return handler.DirectoryGetFiles(subPath, searchPattern);
        }

        public ITransactionalFileSystem Transaction()
        {
            return new TransactionalFileSystem(this);
        }
        class TransactionalFileSystem : ITransactionalFileSystem
        {
            private FileSystem fileSystem;
            public TransactionalFileSystem(FileSystem fileSystem)
            {
                this.fileSystem = fileSystem;
            }
            public void Commit()
            {
                foreach (var tuple in filesToCommit)
                {
                    if (File.Exists(tuple.Item2))
                        File.Delete(tuple.Item2);
                    File.Move(tuple.Item1, tuple.Item2);
                }
                filesToCommit.Clear();
            }

            public bool FileExists(string path)
            {
                return fileSystem.FileExists(path);
            }

            public void DeleteFile(string path)
            {
                fileSystem.DeleteFile(path);
            }

            public Stream OpenRead(string path)
            {
                return fileSystem.OpenRead(path);
            }

            public Stream OpenWrite(string path)
            {
                var tempFile = Path.GetTempFileName();
                filesToCommit.Add(new System.Tuple<string, string>(tempFile, path));
                return File.OpenWrite(tempFile);
            }

            public string[] DirectoryGetFiles(string path, string searchPattern)
            {
                return fileSystem.DirectoryGetFiles(path, searchPattern);
            }

            public ITransactionalFileSystem Transaction()
            {
                throw new NotImplementedException();
            }

            readonly List<System.Tuple<String, String>> filesToCommit = new List<System.Tuple<string, string>>();

        }

    }
}