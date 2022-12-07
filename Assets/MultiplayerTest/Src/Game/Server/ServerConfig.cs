using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLGFramework
{
    public class ServerConfig
    {
        public string SessionName { get; private set; } = "localhost";
        public string Region { get; private set; }
        public string Lobby { get; private set; }
        public ushort Port { get; private set; } = 6540;
        public ushort PublicPort { get; private set; }
        public string PublicIP { get; private set; }
        public Dictionary<string, SessionProperty> SessionProperties { get; private set; } = new Dictionary<string, SessionProperty>();

        private ServerConfig() { }

        public static ServerConfig Resolve()
        {
            ServerConfig config = new ServerConfig();

            // Session Name
            if (CommandLineUtils.TryGetArg(out string sessionName, "-session")) {
                config.SessionName = sessionName;
            }

            // Custom Region
            if (CommandLineUtils.TryGetArg(out string customRegion, "-region")) {
                config.Region = customRegion;
            }

            // Server Lobby
            if (CommandLineUtils.TryGetArg(out string customLobby, "-lobby")) {
                config.Lobby = customLobby;
            }

            // Server Port
            if (CommandLineUtils.TryGetArg(out string customPort, "-port", "-PORT") &&
              ushort.TryParse(customPort, out var port)) {
                config.Port = port;
            }

            // Custom Public IP
            if (CommandLineUtils.TryGetArg(out string customPublicIP, "-publicip")) {
                config.PublicIP = customPublicIP;
            }

            // Custom Public Port
            if (CommandLineUtils.TryGetArg(out string customPublicPort, "-publicport") && ushort.TryParse(customPublicPort, out var publicPort)) {
                config.PublicPort = publicPort;
            }

            // Server Properties
            List<(string, string)> argsCustomProps = CommandLineUtils.GetArgumentList("-P");

            foreach ((string, string) item in argsCustomProps) {
                string key = item.Item1;
                string value = item.Item2;

                if (int.TryParse(value, out var result)) {
                    config.SessionProperties.Add(key, result);
                    continue;
                }

                config.SessionProperties.Add(key, value);
            }

            return config;
        }

        public override string ToString()
        {
            string properties = string.Empty;

            foreach (KeyValuePair<string, SessionProperty> item in SessionProperties) {
                properties += $"{item.Value}={item.Value}, ";
            }

            return $"[{nameof(ServerConfig)}]: " +
              $"{nameof(SessionName)}={SessionName}, " +
              $"{nameof(Region)}={Region}, " +
              $"{nameof(Lobby)}={Lobby}, " +
              $"{nameof(Port)}={Port}, " +
              $"{nameof(PublicIP)}={PublicIP}, " +
              $"{nameof(PublicPort)}={PublicPort}, " +
              $"{nameof(SessionProperties)}={properties}]";
        }
    }
}
