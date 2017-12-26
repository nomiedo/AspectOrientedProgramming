using System;

namespace ArchServer.DAL
{
    public interface IServerHandler
    {
        void HandleStatuses();
        void HandleFiles();
        void SendNewSettings(Guid clientId);
    }
}
