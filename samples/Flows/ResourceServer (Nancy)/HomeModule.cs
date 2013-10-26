using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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