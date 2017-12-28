using System;
using AOPDynamicProxy;
using Topshelf;
using Autofac;
using Castle.DynamicProxy;


namespace ArchServer
{

    class Program
    {
        public static IContainer Container { get; set; }
        static void Main(string[] args)
        {
            var generator = new ProxyGenerator();

            var builder = new ContainerBuilder();

            builder.Register(c =>
                    generator.CreateInterfaceProxyWithTarget<IMessagingHelper>(new MessagingHelper(),
                        new LogInterceptor()))
                .As<IMessagingHelper>();
            Container = builder.Build();

            //var willBeIntercepted = Container.Resolve<IMessagingHelper>();
           
            HostFactory.Run(x =>
            {
                x.Service<ArchServer>(conf =>
                {
                    conf.ConstructUsing(() => new ArchServer());
                    conf.WhenStarted(s => s.Start());
                    conf.WhenStopped(s => s.Stop());
                });
                x.StartAutomaticallyDelayed();
                x.RunAsLocalService();
                x.EnableServiceRecovery(r => r.RestartService(0).RestartService(1));
            });
        }
    }
}
