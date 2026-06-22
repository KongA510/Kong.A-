using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwsh.OAuth.IamAuthentication
{
    internal class IamOptions : OAuthOptions
    {          
        /// <summary>
        /// PLM root秘钥
        /// </summary>
        public string PlmSecret { get; set; }

        /// <summary>
        /// PLM DB
        /// </summary>
        public string Database { get; set; }
       
    }
}
