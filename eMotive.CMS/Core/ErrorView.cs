using System.Collections.Generic;
using eMotive.CMS.Services.Interfaces;
/*using Ninject;

namespace eMotive.CMS.Core
{
    public class ErrorView
    {
        [Inject]
        public IConfigurationService Configuration { get; set; }

        private string _referer;

        public IEnumerable<string> Errors { get; set; }
        public string ControllerName { get; set; }
        public string Referer
        {
            set { _referer = value; }
        }

        public string GetReferrer()
        {
            return !string.IsNullOrEmpty(_referer) ? _referer : Configuration.FetchDefaultReferer();
        }
    }
}*/