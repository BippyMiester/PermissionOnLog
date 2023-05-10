using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("PermissionOnLog", "BippyMiester", "0.1.0")]
    [Description("Grants or revokes permissions or groups to a player when they log in or out of the server")]
    class PermissionOnLog : CovalencePlugin
    {
        private PluginConfig _config;

        private void Init()
        {
            Puts("PermissionOnLog has been initialized...");
            _config = Config.ReadObject<PluginConfig>();
        }

        void OnUserConnected(IPlayer player)
        {
            foreach(string permissionToGrant in _config.PermissionGrantOnLogin )
            {
                bool PermissionExists = permission.PermissionExists(permissionToGrant);
                if(!PermissionExists)
                {
                    Puts($"Persmission {permissionToGrant} does not exist");
                    continue;
                }
                Puts($"Adding {permissionToGrant} permission to {player.Name}");
                permission.GrantUserPermission(player.Id, permissionToGrant, null);
            }

            foreach (string groupToGrant in _config.GroupGrantOnLogin)
            {
                bool GroupExists = permission.GroupExists(groupToGrant);
                if(!GroupExists)
                {
                    Puts($"Group {groupToGrant} does not exist");
                    continue;
                }
                Puts($"Adding {groupToGrant} group to {player.Name}");
                permission.AddUserGroup(player.Id, groupToGrant);
            }
        }

        void OnUserDisconnected(IPlayer player)
        {
            Puts($"Revoking permissions from player {player.Name}");
            foreach (string permissionToRevoke in _config.PermissionRevokeOnLogout)
            {
                bool PermissionExists = permission.PermissionExists(permissionToRevoke);
                if (!PermissionExists)
                {
                    Puts($"ERROR: Persmission {permissionToRevoke} does not exist");
                    continue;
                }

                bool UserHasPermission = permission.UserHasPermission(player.Id, permissionToRevoke);
                if(!UserHasPermission)
                {
                    Puts($"ERROR: User {player.Name} does not have the permission {permissionToRevoke}");
                    continue;
                }

                Puts($"Revoking {permissionToRevoke} permission from {player.Name}");
                permission.RevokeUserPermission(player.Id, permissionToRevoke);
            }

            Puts($"Revoking groups from player {player.Name}");
            foreach (string groupToRevoke in _config.GroupRevokeOnLogout)
            {
                bool GroupExists = permission.GroupExists(groupToRevoke);
                if (!GroupExists)
                {
                    Puts($"ERROR: Group {groupToRevoke} does not exist");
                    continue;
                }

                bool UserHasGroup = permission.UserHasGroup(player.Id, groupToRevoke);
                if (!UserHasGroup)
                {
                    Puts($"ERROR: User {player.Name} is not apart of the {groupToRevoke} group");
                    continue;
                }

                Puts($"Revoking {groupToRevoke} group from {player.Name}");
                permission.RemoveUserGroup(player.Id, groupToRevoke);
            }
        }

        #region Config

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig();
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<PluginConfig>();
        }

        private class PluginConfig
        {
            public string[] PermissionGrantOnLogin = { "permission1" };
            public string[] PermissionRevokeOnLogout = { "permission1" };
            public string[] GroupGrantOnLogin = { "group1" };
            public string[] GroupRevokeOnLogout = { "group2" };
        }

        #endregion
    }


}
