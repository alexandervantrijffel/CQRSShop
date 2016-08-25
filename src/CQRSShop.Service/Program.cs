using Topshelf;

namespace CQRSShop.Service
{
    class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<MyReplayService>(s =>
                {
                    //s.ConstructUsing(name => new IndexingService());
					s.ConstructUsing(name => new MyReplayService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("CQRSShop.Service");
                x.SetDisplayName("CQRSShop.Service");
                x.SetServiceName("CQRSShop.Service");
            });
        }
    }
}
