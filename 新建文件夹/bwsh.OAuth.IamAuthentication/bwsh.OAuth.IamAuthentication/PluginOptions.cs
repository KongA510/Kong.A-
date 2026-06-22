using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwsh.OAuth.IamAuthentication
{
    internal class PluginOptions
    {
        public string AuthenticationType { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 授权URL
        /// </summary>
        public string Instance { get; set; }
        //public string Domain { get; set; }

        /// <summary>
        /// Client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// plm callback   /signin-oidc
        /// </summary>
        public string CallbackPath { get; set; }

        /// <summary>
        /// PLM root秘钥
        /// </summary>
        public string PlmSecret { get; set; }

        /// <summary>
        /// PLM DB
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Token Url
        /// </summary>
        public string TokenEndpoint { get; set; }
        /// <summary>
        /// 获取用户信息URL
        /// </summary>
        public string UserInformationEndpoint { get; set; }
        
    }
}
