package projekt.pap;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.text.Text;
import javafx.stage.Stage;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.time.LocalDate;
import java.util.Date;

public class MakeReservation {
    private VBox box = new VBox();
    private GridPane grid = new GridPane();
    private String s_time = null;
    private String e_time = null;

    private JSONArray tableIds = new JSONArray();
    private JSONArray tableNames = new JSONArray();
    
    private void setup_ui(){
        box.setPadding(new Insets(20));
        box.setSpacing(10);
        box.setAlignment(Pos.CENTER);
        grid.setHgap(10.0);
        grid.setVgap(10.0);
        grid.setAlignment(Pos.CENTER);

        var err_label = new Label("Checking tables...");
        err_label.setAlignment(Pos.TOP_CENTER);
        err_label.setStyle("-fx-font-size: 17px");
        err_label.setVisible(false);
        box.getChildren().add(err_label);

        var label = new Label("Insert hours of reservation.");
        label.setAlignment(Pos.CENTER);
        label.setStyle("-fx-font-size: 15px");

        box.getChildren().add(label);

        var h_box = new HBox();
        h_box.setAlignment(Pos.CENTER_LEFT);
        h_box.setSpacing(10);
        var now_year = new TextField();
        var now_month = new TextField();
        var now_day = new TextField();
        var now_text = new Text("Date of reservation (DD/MM/YYYY):");

        var date = LocalDate.now();

        now_year.setText(String.valueOf(date.getYear()));
        now_month.setText(String.valueOf(date.getMonthValue()));
        now_day.setText(String.valueOf(date.getDayOfMonth()));
        now_year.setPrefWidth(80);
        now_month.setPrefWidth(50);
        now_day.setPrefWidth(50);
        grid.add(now_text, 0, 0);
        grid.add(now_day, 1, 0);
        grid.add(now_month, 2, 0);
        grid.add(now_year, 3, 0);

        var s_h_box = new HBox();
        s_h_box.setAlignment(Pos.CENTER_LEFT);
        s_h_box.setSpacing(10);
        var s_hour = new TextField();
        var s_minute = new TextField();
        var s_text = new Text("Start hour of reservation (HH:MM):");
        s_hour.setPrefWidth(95);
        s_minute.setPrefWidth(96);
        s_h_box.getChildren().addAll(s_hour, s_minute);
        grid.add(s_text, 0, 1);
        grid.add(s_h_box, 1, 1, 3, 1);

        var e_h_box = new HBox();
        e_h_box.setAlignment(Pos.CENTER_LEFT);
        e_h_box.setSpacing(10);
        var e_hour = new TextField();
        var e_minute = new TextField();
        var e_text = new Text("End hour of reservation (HH:MM):");
        e_hour.setPrefWidth(95);
        e_minute.setPrefWidth(96);
        e_h_box.getChildren().addAll(e_hour, e_minute);
        grid.add(e_text, 0, 2);
        grid.add(e_h_box, 1, 2, 3, 1);

        var e_n_box = new HBox();
        e_n_box.setAlignment(Pos.CENTER_LEFT);
        e_n_box.setSpacing(10);
        var namebox = new TextField();
        var n_text = new Text("Name:");
        namebox.setPrefWidth(201);
        e_n_box.getChildren().add(namebox);
        grid.add(n_text, 0, 3);
        grid.add(e_n_box, 1, 3, 3, 1);

        box.getChildren().add(grid);

        var go_button = new Button("Check availability");
        go_button.prefWidth(50);
        box.getChildren().add(go_button);

        var combo_h_box = new HBox();
        combo_h_box.setAlignment(Pos.CENTER);
        combo_h_box.setSpacing(10);
        combo_h_box.setVisible(false);

        var combo = new ComboBox<String>();
        var combo_button = new Button("Reserve Table");
        combo_button.setPrefWidth(200);

        combo_h_box.getChildren().addAll(combo, combo_button);
        box.getChildren().add(combo_h_box);

        go_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle (ActionEvent e){
                combo_h_box.setVisible(false);
                err_label.setVisible(false);
                HttpResponse<String>result_tables = null;
                combo.getItems().clear();

                var res_id = App.rest_id;

                if (now_year.getText().isEmpty() || now_month.getText().isEmpty() || now_day.getText().isEmpty()
                        || e_hour.getText().isEmpty() || e_minute.getText().isEmpty() || s_hour.getText().isEmpty()
                        || s_minute.getText().isEmpty()){
                    err_label.setText("All fields must be filled.");
                    err_label.setVisible(true);
                    return;
                }

                String year = now_year.getText();
                String month = now_month.getText();
                String day = now_day.getText();

                String start_hour = s_hour.getText();
                String start_minute = s_minute.getText();
                String end_hour = e_hour.getText();
                String end_minute = e_minute.getText();

                SimpleDateFormat formatter = new SimpleDateFormat("dd-mm-yyy HH:mm");

                try {
                    Date s_date = formatter.parse(day + "-" + month + "-" + year + " " + start_hour + ":" + start_minute);
                    Date e_date = formatter.parse(day + "-" + month + "-" + year + " " + end_hour + ":" + end_minute);

                    if (s_date.compareTo(e_date) >= 0){
                        err_label.setText("End of reservation must be after start.");
                        err_label.setVisible(true);
                        return;
                    }
                }
                catch (ParseException err){
                    err_label.setText("Wrong date format");
                    err_label.setVisible(true);
                    return;
                }

                String startTime = null;
                String endTime = null;

                try {
                    startTime = DateParser.parseToTimestamp(year, month, day, start_hour, start_minute, "00");
                    endTime = DateParser.parseToTimestamp(year, month, day, end_hour, end_minute, "00");
                } catch (IllegalArgumentException err) {
                    err_label.setText("All fields must be numbers of appropriate value.");
                    err_label.setVisible(true);
                    return;
                }

                try {
                    result_tables = RestaurantAPI.getFreeTables(res_id, startTime, endTime, App.token);
                } catch (IOException| InterruptedException err) {
                    err_label.setText("Unable to connect.");
                    err_label.setVisible(true);
                    return;
                }

                if(result_tables.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result_tables.body());
                        err_label.setText(status_getter.getString("message"));
                        err_label.setVisible(true);
                    } catch (JSONException err) {}
                    return;
                }

                tableIds = new JSONObject(result_tables.body()).getJSONArray("tableIds");
                tableNames = new JSONObject(result_tables.body()).getJSONArray("tableNames");

                if(tableIds.isEmpty()){
                    err_label.setText("No free tables for this time.");
                    err_label.setVisible(true);
                    return;
                }

                for (Object object : tableNames) {
                    String tableName = (String)object;
                    combo.getItems().add(tableName);
                }

                s_time = startTime;
                e_time = endTime;
                combo_h_box.setVisible(true);
            }
        });

        combo_button.setOnAction(new EventHandler<ActionEvent>(){
            @Override
            public void handle(ActionEvent e){
                err_label.setVisible(false);
                HttpResponse<String> result = null;
                int tableId = 0;

                try {
                    var val = combo.getValue();
                    for(int i = 0; i < tableNames.length(); i++) {
                        if(tableNames.get(i).equals(val)) {
                            tableId = (int)tableIds.get(i);
                            break;
                        }
                    }
                } catch (NullPointerException err) {
                    err_label.setText("ID of reserved table is not selected.");
                    err_label.setVisible(true);
                    return;
                }

                String name = namebox.getText();

                if (name.isBlank()){
                    err_label.setText("Name for reservation is empty.");
                    err_label.setVisible(true);
                    return;
                }

                try {
                    result = RestaurantAPI.addReservation(tableId, s_time, e_time, name, App.token);
                } catch (InterruptedException|IOException err) {
                    err_label.setText("Unable to connect.");
                    err_label.setVisible(true);
                    return;
                }

                if(result.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result.body());
                        err_label.setText(status_getter.getString("message"));
                        err_label.setVisible(true);
                    } catch (JSONException err) {}
                    return;
                }

                err_label.setText(String.format("Successfully booked reservation %d.", tableId));
                err_label.setVisible(true);
            }
        });
    }

    public void run() {
        var stage = new Stage();
        int WIDTH = 500;
        int HEIGHT = 300;
        stage.setResizable(true);
        stage.setMinWidth(WIDTH);
        stage.setMinHeight(HEIGHT);
        stage.setTitle("Table Manager");

        setup_ui();

        var scene = new Scene(box, WIDTH, HEIGHT);

        scene.getStylesheets().add("new_reservation.css");
        stage.setScene(scene);
        stage.show();
    }
}
