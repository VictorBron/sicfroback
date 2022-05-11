using System;

namespace BackendOperacionesFroward.Entities.Models
{
    public static class EnumObjects
    {

        /*
        * Types of users and powers/responsabilities:
        *  FROWARD_ADMIN:              0
        *      - CRUD profile users/clients
        *      - VIEW "Schedule attention" 
        *      - VIEW requests
        *      - // Gestión de mantenedores ??
        *  FROWARD_SUPERVISOR:         1
        *      - VIEW requests
        *      - CRUD of the "Schedule attention"
        *  FROWARD_READER:             2
        *      - VIEW requests
        *      - VIEW Schedule attention
        *      
        *  CLIENT                      3
        *      - CRU requests
        *      - VIEW Schedule attention
        */


        public enum USER_TYPES
        {
            FROWARD_ADMIN,
            FROWARD_SUPERVISOR,
            FROWARD_READER,
            CLIENT,
        }

        public enum REQUEST_STATE { 
            EMPTY = 0,
            VALID = 1,
            DEFEATED = 2,
            CANCELED = 3
        }
        public enum VEHICLE_TYPE
        {
            MAYOR = 0,
            MINOR = 1,
        }

        public static string GetUserType(int key)
        {
            return Enum.GetName(typeof(USER_TYPES), key);
        }

        public static string GetVehicleType(int key)
        {
            if (key == (int)VEHICLE_TYPE.MAYOR)
                return "VEHICULO MAYOR";
            else if (key == (int)VEHICLE_TYPE.MINOR)
                return "VEHICULO MENOR";
            return "";
        }

    }
}
