using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceHelper;

namespace ArchServer
{
    public interface IFileHelper
    {
        DirectoryInfo GetDirectoryWithValidation(string path);
        List<byte[]> SplitFileToListOfByteArray(string path);
        void Archivation(DirectoryInfo resourceDirectory, DirectoryInfo wrongDirectory);
        void CreateArchive(List<FileInfo> files, string path);
        void DeleteFile(FileInfo file);
        void AppendAllBytes(string path, byte[] bytes);
        byte[] CombineGroupOfBytes(IGrouping<Guid, SequanceMessage> list);
    }
}
