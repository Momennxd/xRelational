namespace XRelational_Engine
{
    public class clsQueryEntryPoint
    {

        public enum enEntryPoints
        {
            eSelect = 1,
            eInsert = 2,
            eDelete = 3,
            eUpdate = 4
        };

        public static enEntryPoints GetEntryPoint(string sEntryPoint)
        {
           switch (sEntryPoint.ToLower())
            {
                case "select":
                    return enEntryPoints.eSelect;
                case "insert":
                    return enEntryPoints.eInsert;
                case "delete":
                    return enEntryPoints.eDelete;
                case "update":
                    return enEntryPoints.eUpdate;
                default:
                    return enEntryPoints.eSelect;
            }
        }

    }
}
