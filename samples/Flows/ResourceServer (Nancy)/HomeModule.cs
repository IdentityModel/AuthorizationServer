using Nancy;

namespace ResourceServer
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => "Nancy home";
        }
    }
}