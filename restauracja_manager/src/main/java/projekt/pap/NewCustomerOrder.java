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


public class NewCustomerOrder {
    private VBox box = new VBox();

    private void setup_ui(){
        box.setId("vbox");

        var err_label = new Label("Cheking menu...");
        err_label.setVisible(false);
        err_label.setId("err_label");
        box.getChildren().add(err_label);

        var table = new TableView<DishRecord>();
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

        int res_id = App.rest_id;

        HttpResponse<String>result_res = null;

        try {
            result_res = RestaurantAPI.getRestaurant(res_id, App.token);
        } catch (IOException|InterruptedException err) {
            err_label.setText("Unable to connect.");
            err_label.setVisible(true);
            return;
        }

        if(result_res.statusCode() != 200){
            try {
                var status_getter = new JSONObject(result_res.body());
                err_label.setText(status_getter.getString("message"));
                err_label.setVisible(true);
            } catch (JSONException err) {}
            return;
        }

        var dishes = new JSONObject(result_res.body()).getJSONObject("menu").getJSONArray("dishes");

        for (Object object : dishes) {
            var dish_obj = new JSONObject(object.toString());
            table.getItems().add(new DishRecord(dish_obj.getInt("id"),
                    dish_obj.getString("name"), dish_obj.getInt("price")));
        }

        box.getChildren().add(table);

        var go_button = new Button("Make order from selected");

        var h_box = new HBox();
        h_box.setSpacing(20);
        h_box.setPadding(new Insets(10));
        h_box.setAlignment(Pos.CENTER);
        var takeout_box = new CheckBox("Takeout?");
        takeout_box.setId("takeout_checkbox");

        go_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                err_label.setVisible(false);
                var selected = table.getSelectionModel().getSelectedItems();
                if(selected.size() == 0){
                    err_label.setText("No dishes selected.");
                    err_label.setVisible(true);
                    return;
                }

                int totalPrice = 0;

                var dish_list = new JSONArray();
                for (DishRecord dish : selected) {
                    dish_list.put(new JSONObject().put("dishId", dish.getId()).put("count", 1));
                    totalPrice += dish.getPriceReal();
                }

                var time = DateParser.parseNow();

                HttpResponse<String> result_order = null;

                try {
                    result_order = RestaurantAPI.addOrder(takeout_box.selectedProperty().get(), time, totalPrice,
                            App.employee_id, res_id, dish_list, App.token);
                } catch (IOException|InterruptedException err) {
                    err_label.setText("Unable to add order to database.");
                    err_label.setVisible(true);
                    return;
                }

                if (result_order.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result_order.body());
                        err_label.setText(status_getter.getString("message"));
                        err_label.setVisible(true);
                    } catch (JSONException err) {
                        err_label.setText("Unable to add order.");
                        err_label.setVisible(true);
                    }
                    return;
                }
                else{
                    err_label.setText("Successfully made an order.");
                    err_label.setVisible(true);
                }
            }
        });

        h_box.getChildren().addAll(go_button, takeout_box);
        box.getChildren().add(h_box);
    }

    public void run(){
        var stage = new Stage();
        int WIDTH = 400;
        int HEIGHT = 450;
        stage.setMinHeight(HEIGHT);
        stage.setMinWidth(WIDTH);
        stage.setResizable(true);
        stage.setTitle("Add Order From Customer");

        setup_ui();

        var scene = new Scene(box, WIDTH, HEIGHT);
        scene.getStylesheets().add("new_order.css");
        stage.setScene(scene);
        stage.show();
    }

}