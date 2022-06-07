package projekt.pap;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;


public class ManageEmployees {
    private VBox box = new VBox();
    private TableView<EmployeeRecord> table;
    private Label err_label;

    private void update_table(){
        table.getItems().clear();
        int res_id = App.rest_id;

        HttpResponse<String>result_emp = null;

        try {
            result_emp = RestaurantAPI.getRestaurantEmployees(res_id, App.token);
        } catch (IOException|InterruptedException err) {
            err_label.setText("Unable to connect.");
            err_label.setVisible(true);
            return;
        }

        if(result_emp.statusCode() != 200){
            try {
                var status_getter = new JSONObject(result_emp.body());
                err_label.setText(status_getter.getString("message"));
                err_label.setVisible(true);
            } catch (JSONException err) {
                err_label.setText("Critical backend error");
                err_label.setVisible(true);
            }
            return;
        }

        var employees = new JSONArray(result_emp.body());

        for (Object object : employees) {
            var emp_obj = new JSONObject(object.toString());
            table.getItems().add(new EmployeeRecord(
                    emp_obj.getInt("id"), emp_obj.getString("name"),
                    emp_obj.getString("surname"), emp_obj.getString("birthDate"),
                    emp_obj.getInt("salary"), emp_obj
            ));
        }
    }

    private void setup_ui(){
        box.setId("vbox");

        err_label = new Label("Manage employees");
        err_label.setVisible(false);
        box.getChildren().add(err_label);

        table = new TableView<EmployeeRecord>();
        table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
        table.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);

        var id_col = new TableColumn<EmployeeRecord, String>("ID");
        id_col.setCellValueFactory(new PropertyValueFactory<>("id"));
        id_col.setResizable(false);
        var name_col = new TableColumn<EmployeeRecord, String>("name");
        name_col.setCellValueFactory(new PropertyValueFactory<>("name"));
        var surname_col = new TableColumn<EmployeeRecord, String>("surname");
        surname_col.setCellValueFactory(new PropertyValueFactory<>("surname"));
        var salary_col = new TableColumn<EmployeeRecord, String>("salary (zł)");
        salary_col.setCellValueFactory(new PropertyValueFactory<>("salary"));
        var bDate_col = new TableColumn<EmployeeRecord, String>("birth date");
        bDate_col.setCellValueFactory(new PropertyValueFactory<>("birthDate"));
        table.getColumns().addAll(id_col, name_col, surname_col, salary_col, bDate_col);

        update_table();
        box.getChildren().add(table);

        var h_box = new HBox();
        h_box.setSpacing(15);
        h_box.setAlignment(Pos.CENTER);
        h_box.setPadding(new Insets(10));

        var lay_off_button = new Button("Lay off employee");
        lay_off_button.setId("lay_off_button");

        var salary_button = new Button("Change salary");
        salary_button.setId("update_button");

        var role_button = new Button("Update role");
        role_button.setId("update_button");

        h_box.getChildren().addAll(lay_off_button, salary_button, role_button);
        box.getChildren().add(h_box);

        var update_label = new Label("Update");
        update_label.setVisible(false);
        box.getChildren().add(update_label);

        var stack_boxes = new StackPane();
        stack_boxes.setAlignment(Pos.CENTER);
        stack_boxes.setPadding(new Insets(10));

        var h_box_hidden = new HBox();
        h_box_hidden.setSpacing(15);
        h_box_hidden.setAlignment(Pos.CENTER);
        h_box_hidden.setPadding(new Insets(10));
        h_box_hidden.setVisible(false);

        var new_salary_field = new TextField();
        new_salary_field.setPromptText("New salary (zł)");

        var new_salary_button = new Button("Update");
        new_salary_button.setId("update_button");

        h_box_hidden.getChildren().addAll(new_salary_field, new_salary_button);
        box.getChildren().add(h_box_hidden);

        var h_box_hidden_role = new HBox();
        h_box_hidden_role.setSpacing(15);
        h_box_hidden_role.setAlignment(Pos.CENTER);
        h_box_hidden_role.setPadding(new Insets(10));
        h_box_hidden_role.setVisible(false);

        var new_role_button = new Button("Update");
        new_role_button.setId("update_button");

        var combo = new ComboBox<String>();
        combo.getItems().addAll(RightsManager.getEmployeeRoles());

        h_box_hidden_role.getChildren().addAll(combo, new_role_button);
        stack_boxes.getChildren().addAll(h_box_hidden, h_box_hidden_role);
        box.getChildren().add(stack_boxes);

        lay_off_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                h_box_hidden.setVisible(false);
                h_box_hidden_role.setVisible(false);
                stack_boxes.setVisible(false);
                err_label.setVisible(false);
                update_label.setVisible(false);
                var selected = table.getSelectionModel().getSelectedItems();
                if (selected.size() == 0) {
                    err_label.setVisible(true);
                    err_label.setText("No records chosen");
                    return;
                }

                for (EmployeeRecord emp : selected) {
                    HttpResponse<String> result = null;
                    try {
                        result = RestaurantAPI.fireEmployee(Integer.valueOf(emp.getId()), App.token);
                    } catch (IOException | InterruptedException err) {
                        err_label.setVisible(true);
                        err_label.setText("Unable to connect");
                        return;
                    }

                    if(result.statusCode() != 200){
                        try {
                            var status_getter = new JSONObject(result.body());
                            err_label.setText(status_getter.getString("message"));
                            err_label.setVisible(true);
                        } catch (JSONException err) {
                            System.out.println(result.body());
                            err_label.setText("Critical backend error");
                            err_label.setVisible(true);
                        }
                        return;
                    }
                }
                err_label.setText("Employee/s fired from restaurant.");
                err_label.setVisible(true);
                update_table();
            }
        });

        salary_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                stack_boxes.setVisible(true);
                h_box_hidden.setVisible(true);
                h_box_hidden_role.setVisible(false);
                new_salary_field.clear();
                err_label.setVisible(true);
                err_label.setText("Insert new salary (zł)");
                update_label.setText("Update employee salary");
                update_label.setVisible(true);
            }
        });

        role_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                stack_boxes.setVisible(true);
                h_box_hidden.setVisible(false);
                h_box_hidden_role.setVisible(true);
                new_salary_field.clear();
                err_label.setVisible(true);
                err_label.setText("Insert new role");
                update_label.setText("Update employee role");
                update_label.setVisible(true);
            }
        });


        new_salary_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                err_label.setVisible(false);
                update_label.setVisible(false);
                var selected = table.getSelectionModel().getSelectedItems();
                if (selected.size() == 0) {
                    err_label.setVisible(true);
                    err_label.setText("No records chosen");
                    return;
                }

                var new_value = new_salary_field.getText().strip();

                if(new_value.isBlank()){
                    err_label.setVisible(true);
                    err_label.setText("New value is empty");
                    return;
                }

                int value = 0;

                try {
                    value = Integer.valueOf(new_value) * 100;
                } catch (Exception err) {
                    err_label.setVisible(true);
                    err_label.setText("New salary (zł) must be a number");
                    return;
                }

                if(value < 2000 || value > 1000000){
                    err_label.setVisible(true);
                    err_label.setText("New salary must be between 2000 zł and 1000000 zł");
                    return;
                }

                for (EmployeeRecord emp : selected) {
                    HttpResponse<String> result = null;
                    try {
                        result = RestaurantAPI.updateEmployeeSalary(Integer.valueOf(emp.getId()),
                                value, App.token);
                    } catch (IOException | InterruptedException err) {
                        err_label.setVisible(true);
                        err_label.setText("Unable to connect");
                        return;
                    }

                    if(result.statusCode() != 200){
                        try {
                            var status_getter = new JSONObject(result.body());
                            err_label.setText(status_getter.getString("message"));
                            err_label.setVisible(true);
                        } catch (JSONException err) {
                            System.out.println(result.body());
                            err_label.setText("Critical backend error");
                            err_label.setVisible(true);
                        }
                        return;
                    }
                }
                err_label.setText("Salary of employee/s has been changed.");
                err_label.setVisible(true);
                update_table();
            }
        });

        new_role_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                err_label.setVisible(false);
                update_label.setVisible(false);
                var selected = table.getSelectionModel().getSelectedItems();
                if (selected.size() == 0) {
                    err_label.setVisible(true);
                    err_label.setText("No records chosen");
                    return;
                }

                var new_value = combo.getSelectionModel().getSelectedItem().toLowerCase();

                for (EmployeeRecord emp : selected) {
                    HttpResponse<String> result = null;
                    String name = emp.getName();

                    try {
                        result = RestaurantAPI.updateEmployeeRole(new_value, Integer.valueOf(emp.getId()), App.token);
                    } catch (IOException | InterruptedException err) {
                        err_label.setVisible(true);
                        err_label.setText("Unable to connect");
                        return;
                    }

                    if(result.statusCode() != 200){
                        try {
                            var status_getter = new JSONObject(result.body());
                            err_label.setText(status_getter.getString("message"));
                            err_label.setVisible(true);
                        } catch (JSONException err) {
                            System.out.println(result.body());
                            err_label.setText("Critical backend error");
                            err_label.setVisible(true);
                        }
                        return;
                    }

                    err_label.setText(
                            String.format("Role of employee %s has been changed", name));
                    err_label.setVisible(true);
                }
                update_table();
            }
        });
    }

    public void run(){
        var stage = new Stage();
        int WIDTH = 500;
        int HEIGHT = 550;
        stage.setMinWidth(456.0);
        stage.setMinHeight(550.0);
        stage.setResizable(true);
        stage.setTitle("Manage Employees");

        setup_ui();

        var scene = new Scene(box, WIDTH, HEIGHT);
        scene.getStylesheets().add("manage_employees.css");
        stage.setScene(scene);
        stage.show();
    }

}