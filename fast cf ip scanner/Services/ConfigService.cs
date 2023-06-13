using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace fast_cf_ip_scanner.Services
{
    public class ConfigService
    {
        public ConfigService()
        {
            ConfigProviders = new()
            {
                    new ConfigProviderModel()
                    {
                        name = "vpei",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/vpei/Free-Node-Merge/main/o/node.txt",
                        }
                    },
                    new ConfigProviderModel() {
                        name = "mfuu",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/mfuu/v2ray/master/v2ray",
                        },
                    },
                    new ConfigProviderModel() {
                        name = "peasoft",
                        type = "raw",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/peasoft/NoMoreWalls/master/list_raw.txt",
                        },
                    },
                    new ConfigProviderModel() {
                        name = "ermaozi",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/ermaozi/get_subscribe/main/subscribe/v2ray.txt",
                        }
                    },
                    new ConfigProviderModel() {
                        name = "aiboboxx",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/aiboboxx/v2rayfree/main/v2",
                        },
                    },
                    new ConfigProviderModel() {
                        name = "mahdibland",
                        type = "raw",
                        random = false,
                        urls = new() {
                            "https://raw.githubusercontent.com/mahdibland/V2RayAggregator/master/sub/splitted/vmess.txt",
                            "https://raw.githubusercontent.com/mahdibland/V2RayAggregator/master/sub/splitted/trojan.txt",
                        }
                    },
                    new ConfigProviderModel() {
                        name = "autoproxy",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription1",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription2",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription3",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription4",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription5",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription6",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription7",
                            "https://raw.githubusercontent.com/w1770946466/Auto_proxy/main/Long_term_subscription8",
                        }
                    },
                    new ConfigProviderModel() {
                        name = "freefq",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/freefq/free/master/v2",
                        }
                    },
                    new ConfigProviderModel()
                    {
                        name = "pawdroid",
                        type = "b64",
                        random = true,
                        urls = new() {
                            "https://raw.githubusercontent.com/Pawdroid/Free-servers/main/sub",
                        }
                    }
                };
            AlpnList = new()
            {
                "h2,http/1.1",
                "h2,http/1.1",
                "h2,http/1.1",
                "http/1.1"
            };
            FpList = new()
            {
                "chrome",
                "chrome",
                "chrome",
                "firefox",
                "safari",
                "edge",
                "ios",
                "android",
                "random"
            };
            DomainList = new()
            {
              "discord.com",
              "laravel.com",
              "cdnjs.com",
              "www.speedtest.net",
              "workers.dev",
              "nginx.com",
              "chat.openai.com",
              "auth0.openai.com",
              "codepen.io",
              "api.jquery.com"
            };
        }
        public List<ConfigProviderModel> ConfigProviders { get; set; }
        public List<string> AlpnList { get; set; }

        public List<string> FpList { get; set; }

        public List<string> DomainList { get; set; }






        public async Task<List<string>> GetConfig(ConfigProviderModel configProvider)
        {
            HttpClient client = new HttpClient();
            List<string> configs = new();
            foreach (var url in configProvider.urls)
            {
                var res = await client.GetStringAsync(url);
                if (configProvider.type == "b64")
                {
                    byte[] contentbyteArray = Convert.FromBase64String(res);
                    configs.AddRange(Encoding.UTF8.GetString(contentbyteArray).Split("\n").ToList());
                }
                else
                {
                    configs.AddRange(res.Split("\n").ToList());
                }
            }
            return configs;
        }
        public ConfigModel DecodeConfig(string config)
        {
            var decodedconfig = new ConfigModel();
            if (config.StartsWith("vmess://"))
            {
                try
                {
                    byte[] decodedBytes = Convert.FromBase64String(config.Substring(8));
                    string decodedString = Encoding.UTF8.GetString(decodedBytes);
                    decodedconfig = JsonSerializer.Deserialize<ConfigModel>(decodedString);
                }
                catch (Exception ex)
                {

                }
            }
            else if (config.StartsWith("vless://"))
            {
                try
                {
                    config = config.Substring("vless://".Length);

                    // Extract the ID
                    int idEndIndex = config.IndexOf('@');
                    if (idEndIndex != -1)
                    {
                        decodedconfig.id = config.Substring(0, idEndIndex);
                        config = config.Substring(idEndIndex + 1);
                    }

                    // Extract the Host and Port
                    int hostEndIndex = config.IndexOf(':');
                    int pathStartIndex = config.IndexOf('/');
                    int queryStartIndex = config.IndexOf('?');
                    int fragmentStartIndex = config.IndexOf('#');

                    int endIndex = new[] { hostEndIndex, pathStartIndex, queryStartIndex, fragmentStartIndex }
                        .Where(index => index != -1)
                        .Min();

                    if (endIndex != -1)
                    {
                        decodedconfig.host = config.Substring(0, endIndex);

                        // Extract the Port
                        int portStartIndex = hostEndIndex + 1;
                        int portLength = endIndex - portStartIndex;
                        if (portLength > 0)
                        {
                            decodedconfig.port = config.Substring(portStartIndex, portLength);
                        }

                        config = config.Substring(endIndex);
                    }

                    // Extract the Query string
                    if (queryStartIndex != -1)
                    {
                        int queryEndIndex = fragmentStartIndex != -1 ? fragmentStartIndex : config.Length;
                        string queryString = config.Substring(queryStartIndex + 1, queryEndIndex - queryStartIndex - 1);
                        string[] queryParameters = queryString.Split('&');

                        foreach (var parameter in queryParameters)
                        {
                            string[] keyValue = parameter.Split('=');
                            if (keyValue.Length == 2)
                            {
                                string key = keyValue[0];
                                string value = keyValue[1];

                                switch (key)
                                {
                                    case "type":
                                        decodedconfig.net = value;
                                        break;
                                    case "encryption":
                                        decodedconfig.tls = value;
                                        break;
                                    case "aid":
                                        decodedconfig.add = value;
                                        break;
                                        // Add more cases for other query parameters if needed
                                }
                            }
                        }

                        config = config.Substring(0, queryStartIndex);
                    }

                    // Extract the Fragment
                    if (fragmentStartIndex != -1)
                    {
                        decodedconfig.protocol = Uri.UnescapeDataString(config.Substring(fragmentStartIndex + 1));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else if (config.StartsWith("trojan://"))
            {
                try
                {
                    config = config.Substring("trojan://".Length);
                    int idEndIndex = config.IndexOf('@');
                    if (idEndIndex != -1)
                    {
                        decodedconfig.id = config.Substring(0, idEndIndex);
                        config = config.Substring(idEndIndex + 1);
                    }
                    int hostEndIndex = config.IndexOf(':');
                    int pathStartIndex = config.IndexOf('/');
                    int queryStartIndex = config.IndexOf('?');
                    int fragmentStartIndex = config.IndexOf('#');

                    int endIndex = new[] { hostEndIndex, pathStartIndex, queryStartIndex, fragmentStartIndex }
                        .Where(index => index != -1)
                        .Min();
                    if (endIndex != -1)
                    {
                        decodedconfig.host = config.Substring(0, endIndex);

                        // Extract the Port
                        int portStartIndex = hostEndIndex + 1;
                        int portLength = endIndex - portStartIndex;
                        if (portLength > 0)
                        {
                            decodedconfig.port = config.Substring(portStartIndex, portLength);
                        }

                        config = config.Substring(endIndex);
                    }

                    // Extract the Path
                    if (config.StartsWith("/"))
                    {
                        int pathEndIndex = config.IndexOf('?');
                        if (pathEndIndex != -1)
                        {
                            decodedconfig.path = config.Substring(1, pathEndIndex - 1);
                            config = config.Substring(pathEndIndex);
                        }
                        else
                        {
                            decodedconfig.path = config.Substring(1);
                            config = string.Empty;
                        }
                    }

                    // Extract the Query string
                    if (config.StartsWith("?"))
                    {
                        int queryEndIndex = config.IndexOf('#');
                        if (queryEndIndex != -1)
                        {
                            decodedconfig.ps = Uri.UnescapeDataString(config.Substring(1, queryEndIndex - 1));
                            config = config.Substring(queryEndIndex);
                        }
                        else
                        {
                            decodedconfig.ps = Uri.UnescapeDataString(config.Substring(1));
                            config = string.Empty;
                        }
                    }

                    // Extract the Fragment
                    if (config.StartsWith("#"))
                    {
                        decodedconfig.protocol = Uri.UnescapeDataString(config.Substring(1));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return decodedconfig;
        }
        public List<ConfigModel> DecodeConfig(List<string> configs)
        {
            var decodedconfigList = new List<ConfigModel>();
            foreach (var config in configs)
            {
                var decodedCongig = DecodeConfig(config);
                decodedconfigList.Add(decodedCongig);
            }
            return decodedconfigList;
        }
        public List<string> FilteredVmessVlessTrojanConfigs(List<string> configs)
        {
            List<string> filteredConfigs = configs
                .Where(cnf => Regex.IsMatch(cnf.ToString(), "^(vmess|vless|trojan)://", RegexOptions.IgnoreCase))
                .ToList();
            return filteredConfigs;
        }


        public ConfigModel MixConfig(ConfigModel conf, string ip, string worker)
        {
            if (conf.tls != "tls" || conf.net == "tcp")
            {
                return null;
            }
            var mixedConfig = conf;
            mixedConfig.add = ip;
            mixedConfig.port = "443";
            mixedConfig.path = conf.add + ":" + conf.port + "/" + conf.path;
            mixedConfig.host = worker;
            mixedConfig.sni = worker;
            var rand = new Random();
            mixedConfig.alpn = AlpnList[rand.Next(AlpnList.Count)];
            mixedConfig.fp = FpList[rand.Next(FpList.Count)];
            return mixedConfig;
        }
    }
}
