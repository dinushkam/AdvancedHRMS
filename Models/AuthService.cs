using AdvancedHRMS.Models;

namespace AdvancedHRMS
{
    public static class AuthService
    {
        public static User CurrentUser { get; set; }

        public static bool IsAdmin => CurrentUser?.Role == "Admin";
        public static bool IsHR => CurrentUser?.Role == "Manager";
        public static bool IsEmployee => CurrentUser?.Role == "Employee";

        public static string CurrentUserEmail { get; internal set; }

        public static bool HasAccess(string requiredRole)
        {
            return CurrentUser?.Role == requiredRole || CurrentUser?.Role == "Admin";
        }

        internal static void Logout()
        {
            throw new NotImplementedException();
        }
    }
}