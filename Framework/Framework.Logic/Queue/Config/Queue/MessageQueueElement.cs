using System;
using System.Configuration;

namespace Framework.Logic.Queue.Config.Queue
{
    /// <summary>
    ///     A simple class for parsing a named servie out of a configuration file.
    /// </summary>
    public class MessageQueueElement : ConfigurationElement
    {
        /// <summary>
        ///     The name of the queue.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("messageQueueHostServerName", IsRequired = true)]
        public string MessageQueueHostServerName
        {
            get { return (string) this["messageQueueHostServerName"]; }
            set { this["messageQueueHostServerName"] = value; }
        }

        [ConfigurationProperty("multicastAddress", IsRequired = false)]
        public string MulticastAddress
        {
            get { return (string) this["multicastAddress"]; }
            set { this["multicastAddress"] = value; }
        }

        [ConfigurationProperty("isTransactional", IsRequired = false)]
        public bool IsTransactional
        {
            get { return Convert.ToBoolean(this["isTransactional"]); }
            set { this["isTransactional"] = value; }
        }

        [ConfigurationProperty("auditActivity", IsRequired = false)]
        public bool AuditActivity
        {
            get { return Convert.ToBoolean(this["auditActivity"]); }
            set { this["auditActivity"] = value; }
        }

        [ConfigurationProperty("defaultRecoverable", IsRequired = false)]
        public bool DefaultRecoverable
        {
            get { return Convert.ToBoolean(this["defaultRecoverable"]); }
            set { this["defaultRecoverable"] = value; }
        }

        [ConfigurationProperty("mode", IsRequired = true)]
        public string Mode
        {
            get { return (string) this["mode"]; }
            set { this["mode"] = value; }
        }

        [ConfigurationProperty("messageType", IsRequired = true)]
        public string MessageType
        {
            get { return (string) this["messageType"]; }
            set { this["messageType"] = value; }
        }
    }
}