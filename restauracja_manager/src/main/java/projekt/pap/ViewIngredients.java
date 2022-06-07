package projekt.pap;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;

public class ViewIngredients {
    private VBox box = new VBox();
    private TableView<IngredientRecord> table;
    private Label err_label;

    private void update_table(){
        table.getItems().clear();
        HttpResponse<String>result_res;
        JSONArray ingredients;
        try {
            result_res = RestaurantAPI.getRestaurant(App.rest_id, App.token);
            if(result_res.statusCode() != 200){
                var status_getter = new JSONObject(result_res.body());
                err_label.setText(status_getter.getString("message"));
                return;
            }
            ingredients = new JSONObject(result_res.body()).getJSONArray("restaurantIngredients");
        } catch (JSONException e) {
            err_label.setText("Critical backend error. Result not JSON compatible.");
            return;
        } catch (IOException|InterruptedException e){
            err_label.setText("Cannot connect.");
            return;
        }

        if (ingredients.length() == 0) {
            err_label.setText("No ingredients in this restaurant.");
            return;
        }

        for (Object object : ingredients) {
            var ing_obj = new JSONObject(object.toString());
            int id = ing_obj.getInt("ingredientId");
            int amount = ing_obj.getInt("amount");
            String name;
            int price;
            boolean isPricePerKG;

            try {
                var result_ing = RestaurantAPI.getIngredient(id, App.token);
                var ing_json = new JSONObject(result_ing.body());
                name = ing_json.getString("name");
                price = ing_json.getInt("price");
                isPricePerKG = ing_json.getBoolean("isPricePerKilogram");

                var ingredient = new IngredientRecord(id, price,
                    amount, isPricePerKG, name);
                table.getItems().add(ingredient);
            } catch (IOException|InterruptedException err) {
                err_label.setText("Cannot connect.");
                return;
            } catch (JSONException err){
                err_label.setText("Critical backend error. Result not JSON compatible.");
                return;
            }
        }
    }

    private void setup_ui(){
        box.setId("vbox");

        err_label = new Label("View ingredients in your restaurant.");
        err_label.setId("err_label");
        box.getChildren().add(err_label);

        table = new TableView<IngredientRecord>();
        table.setPrefSize(300, 300);
        table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
        table.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);

        var id_col = new TableColumn<IngredientRecord, String>("ID");
        id_col.setCellValueFactory(new PropertyValueFactory<>("id"));
        id_col.setResizable(false);
        var name_col = new TableColumn<IngredientRecord, String>("name");
        name_col.setCellValueFactory(new PropertyValueFactory<>("name"));
        var amount_col = new TableColumn<IngredientRecord, String>("amount (kg)");
        amount_col.setCellValueFactory(new PropertyValueFactory<>("amount"));
        var price_col = new TableColumn<IngredientRecord, String>("price (zł)");
        price_col.setCellValueFactory(new PropertyValueFactory<>("price"));
        var ppKG_col = new TableColumn<IngredientRecord, String>("price per kilogram?");
        ppKG_col.setCellValueFactory(new PropertyValueFactory<>("isPricePerKilogram"));
        table.getColumns().addAll(id_col, name_col, amount_col, price_col, ppKG_col);

        update_table();

        box.getChildren().add(table);

        var new_ingr_label = new Label("Name - Price (zł) - Price per kilogram");
        new_ingr_label.setAlignment(Pos.CENTER);
        box.getChildren().add(new_ingr_label);

        var h_box = new HBox();
        h_box.setSpacing(10);
        h_box.setAlignment(Pos.CENTER);
        h_box.setPadding(new Insets(10));

        var name_field = new TextField();
        name_field.setPromptText("Name");
        var price_field = new TextField();
        price_field.setPromptText("Price (zł)");
        var new_ingr_box = new CheckBox("Price per kilogram?");
        new_ingr_box.setId("new_ingr_box");

        h_box.getChildren().addAll(name_field, price_field, new_ingr_box);
        box.getChildren().add(h_box);

        var new_ingr_button = new Button("Add");
        new_ingr_button.setPrefWidth(50);
        box.getChildren().add(new_ingr_button);

        new_ingr_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                new_ingr_label.setText("Name - Price (zł) - Price per kilogram");
                var name = name_field.getText().strip();

                if(name.isBlank()){
                    new_ingr_label.setText("Name field is empty");
                    return;
                }

                var price_txt = price_field.getText().strip();

                if(price_txt.isBlank()){
                    new_ingr_label.setText("Price field is empty");
                    return;
                }

                int price = -1;

                try {
                    price = Integer.valueOf(price_txt) * 100;
                } catch (NumberFormatException err) {
                    new_ingr_label.setText("Price (zł) should be a number");
                    return;
                }

                HttpResponse<String>result_add;

                try {
                    result_add = RestaurantAPI.addIngredient(name, price,
                        new_ingr_box.isSelected(), App.token);
                } catch (InterruptedException|IOException err) {
                    new_ingr_label.setText("Connection error");
                    return;
                }

                if(result_add.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result_add.body());
                        new_ingr_label.setText(status_getter.getString("message"));
                        new_ingr_label.setVisible(true);
                    } catch (JSONException err) {
                        System.out.println(result_add.body());
                        new_ingr_label.setText("Critical backend error");
                        new_ingr_label.setVisible(true);
                    }
                    return;
                }

                try {
                    result_add = RestaurantAPI.addIngredient(name, price,
                        new_ingr_box.isSelected(), App.token);
                } catch (InterruptedException|IOException err) {
                    new_ingr_label.setText("Connection error");
                    return;
                }

                if(result_add.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result_add.body());
                        new_ingr_label.setText(status_getter.getString("message"));
                        new_ingr_label.setVisible(true);
                    } catch (JSONException err) {
                        System.out.println(result_add.body());
                        new_ingr_label.setText("Critical backend error");
                        new_ingr_label.setVisible(true);
                    }
                    return;
                }

                int new_id = new JSONObject(result_add.body()).getInt("id");
                HttpResponse<String>result_rest;

                try {
                    result_rest = RestaurantAPI.addRestaurantIngredient(App.rest_id, new_id, 0, App.token);
                } catch (InterruptedException|IOException err) {
                    new_ingr_label.setText("Connection error");
                    return;
                }

                if(result_rest.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result_rest.body());
                        new_ingr_label.setText(status_getter.getString("message"));
                        new_ingr_label.setVisible(true);
                    } catch (JSONException err) {
                        System.out.println(result_rest.body());
                        new_ingr_label.setText("Critical backend error");
                        new_ingr_label.setVisible(true);
                    }
                    return;
                }

                new_ingr_label.setText("Successfully added new ingredient.");
                update_table();
            }
        });

    }

    public void run(){
        var stage = new Stage();
        int WIDTH = 600;
        int HEIGHT = 500;
        stage.setMinWidth(WIDTH);
        stage.setMinHeight(HEIGHT);
        stage.setResizable(true);
        stage.setTitle("View Ingredients");

        setup_ui();

        var scene = new Scene(box, WIDTH, HEIGHT);
        scene.getStylesheets().add("view_ingredients.css");
        stage.setScene(scene);
        stage.show();
    }

}