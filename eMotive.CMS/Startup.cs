﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(eMotive.Startup))]
namespace eMotive
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
