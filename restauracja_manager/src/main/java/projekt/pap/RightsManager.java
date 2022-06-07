package projekt.pap;

import java.util.HashMap;
import java.util.Map;
import java.util.Set;

public class RightsManager {
    /* Rights levels:
     * 0 - admin - absolute power
     * 0 - owner - absolute power in his restaurant
     * 2 - manager - more power than employee
     * 3 - employee - basic rights
     * 99 - nothing
    */

    private static Map<String, Integer> dict = new HashMap<String, Integer>(){{
      put("admin", 0);
      put("owner", 0);
      put("manager", 2);
      put("employee", 3);
    }};

    public static int getRightsByRole(String role){
        if (role == null){
            return 99;
        }
        return dict.get(role.toLowerCase());
    }

    public static Set<String> getEmployeeRoles(){
        var new_dict = dict;
        new_dict.remove("owner");
        new_dict.remove("admin");

        return new_dict.keySet();
    }
}
