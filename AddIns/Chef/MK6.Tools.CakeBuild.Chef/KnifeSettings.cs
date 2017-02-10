using Cake.Core.Tooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MK6.Tools.CakeBuild.Chef
{
    public class KnifeSettings : ToolSettings
    {
        public KnifeSettings()
        {
            ChefDKVersion = "1.0.3";
        }

        /// <summary>
        /// Optional property to speficy the version of chef dk to install if not found on the system
        /// </summary>
        public string ChefDKVersion { get; set; }

        internal string SubCommand { get; set; }

        internal IDictionary<string, string> Options { get; set; }

        internal string GetOption(string key)
        {
            return Options.ContainsKey(key) ? Options[key] : null;
        }

        internal bool GetOptionToggle(string key)
        {
            return Options.ContainsKey(key) ? true : false;
        }

        internal void SetOption(string key, string value)
        {
            Options[key] = value;
        }

        internal void SetOptionToggle(string key, bool value)
        {
            if (!value && Options.ContainsKey(key))
                Options.Remove(key);
            else if (value)
                Options[key] = string.Empty;
        }
    }

    public class KnifeBootstrapSettings : KnifeSettings
    {
        private const string _runListKey = "--run-list";
        private const string _sudoKey = "--sudo";
        private const string _sshUserKey = "--ssh-user";
        private const string _sshPasswordKey = "--ssh-password";
        private const string _useSudoPasswordKey = "--use-sudo-password";

        public KnifeBootstrapSettings()
        {
            SubCommand = "bootstrap";
            Options = new Dictionary<string, string>();
        }

        public string RunList { get { return GetOption(_runListKey); } set { SetOption(_runListKey, value); } }
        public string FQDNOrIPAddress { get { return GetOption(nameof(FQDNOrIPAddress)); } set { SetOption(nameof(FQDNOrIPAddress), value); } }
        public bool Sudo { get { return GetOptionToggle(_sudoKey); } set { SetOptionToggle(_sudoKey, value); } }
        public bool UseSudoPassword { get { return GetOptionToggle(_sudoKey); } set { SetOptionToggle(_sudoKey, value); } }
        public string SSHUser { get { return GetOption(_sshUserKey); } set { SetOption(_sshUserKey, value); } }
        public string SSHPassword { get { return GetOption(_sshPasswordKey); } set { SetOption(_sshPasswordKey, value); } }

    }
}
