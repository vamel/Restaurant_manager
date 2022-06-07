package projekt.pap;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.control.Tooltip;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.text.Text;
import javafx.stage.Stage;
import javafx.util.Duration;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;
import java.util.ArrayList;
import java.util.List;


public class NewEmployeeWindow{
    private VBox box = new VBox();

    public void run(){
        var stage = new Stage();
        stage.setResizable(true);
        stage.setMinWidth(350.0);
        stage.setMinHeight(500.0);
        stage.setTitle("Register New Employee");

        setup_stage();

        var scene = new Scene(box);
        scene.getStylesheets().add("new_employee.css");
        stage.setScene(scene);
        stage.show();
    }

    private void setup_stage(){
        box.setId("vbox");

        var label = new Label("PASS DATA OF NEW EMPLOYEE.");
        label.setAlignment(Pos.CENTER);
        box.getChildren().add(label);

        List<String> texts_str = List.of("USERNAME", "NAME", "SURNAME",
        "PASSWORD", "PESEL (optional)", "BIRTH DATE (DD/MM/YYYY)");
        // order is same as in API

        ArrayList<TextField> fields = new ArrayList<TextField>(texts_str.size());
        ArrayList<Label> labels = new ArrayList<Label>(texts_str.size());

        for (int i = 0; i < texts_str.size() - 1; i++) {
            fields.add(new TextField());
            labels.add(new Label());

            fields.get(i).setAlignment(Pos.CENTER);
            labels.get(i).setId("label_text");
            labels.get(i).setText(texts_str.get(i));

            box.getChildren().addAll(labels.get(i), fields.get(i));
        }

        var password_tooltip = new Tooltip("Password should be at least 4 characters long.");
        var stop = new Image(getClass().getResourceAsStream("/graphics/tooltip_icons/stop.gif"));
        password_tooltip.setShowDelay(Duration.ZERO);
        password_tooltip.setGraphic(new ImageView(stop));
        fields.get(3).setTooltip(password_tooltip);
        var date_text = new Text();
        date_text.setText(texts_str.get(texts_str.size() - 1));

        box.getChildren().add(date_text);

        var h_box = new HBox();
        h_box.setAlignment(Pos.CENTER);
        h_box.setSpacing(20);

        TextField year_field = new TextField();
        TextField month_field = new TextField();
        TextField day_field = new TextField();

        year_field.setPromptText("YYYY");
        month_field.setPromptText("MM");
        day_field.setPromptText("DD");

        year_field.setPrefWidth(100);
        day_field.setPrefWidth(50);
        month_field.setPrefWidth(50);

        h_box.getChildren().addAll(day_field, month_field, year_field);
        box.getChildren().add(h_box);

        var exec_button = new Button("Register");
        exec_button.setAlignment(Pos.BOTTOM_CENTER);

        box.getChildren().add(exec_button);

        exec_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){

                boolean condition = true;

                for (int i = 0; i < fields.size() - 2; i++){
                    if (fields.get(i).getText().isBlank()) condition = false;
                }

                if (fields.get(3).getText().length() < 4)
                {
                    label.setText("Password should be at least 4 characters long.");
                    return;
                }
                else if (fields.get(4).getText().length() != 11 && fields.get(4).getText().length() != 0)
                {
                    label.setText("PESEL must have 11 numbers.");
                    return;
                }

                else if (!condition || (year_field.getText().isEmpty() ||
                    month_field.getText().isEmpty() || day_field.getText().isEmpty())){
                    label.setText("All fields apart from PESEL must be filled.");
                    return;
                }

                HttpResponse<String>result = null;
                String date_str = null;

                try {
                    date_str = DateParser.parseDateToTimestamp(year_field.getText(),
                    month_field.getText(), day_field.getText());
                } catch (IllegalArgumentException err) {
                    label.setText("Wrong date format. Should be DD/MM/YYYY");
                    return;
                }

                int res_id = App.rest_id;
                String pesel = null;

                if (!fields.get(4).getText().isEmpty()){
                    pesel = fields.get(4).getText();
                }

                try {
                    result = RestaurantAPI.registerEmployee(fields.get(0).getText(), fields.get(1).getText(),
                    fields.get(2).getText(), fields.get(3).getText(), res_id,
                    pesel,  date_str, App.token);
                } catch (IOException | InterruptedException err) {
                    System.out.println(err);
                    label.setText("Unable to connect.");
                    return;
                }

                if (result.statusCode() != 200){
                    try {
                        var status_getter = new JSONObject(result.body());
                        label.setText(status_getter.getString("message"));
                    } catch (JSONException err) {
                        label.setText("Unable to get result. No rights to add employee.");
                    }
                    return;
                }

                label.setText(String.format("Employee %s succesfully added.",
                fields.get(0).getText()));
            }
        });
    }
}
