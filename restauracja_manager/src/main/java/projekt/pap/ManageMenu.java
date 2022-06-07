package projekt.pap;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Pos;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.text.Text;
import javafx.stage.Stage;
import javafx.stage.WindowEvent;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;
import java.util.ArrayList;
import java.util.List;


public class ManageMenu {
    private boolean isDishMenuOpen = false;
    private int menuID;
    private VBox box = new VBox();
    private VBox dish_box;
    private VBox wrapper_box;
    private TableView<DishRecord>table;
    private Label err_label;
    private Label dish_label;
    private List<Integer> ing_ids = new ArrayList<Integer>();
    private List<String> ing_names = new ArrayList<String>();

    private void setup_dish_ui(){
        wrapper_box = new VBox();
        dish_box = new VBox();

        wrapper_box.setId("vbox");
        dish_box.setId("vbox");

        dish_label = new Label("Add new dish");
        dish_label.setId("dish_label");

        var h_name_box = new HBox();
        h_name_box.setSpacing(10);
        h_name_box.setAlignment(Pos.CENTER);
        var name_text = new Text("Name:");
        var name_field = new TextField();
        name_field.setPromptText("name of new dish");

        h_name_box.getChildren().addAll(name_text, name_field);

        var h_price_box = new HBox();
        h_price_box.setSpacing(10);
        h_price_box.setAlignment(Pos.CENTER);
        var price_text = new Text("Price (zł):");
        var price_field = new TextField();
        price_field.setPromptText("price (zł) of new dish");

        h_price_box.getChildren().addAll(price_text, price_field);

        updateIngredientList();
        addNewRow();

        var h_box = new HBox();
        h_box.setSpacing(10);
        h_box.setAlignment(Pos.CENTER);

        var new_row_button = new Button("Add Ingredient");
        var complete_button = new Button("Complete");

        h_box.getChildren().addAll(new_row_button, complete_button);
        wrapper_box.getChildren().addAll(dish_label, h_name_box, h_price_box, dish_box, h_box);

        new_row_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                addNewRow();
            }
        });

        complete_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                var selected_names = new ArrayList<String>();
                var selected_amounts = new ArrayList<Integer>();
                var selected_ids = new ArrayList<Integer>();

                String name = name_field.getText().strip();
                var price_txt = price_field.getText().strip();
                if(name.isBlank() || price_txt.isBlank()){
                    dish_label.setText("Name or price of the dish is empty.");
                    return;
                }

                int price = -1;
                try {
                    price = Integer.valueOf(price_txt) * 100;
                    if(price < 0){
                        dish_label.setText("Price must be positive number.");
                        return;
                    }
                } catch (IllegalArgumentException err) {
                    dish_label.setText("Price must be a number.");
                    return;
                }
                var json_ingredients = new JSONArray();

                if(dish_box.getChildren().size() == 0){
                    dish_label.setText("Dish must consist of ingredients.");
                    return;
                }

                for (Node node : dish_box.getChildren()) {
                    if(node instanceof HBox){
                        var h_box = (HBox) node;
                        if(h_box.getChildren().get(0) instanceof ComboBox<?>){
                            var combo_box = (ComboBox<?>) h_box.getChildren().get(0);
                            var ing_name = (String) combo_box.getSelectionModel().getSelectedItem();
                            if(selected_names.contains(ing_name)){
                                dish_label.setText("Selected same ingredient more than 1 time.");
                                return;
                            }
                            selected_names.add(ing_name);

                            int idx = ing_names.indexOf(ing_name);
                            selected_ids.add(ing_ids.get(idx));
                        }

                        if(h_box.getChildren().get(1) instanceof TextField){
                            var amount_field = (TextField) h_box.getChildren().get(1);
                            var amount_txt = amount_field.getText().strip();
                            if(amount_txt.isBlank()){
                                dish_label.setText("Amount (gram) is left empty.");
                                return;
                            }
                            try {
                                var amount = Integer.valueOf(amount_txt);
                                if(amount <= 0){
                                    dish_label.setText("Amount (gram) must be greater than 0.");
                                    return;
                                }
                                selected_amounts.add(amount);
                            } catch (NumberFormatException err) {
                                dish_label.setText("Amount (gram) must be a number.");
                                return;
                            }
                        }
                    }
                }

                for (int i = 0; i < selected_names.size(); i++){
                    var json_obj = new JSONObject();
                    json_obj.put("ingredientId", selected_ids.get(i));
                    json_obj.put("amount", selected_amounts.get(i));
                    json_ingredients.put(json_obj);
                }

                HttpResponse<String> result_dish = null;

                try {
                    result_dish = RestaurantAPI.addDish(name, price, menuID, json_ingredients, App.token);
                    updateTable();
                    if(result_dish.statusCode() != 200){
                        var status_getter = new JSONObject(result_dish.body());
                        dish_label.setText(status_getter.getString("message"));
                        return;
                    } else {
                        dish_label.setText("Successfully added new dish to menu.");
                        return;
                    }
                } catch (IOException|InterruptedException err) {
                    dish_label.setText("Cannot connect.");
                    return;
                } catch (JSONException err){
                    System.out.println(result_dish.body());
                    dish_label.setText("Critical backend error. Result not JSON compatible.");
                    return;
                }
            }
        });
    }

    private void addItemsToComboBox(ComboBox<String> combo){
        combo.getItems().clear();
        combo.getItems().addAll(ing_names);
    }

    private void updateIngredientList(){
        var ingredients = new JSONArray();
        try {
            var result_res = RestaurantAPI.getRestaurant(App.rest_id, App.token);
            if(result_res.statusCode() != 200){
                var status_getter = new JSONObject(result_res.body());
                dish_label.setText(status_getter.getString("message"));
                return;
            }
            ingredients = new JSONObject(result_res.body()).getJSONArray("restaurantIngredients");
            menuID = new JSONObject(result_res.body()).getJSONObject("menu").getInt("id");
        } catch (JSONException e) {
            dish_label.setText("Critical backend error. Result not JSON compatible.");
            return;
        } catch (IOException|InterruptedException e){
            dish_label.setText("Cannot connect.");
            return;
        }

        if(ingredients.length() == 0){
            dish_label.setText("No ingredients registered in this restaurant. Add in View Ingredients menu.");
            return;
        }

        for (Object object : ingredients) {
            var ing_obj = new JSONObject(object.toString());
            int id = ing_obj.getInt("ingredientId");
            ing_ids.add(id);
            String name;
            try {
                var result_ing = RestaurantAPI.getIngredient(id, App.token);
                var ing_json = new JSONObject(result_ing.body());
                name = ing_json.getString("name");

            } catch (IOException|InterruptedException e) {
                dish_label.setText("Cannot connect.");
                return;
            } catch (JSONException e){
                dish_label.setText("Critical backend error. Result not JSON compatible.");
                return;
            }
            ing_names.add(name);
        }
    }

    private void addNewRow(){
        var h_box = new HBox();
        h_box.setSpacing(10);
        h_box.setAlignment(Pos.CENTER);

        var numberField = new TextField();
        numberField.setPromptText("amount in grams");

        var comboBox = new ComboBox<String>();
        addItemsToComboBox(comboBox);
        var del_button = new Button("X");

        h_box.getChildren().addAll(comboBox, numberField, del_button);
        dish_box.getChildren().add(h_box);

        del_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                dish_box.getChildren().remove(h_box);
            }
        });
    }

    private void updateTable(){
        table.getItems().clear();
        err_label.setText("Manage Menu");
        int res_id = App.rest_id;

        HttpResponse<String>result_res = null;

        try {
            result_res = RestaurantAPI.getRestaurant(res_id, App.token);
        } catch (IOException|InterruptedException err) {
            err_label.setText("Unable to connect.");
            return;
        }

        if(result_res.statusCode() != 200){
            try {
                var status_getter = new JSONObject(result_res.body());
                err_label.setText(status_getter.getString("message"));
            } catch (JSONException err) {}
            return;
        }

        var dishes = new JSONObject(result_res.body()).getJSONObject("menu").getJSONArray("dishes");
        menuID = new JSONObject(result_res.body()).getJSONObject("menu").getInt("id");

        for (Object object : dishes) {
            var dish_obj = new JSONObject(object.toString());
            table.getItems().add(new DishRecord(dish_obj.getInt("id"),
                dish_obj.getString("name"), dish_obj.getInt("price")));
        }
    }

    private void setup_ui(){
        box.setId("vbox");

        err_label = new Label("Manage Menu");
        err_label.setId("dish_label");
        box.getChildren().add(err_label);

        table = new TableView<DishRecord>();
        table.setPrefSize(400, 300);
        table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
        table.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);

        var name_col = new TableColumn<DishRecord, String>("name");
        name_col.setCellValueFactory(new PropertyValueFactory<>("name"));
        var price_col = new TableColumn<DishRecord, String>("price (zł)");
        price_col.setCellValueFactory(new PropertyValueFactory<>("price"));
        var id_col = new TableColumn<DishRecord, String>("ID");
        id_col.setCellValueFactory(new PropertyValueFactory<>("id"));
        id_col.setResizable(false);
        table.getColumns().addAll(id_col, name_col, price_col);
        updateTable();
        box.getChildren().add(table);

        var h_bottom_box = new HBox();
        h_bottom_box.setSpacing(15);
        h_bottom_box.setAlignment(Pos.CENTER);

        var dish_button = new Button("New Dish");
        var delete_button = new Button("Delete Dish");
        h_bottom_box.getChildren().addAll(delete_button, dish_button);
        box.getChildren().add(h_bottom_box);

        dish_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                if(!isDishMenuOpen){
                    isDishMenuOpen = true;
                    var stage = new Stage();
                    int WIDTH = 400;
                    int HEIGHT = 300;
                    stage.setResizable(true);
                    stage.setMinWidth(WIDTH);
                    stage.setMinHeight(HEIGHT);
                    stage.setTitle("Add New Dish");

                    setup_dish_ui();

                    var scene = new Scene(wrapper_box, WIDTH, HEIGHT);
                    scene.getStylesheets().add("new_dish.css");
                    stage.setScene(scene);
                    stage.show();

                    stage.setOnCloseRequest(new EventHandler<WindowEvent>() {
                        public void handle(WindowEvent we) {
                            isDishMenuOpen = false;
                        }
                    });
                }
            }
        });

        delete_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                var selected = table.getSelectionModel().getSelectedItems();
                HttpResponse<String> result_delete = null;

                try {
                    var dishRecord = selected.get(0);
                    result_delete = RestaurantAPI.deleteDish(Integer.valueOf(dishRecord.getId()), App.token);
                    if(result_delete.statusCode() != 200){
                        var status_getter = new JSONObject(result_delete.body());
                        err_label.setText(status_getter.getString("message"));
                        return;
                    } else {
                        err_label.setText("Employee(s) successfully deleted.");
                        updateTable();
                    }
                } catch (IOException | InterruptedException err) {
                    err_label.setText("Unable to connect.");
                    return;
                } catch (JSONException err) {
                    System.out.println(result_delete.body());
                    err_label.setText("Critical error. Result not JSON compatible.");
                    return;
                }
            }
        });
    }

    public void run(){
        var stage = new Stage();
        int WIDTH = 400;
        int HEIGHT = 450;
        stage.setResizable(true);
        stage.setMinHeight(HEIGHT);
        stage.setMinWidth(WIDTH);
        stage.setTitle("Manage Menu");

        setup_ui();

        var scene = new Scene(box, WIDTH, HEIGHT);
        scene.getStylesheets().add("manage_menu.css");
        stage.setScene(scene);
        stage.show();
    }

}