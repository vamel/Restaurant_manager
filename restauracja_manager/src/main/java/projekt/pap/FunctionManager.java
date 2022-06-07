package projekt.pap;

import javafx.event.EventHandler;
import javafx.event.ActionEvent;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;


interface f_interface {
    default EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                System.out.println("Unimplemented");
            }
        };
    }

    default Set<Integer> getIds() {
        return Set.<Integer>of();
    };

    default String getButtonName(){
        return "Unnamed";
    }
}

/* How to implement button fuctionalities:
 * 1. Create class implementing interface above
 * 2. In handle method, implement the real functionality of the button
 * 3. Initialize set of integers containing ids of users allowed to use this functionality
 * 4. Implement getButtonName returning name of this functionality
 * 5. Create instance of this method in list_of_functions of FunctionManager
*/

class DBFunction_Test implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                System.out.println("Test");
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2, 3);
    };

    public String getButtonName(){
        return "TEST";
    }
}

class DBFManageMenu implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var menu = new ManageMenu();
                menu.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2);
    };

    public String getButtonName(){
        return "Manage Menu";
    }
}

class DBFViewIngredients implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var ing_window = new ViewIngredients();
                ing_window.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2);
    };

    public String getButtonName(){
        return "View Ingredients";
    }
}

class DBFManageEmployees implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var emp_window = new ManageEmployees();
                emp_window.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of();
    };

    public String getButtonName(){
        return "Manage Employees";
    }
}

class DBFNewEmployee implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var new_emp_window = new NewEmployeeWindow();
                new_emp_window.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of();
    };

    public String getButtonName(){
        return "New Employee";
    }
}

class DBFNewOrder implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var order_window = new NewCustomerOrder();
                order_window.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2, 3);
    };

    public String getButtonName(){
        return "New Order";
    }
}

class DBFReserveTable implements f_interface{
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var table_man = new MakeReservation();
                table_man.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2, 3);
    };

    public String getButtonName(){
        return "Reserve Table";
    }
}

class DBFSupplierManagement implements f_interface {
    public EventHandler<ActionEvent> getHandler(Object... args){
        return new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent event) {
                var supplierManagement = new SupplierManagementWindow();
                supplierManagement.run();
            }
        };
    }

    public Set<Integer> getIds() {
        return Set.<Integer>of(2);
    };

    public String getButtonName(){
        return "Manage suppliers";
    }
}

public class FunctionManager {
    private List<f_interface> list_of_functions = List.<f_interface>of(
        new DBFunction_Test(),
        new DBFReserveTable(),
        new DBFNewEmployee(),
        new DBFNewOrder(),
        new DBFManageEmployees(),
        new DBFViewIngredients(),
        new DBFManageMenu(),
        new DBFSupplierManagement()
    );

    public ArrayList<f_interface> getFunctions(int id){
        var result = new ArrayList<f_interface>();

        var iterator = list_of_functions.iterator();
        while(iterator.hasNext()){
            var func_obj = iterator.next();

            if(func_obj.getIds().contains(id) || id == 0){
                result.add(func_obj);
            }
        }
        return result;
    }
}
