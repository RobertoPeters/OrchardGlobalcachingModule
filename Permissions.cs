﻿using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Globalcaching
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission FTFAdmin = new Permission { Description = "FTF administratie", Name = "FTFAdmin" };
        public static readonly Permission DistanceAdmin = new Permission { Description = "Afstand administratie", Name = "DistanceAdmin" };
        public static readonly Permission GlobalAdmin = new Permission { Description = "Globalcaching administratie", Name = "GlobalAdmin" };
        public static readonly Permission ApprovedEditor = new Permission { Description = "Goedkeuringbewerker", Name = "ApprovedEditor" };
        public static readonly Permission ApprovedViewer = new Permission { Description = "Goedkeuringcontroleur", Name = "ApprovedViewer" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                FTFAdmin,
                DistanceAdmin,
                GlobalAdmin,
                ApprovedEditor,
                ApprovedViewer
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {FTFAdmin, DistanceAdmin, GlobalAdmin, ApprovedEditor, ApprovedViewer }
                },
                new PermissionStereotype {
                    Name = "Editor",
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                },
                new PermissionStereotype {
                    Name = "Contributor",
                },
            };
        }

    }
}