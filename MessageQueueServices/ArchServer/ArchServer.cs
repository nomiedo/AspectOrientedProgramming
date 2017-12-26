using System.Threading;
using System.Threading.Tasks;
using ArchServer.DAL;
using Castle.DynamicProxy;
using AOPDynamicProxy;

namespace ArchServer
{
    class ArchServer
    {
        private readonly Timer statusTimer;
        private IServerHandler serverHandler;

        static object _sync3 = new object();

        public ArchServer()
        {
            statusTimer = new Timer(HandleStatuses);

            var generator = new ProxyGenerator();
            serverHandler = generator.CreateInterfaceProxyWithTarget<IServerHandler>(new ServerHandler(), new LogInterceptor());
        }

        public bool Start()
        {
            statusTimer.Change(0, 1000);
            return true;
        }

        public bool Stop()
        {
            lock (_sync3)
            {
                statusTimer.Change(Timeout.Infinite, 0);
                return true;
            }
        }

        private void HandleStatuses(object target)
        {
            lock (_sync3)
            {
                Task.Run(() =>
                {
                    serverHandler.HandleStatuses();
                });
            }
        }
    }
}