namespace BR.helper
{
    public class Constants
    {
    }

    public static class UserTypes
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string User = "User";
        public const string Contractor = "Contractor"; // المقاول
        public const string Worker = "Worker"; //العامل
        public const string ConstructionShop = "ConstructionShop"; // محلات البناء
        public const string Customer = "Customer"; //المستخدم العادي للتطبيق
        public const string Factory = "Factory"; //المستخدم العادي للتطبيق

        public static IEnumerable<string> getUserTypes()
        {
            List<string> types = new List<string>();
            types.Add(Admin);
            types.Add(SuperAdmin);
            types.Add(Contractor);
            types.Add(Worker);
            types.Add(ConstructionShop);
            types.Add(Customer);
            types.Add(Factory);
            return types;
        }
    }

    public static class SysConfigCode
    {
        public static int Maintenance = 1;
        public static int Version = 1;
    }
}
