using Aras.IOM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bwsh.OAuth.IamAuthentication
{
    internal class IamHandler : OAuthHandler<IamOptions>
    {
        public ClaimsIdentity identity;
        public string callbackUrl;
        public string StartItem;
        public IamHandler(IOptionsMonitor<IamOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        #region 重写

        // <summary>
        /// OAuth每次请求的Handle
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> HandleRequestAsync()
        {
            WriteLog("44: ");
            /* ShouldHandleRequestAsync()  源码就是比较回调地址和请求地址是否一致   
            ShouldHandleRequestAsync()  => Task.FromResult(Options.CallbackPath == Request.Path);
            */
            if (!await ShouldHandleRequestAsync())
            {
                return false;
            }

            //拿到code回调之后会进行后续操作
            AuthenticationTicket ticket = null;
            Exception exception = null;
            AuthenticationProperties properties = null;
            try
            {
                var authResult = await HandleRemoteAuthenticateAsync();
                WriteLog("60 authResult:" + (authResult != null).ToString());
                if (authResult == null) exception = new InvalidOperationException("Invalid return state, unable to redirect.");
                else if (authResult.Handled) return true;
                else if (authResult.Skipped || authResult.None) return false;
                else if (!authResult.Succeeded)
                {
                    exception = authResult.Failure ?? new InvalidOperationException("Invalid return state, unable to redirect.");
                    properties = authResult.Properties;
                }
                ticket = authResult.Ticket;
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteLog("74: " + ex.Message);
            }

            WriteLog("77:" + (exception != null).ToString());
            if (exception != null)
            {
                RemoteFailureContext errorContext = new RemoteFailureContext(Context, Scheme, Options, exception)
                {
                    Properties = properties
                };
                await Events.RemoteFailure(errorContext);

                if (errorContext.Result != null)
                {
                    if (errorContext.Result.Handled) return true;
                    else if (errorContext.Result.Skipped) return false;
                    else if (errorContext.Result.Failure != null)
                    {
                        WriteLog("RemoteFailure事件返回错误." + errorContext.Result.Failure);
                    }
                }
                WriteLog("95:" + errorContext.Failure.ToString());
                if (errorContext.Failure != null)
                {
                    WriteLog("处理远程登录时遇到错误." + errorContext.Result.Failure);
                }
            }
            WriteLog("101:");
            // We have a ticket if we get here
            var ticketContext = new TicketReceivedContext(Context, Scheme, Options, ticket)
            {
                ReturnUri = ticket.Properties.RedirectUri
            };
            WriteLog("107:");
            ticket.Properties.RedirectUri = null;

            // Mark which provider produced this identity so we can cross-check later in HandleAuthenticateAsync
            ticketContext.Properties.Items[".AuthScheme"] = Scheme.Name;

            await Events.TicketReceived(ticketContext);

            if (ticketContext.Result != null)
            {
                if (ticketContext.Result.Handled) return true;
                else if (ticketContext.Result.Skipped) return false;
            }
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            WriteLog("122：" + identity.Claims.Count().ToString());

            try
            {
                foreach (var item in identity.Claims)
                {
                    WriteLog("Claims：" + item.Value.ToString() + "__" + item.Type.ToString());
                }

                Task task = Context.SignInAsync(SignInScheme, ticketContext.Principal, ticketContext.Properties);
                WriteLog("132:SignInAsync");
                WriteLog("133:" + task.IsCompletedSuccessfully.ToString());//True
            }
            catch (Exception ex)
            {
                var aa = ex.Message;
                WriteLog("138 Exception：" + ex.Message);
            }
            //await Context.SignInAsync("Wechat", new ClaimsPrincipal(identity), properties);
            // await Context.SignInAsync(SignInScheme, ticketContext.Principal!, ticketContext.Properties);

            // Default redirect path is the base path
            if (string.IsNullOrEmpty(ticketContext.ReturnUri))
            {
                ticketContext.ReturnUri = "/";
            }

            WriteLog("callbackUrl:" + callbackUrl);
            if (string.IsNullOrEmpty(callbackUrl))
            {
                callbackUrl = Request.GetDisplayUrl();
            }
            WriteLog("154 callbackUrl:" + callbackUrl);
            string newUrl = callbackUrl.Substring(0, callbackUrl.IndexOf("/OAuthServer"));
            if (!string.IsNullOrEmpty(StartItem))
                newUrl = newUrl + $"?StartItem={StartItem}";
            WriteLog("newUrl:" + newUrl);
            Response.Redirect(newUrl);

            //Response.Redirect(callbackUrl);//.Replace("/OAuthServer/External/callback", "")

            return true;
        }

        /// <summary>
        /// 第二步 将code换成钉钉的authCode
        /// </summary>
        /// <returns></returns>
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            // 获取当前请求的完整 URL
            var requestUrl = base.Request.Scheme + "://" + base.Request.Host + base.Request.Path + base.Request.QueryString;
            // 打印或使用请求 URL
            WriteLog($"当前请求 URL: {requestUrl}");

            IQueryCollection query = base.Request.Query;
            var properties = new AuthenticationProperties { RedirectUri = Options.CallbackPath };

            properties.Items["database"] = Options.Database;
            //StringValues stringValues = query["state"];
            //WriteLog("153 state:" + stringValues);
            //AuthenticationProperties properties = base.Options.StateDataFormat.Unprotect(stringValues);
            //if (properties == null)
            //{
            //    return HandleRequestResult.Fail("The oauth state was missing or invalid.");
            //}

            //if (!ValidateCorrelationId(properties))
            //{
            //    return HandleRequestResult.Fail("Correlation failed.", properties);
            //}

            //StringValues stringValues2 = query["error"];
            //if (!StringValues.IsNullOrEmpty(stringValues2))
            //{
            //    StringBuilder stringBuilder = new StringBuilder();
            //    stringBuilder.Append(stringValues2);
            //    StringValues stringValues3 = query["error_description"];
            //    if (!StringValues.IsNullOrEmpty(stringValues3))
            //    {
            //        stringBuilder.Append(";Description=").Append(stringValues3);
            //    }

            //    StringValues stringValues4 = query["error_uri"];
            //    if (!StringValues.IsNullOrEmpty(stringValues4))
            //    {
            //        stringBuilder.Append(";Uri=").Append(stringValues4);
            //    }

            //    return HandleRequestResult.Fail(stringBuilder.ToString(), properties);
            //}

            StartItem = query["StartItem"];
            WriteLog("StartItem:" + StartItem);

            StringValues stringValues5 = query["code"];//authCode code
            if (StringValues.IsNullOrEmpty(stringValues5))
            {
                return HandleRequestResult.Fail("authCode was not found.", properties);
            }
            WriteLog("222 authCode:" + stringValues5);
            //触发第三步获取 accesstoken
            var codeExchangeContext = new OAuthCodeExchangeContext(properties, stringValues5, BuildRedirectUri(Options.CallbackPath));
            OAuthTokenResponse oAuthTokenResponse = await ExchangeCodeAsync(codeExchangeContext);
            if (oAuthTokenResponse.Error != null)
            {
                return HandleRequestResult.Fail(oAuthTokenResponse.Error, properties);
            }

            if (string.IsNullOrEmpty(oAuthTokenResponse.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
            }

            ClaimsIdentity identity = new ClaimsIdentity(ClaimsIssuer);
            if (base.Options.SaveTokens)
            {
                List<AuthenticationToken> list = new List<AuthenticationToken>();
                list.Add(new AuthenticationToken
                {
                    Name = "access_token",
                    Value = oAuthTokenResponse.AccessToken
                });
                if (!string.IsNullOrEmpty(oAuthTokenResponse.RefreshToken))
                {
                    list.Add(new AuthenticationToken
                    {
                        Name = "refresh_token",
                        Value = oAuthTokenResponse.RefreshToken
                    });
                }

                if (!string.IsNullOrEmpty(oAuthTokenResponse.TokenType))
                {
                    list.Add(new AuthenticationToken
                    {
                        Name = "token_type",
                        Value = oAuthTokenResponse.TokenType
                    });
                }

                if (!string.IsNullOrEmpty(oAuthTokenResponse.ExpiresIn) && int.TryParse(oAuthTokenResponse.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                {
                    DateTimeOffset dateTimeOffset = base.Clock.UtcNow + TimeSpan.FromSeconds(result);
                    list.Add(new AuthenticationToken
                    {
                        Name = "expires_at",
                        Value = dateTimeOffset.ToString("o", CultureInfo.InvariantCulture)
                    });
                }

                properties.StoreTokens(list);
            }
            //触发第四步获取 获取用户身份
            AuthenticationTicket authenticationTicket = await CreateTicketAsync(identity, properties, oAuthTokenResponse);
            if (authenticationTicket != null)
            {
                return HandleRequestResult.Success(authenticationTicket);
            }

            return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
        }


        /// <summary>
        /// 第三步 通过到code获取Token
        /// 将授权代码交换为来自远程提供程序的授权令牌。
        /// Exchanges the authorization code for a authorization token from the remote provider.
        /// </summary>
        /// <param name="context">The <see cref="OAuthCodeExchangeContext"/>.</param>
        /// <returns>The response <see cref="OAuthTokenResponse"/>.</returns>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            try
            {
                string code = context.Code;
                string redirectUri = context.RedirectUri;
              
                string param = "{\"grant_type\":\"authorization_code\",\"code\":\"" + code + "\",\"client_id\":\"" + Options.ClientId + "\",\"client_secret\":\"" + Options.ClientSecret + "\",\"redirect_uri\":\"" + redirectUri + "\"}";

                var query = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "client_id", Options.ClientId },
                    { "client_secret", Options.ClientSecret },
                    { "redirect_uri", redirectUri }
                }; 
                var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync();
                var tokenUrl = $"{Options.TokenEndpoint}?{queryString}";
                WriteLog("获取token Url：:" + tokenUrl);
                //测试
                //string body = @"{ ""access_token"": ""admin"", ""refresh_token"": ""13311112222"", ""expires_in"": """" }";
                string result = await GetTokenRequest(tokenUrl);
                WriteLog("token返回结果:" + result);

                JsonDocument jdRes = JsonDocument.Parse(result);
                var resCode = jdRes.RootElement.GetProperty("code").GetString();
                string error = jdRes.RootElement.TryGetProperty("error", out var errorElement) ? errorElement.GetString() : "";
                string errorDesc = jdRes.RootElement.TryGetProperty("error_description", out var errorDescElement) ? errorDescElement.GetString() : "";
                string errorMsg = jdRes.RootElement.TryGetProperty("error_message", out var errorMsgElement) ? errorMsgElement.GetString() : "";
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return OAuthTokenResponse.Failed(new Exception("获取token失败:" + result));
                }
                var expires_in = jdRes.RootElement.TryGetProperty("expires_in", out var el)
                   ? el.ValueKind == JsonValueKind.Number
                       ? el.GetInt32()
                       : int.TryParse(el.GetString(), out int val) ? val : 0
                   : 0;
                OAuthTokenResponse tokensInfo = OAuthTokenResponse.Success(jdRes);
                tokensInfo.AccessToken = jdRes.RootElement.GetProperty("access_token").GetString(); 
                tokensInfo.ExpiresIn = expires_in.ToString(); 
                tokensInfo.RefreshToken = jdRes.RootElement.GetProperty("refresh_token").GetString(); 
                tokensInfo.TokenType = jdRes.RootElement.GetProperty("token_type").GetString();
                tokensInfo.Error = null;

                return tokensInfo;
            }
            catch (Exception ex)
            {
                WriteLog("333:" + ex.ToString());
                return OAuthTokenResponse.Failed(new Exception("333:" + ex.ToString()));
            }

        }

        /// <summary>
        /// 第四步 创建票据
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="tokenResponse"></param>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identitys, AuthenticationProperties properties, OAuthTokenResponse tokenResponse)
        {
            foreach (var item in properties.Items)
            {
                if (item.Key == "returnUrl") callbackUrl = item.Value;
            }

            string database = properties.Items["database"].ToString();
            string plmurl = Request.GetDisplayUrl();
            WriteLog("Request.GetDisplayUrl():" + Request.GetDisplayUrl());
            plmurl = plmurl.Substring(0, plmurl.IndexOf("/OAuthServer"));
            WriteLog("356 authCode:" + base.Request.Query["authCode"].ToString());

            string result = await GetUserRequest(Options.UserInformationEndpoint, tokenResponse.AccessToken);
            WriteLog("获取用户信息:" + result);
            if (string.IsNullOrWhiteSpace(result))
            {
                return null;
            }
            JsonDocument jdRes = JsonDocument.Parse(result);
            var resCode = jdRes.RootElement.GetProperty("code").GetString();
            var message = jdRes.RootElement.GetProperty("message").GetString();
            if (resCode != "0")
            {
                return null;
            }
            var stData = jdRes.RootElement.GetProperty("data");
            JsonDocument jdData = JsonDocument.Parse(stData.GetRawText());
            var account = jdData.RootElement.GetProperty("account").GetString();//登陆名
            string username = account;
            WriteLog(username);
            string openId = username;
            //string open_id = JOBody.GetValue("unionId").ToString();

            identitys.AddClaim(new Claim("sub", openId));
            identitys.AddClaim(new Claim("database", database));
            identitys.AddClaim(new Claim("authentication_type", "IamOAuth"));//
            identitys.AddClaim(new Claim("username", username));
            identitys.AddClaim(new Claim("returnUrl ", Request.GetDisplayUrl()));

            identitys.AddClaim(new Claim("shouldSavePreferences ", "true"));

            Options.Scope.Add(openId);
            JObject JOBody = new JObject();
            JOBody["username"] = username;
            JOBody["database"] = database;
            using JsonDocument jsondoc = JsonDocument.Parse(JOBody.ToString());

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identitys), properties
            , Context, Scheme, Options, Backchannel
            , tokenResponse
            , jsondoc.RootElement.Clone());

            await Events.CreatingTicket(context);

            identity = identitys;
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);

        }

        #endregion
      
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetTokenRequest(string url)
        {
            using (var httpClient = new HttpClient())
            {
                // 设置默认请求头
                //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // 
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    // 处理请求异常（如网络错误、状态码错误等）
                    var result = $"发送POST请求出现异常:{url}:{ex.Message}";
                    WriteLog(result);
                    return "";
                }
            }
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> GetUserRequest(string url, string token)
        {
            using (var httpClient = new HttpClient())
            {
                // 设置默认请求头
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Charset", "utf-8");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // 
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    // 处理请求异常（如网络错误、状态码错误等）
                    var result = $"发送Get请求出现异常:{ex.Message}";
                    WriteLog(result);
                    return "";
                }
            }
        }
      
        public static void WriteLog(string content)
        {
            try
            {
                string filePath = @"C:\PLM-Log\logg.txt";
                string logContent = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {content}";

                bool append = true;

                if (System.IO.File.Exists(filePath) == true)
                {
                    var fileinfo = new FileInfo(filePath);
                    if (fileinfo.Length >= 1024 * 1024 * 10) // 如果大于等于 1M（1024B*1024KB)，则重写日志。
                    {
                        append = false;
                    }
                }
                using (var logFileStream = new StreamWriter(filePath, append, Encoding.UTF8))
                {
                    logFileStream.WriteLine(logContent);
                }
                //var logFileStream = new StreamWriter(filePath, append, Encoding.UTF8);
                //logFileStream.WriteLine(content);
                //logFileStream.Flush();
                //logFileStream.Close();
            }
            catch (Exception e)
            {

            }

        }
    }
}
