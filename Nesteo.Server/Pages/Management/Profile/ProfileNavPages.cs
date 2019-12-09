using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nesteo.Server.Pages.Management.Profile
{
    public static class ProfileNavPages
    {
        public static string EditProfile => "EditProfile";

        public static string ChangePassword => "ChangePassword";

        public static string GetPageNavClass(ViewContext viewContext, string page)
        {
            string activePage = viewContext.ViewData["ActivePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
