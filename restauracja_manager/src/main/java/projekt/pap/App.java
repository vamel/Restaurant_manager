package projekt.pap;

import javafx.application.Application;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.layout.*;
import javafx.scene.shape.Rectangle;
import javafx.stage.Stage;
import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.net.http.HttpResponse;
import java.util.ArrayList;
import java.util.concurrent.ThreadLocalRandom;


public class App extends Application {
    // test credentials:
    // login: mario
    // password: kart
    private boolean isMenu = false;
    private Stage stage;
    private VBox vert_box;
    private VBox vert_box2;
    private HBox hor_box;
    private StackPane stackpane;
    public static String token;
    public static int employee_id;
    public static int rest_id;

    @Override
    public void start(Stage primaryStage) throws Exception{
        stage = primaryStage;
        stage.setResizable(false);
        stage.setTitle("Restaurant Manager");
        stage.getIcons().add(new Image("file:src/main/resources/graphics/app_icon/kevin.png"));
        hor_box = new HBox();
        vert_box = new VBox();
        vert_box2 = new VBox();
        stackpane = new StackPane();
        login();
        Scene scene = new Scene(stackpane);
        scene.getStylesheets().add("main.css");
        stage.setScene(scene);
        stage.show();
    }

    private String generate_welcome_prompt()
    {
        String[] prompts = {"Welcome ", "It's nice to see you ", "Hello ", "Let's work together "};
        int randomNum = ThreadLocalRandom.current().nextInt(0, prompts.length);
        return prompts[randomNum];
    }

    private void configure_fields(Label label_title, TextField login_field, PasswordField password_field,
                                  TextField shown_password_field, Button next_button,
                                  CheckBox show_password_checkbox, Rectangle rectangle) {

        rectangle.setId("first_rectangle");
        next_button.setId("login_button");
        label_title.setId("label_title");
        login_field.setPromptText("login");
        show_password_checkbox.setId("login_checkbox");

        password_field.setPromptText("password");
        password_field.managedProperty().bind(show_password_checkbox.selectedProperty().not());
        password_field.visibleProperty().bind(show_password_checkbox.selectedProperty().not());

        shown_password_field.setPromptText("password");
        shown_password_field.setManaged(false);
        shown_password_field.setVisible(false);
        shown_password_field.managedProperty().bind(show_password_checkbox.selectedProperty());
        shown_password_field.visibleProperty().bind(show_password_checkbox.selectedProperty());
        shown_password_field.textProperty().bindBidirectional(password_field.textProperty());
    }

    private void login(){
        isMenu = false;
        vert_box.setPadding(new Insets(10));
        vert_box.setSpacing(10);
        vert_box.setAlignment(Pos.CENTER);
        hor_box.setPadding(new Insets(0, 0, 0, 240));
        hor_box.setSpacing(5);
        hor_box.setAlignment(Pos.CENTER_LEFT);

        var label_title = new Label("Restaurant Manager");
        var label_checkbox = new Label("show password");
        var login_field = new TextField();
        var password_field = new PasswordField();
        var shown_password_field = new TextField();
        var show_password_checkbox = new CheckBox();
        var next_button = new Button("Login");
        var rectangle = new Rectangle(400, 240);
        var combo = new ComboBox<String>();
        combo.setVisible(false);
        configure_fields(label_title, login_field, password_field, shown_password_field,
                next_button, show_password_checkbox, rectangle);

        hor_box.getChildren().addAll(show_password_checkbox, label_checkbox);

        vert_box.getChildren().addAll(label_title, login_field, password_field, shown_password_field, hor_box, next_button, combo);

        stackpane.setId("background");
        stackpane.getChildren().addAll(rectangle, vert_box);

        next_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e){
                if (!isMenu) {
                    String login = login_field.getText();
                    String password = password_field.getText();
                    combo.getItems().clear(); // just for safety

                    if (login.isBlank() || password.isBlank())
                    {
                        String empty_prompt = "Login or password is empty!";
                        label_title.setText(empty_prompt);
                        return;
                    }

                    HttpResponse<String> result = null;

                    try{
                        result = RestaurantAPI.login(login, password);
                    }
                    catch(IOException |InterruptedException err){
                        System.out.println(err);

                        String err_prompt = "Failed to login. Connection error.";
                        label_title.setText(err_prompt);

                        return;
                    }

                    if (result.statusCode() != 200){
                        String err_prompt = "Failed to login. User not found. Try again.";
                        label_title.setText(err_prompt);

                        return;
                    }

                    var login_getter = new JSONObject(result.body());
                    token = login_getter.getString("token");
                    employee_id = login_getter.getInt("employeeId");

                    HttpResponse<String> result_emp = null;
                    try{
                        result_emp = RestaurantAPI.getEmployee(employee_id, token);
                    }
                    catch(IOException |InterruptedException err){
                        System.out.println(err);

                        String err_prompt = "Failed to login. Connection error.";
                        label_title.setText(err_prompt);

                        return;
                    }

                    if (result_emp.statusCode() != 200) {
                        String err_prompt = "Failed to login. Token expired";
                        label_title.setText(err_prompt);
                        return;
                    }

                    var emp_getter = new JSONObject(result_emp.body());
                    rest_id = emp_getter.getInt("restaurantId");

                    if (rest_id == -1){
                        HttpResponse<String> result_rest = null;
                        try{
                            result_rest = RestaurantAPI.getOwnedRestaurants(employee_id, token);
                        }
                        catch(IOException |InterruptedException err){
                            System.out.println(err);
                            String err_prompt = "Failed to login. No owned restaurants.";
                            label_title.setText(err_prompt);
                            return;
                        }

                        if (result_rest.statusCode() != 200) {
                            String err_prompt = "No owned restaurants.";
                            label_title.setText(err_prompt);
                            return;
                        }

                        var rest_arr = new JSONArray(result_rest.body());
                        var rest_id_arr = new ArrayList<Integer>();

                        if (rest_arr.length() == 0){
                            String err_prompt = "No owned or employing restaurants.";
                            label_title.setText(err_prompt);
                            return;
                        }

                        for (Object object : rest_arr) {
                            var rest_obj = new JSONObject(object.toString());
                            combo.getItems().add(rest_obj.getString("name"));
                            rest_id_arr.add(rest_obj.getInt("id"));
                        }

                        combo.setVisible(true);
                        isMenu = true;

                        combo.valueProperty().addListener(new ChangeListener<String>() {
                            @Override
                            public void changed(ObservableValue observable, String oldValue, String newValue){

                            rest_id = rest_id_arr.get(combo.getSelectionModel().getSelectedIndex());
                                // rest_id = -1;

                                hor_box.getChildren().clear();
                                vert_box.getChildren().clear();
                                stackpane.getChildren().clear();

                                setup_menu(login);
                            }
                        });
                    }
                    else{
                        hor_box.getChildren().clear();
                        vert_box.getChildren().clear();
                        stackpane.getChildren().clear();

                        setup_menu(login);
                    }
                }
            }
        });
    }

    private void setup_menu(String login){
        isMenu = true;
        vert_box2.setAlignment(Pos.TOP_CENTER);
        var rectangle = new Rectangle(400, 450);
        rectangle.setId("second_rectangle");

        var label_title = new Label("Menu");
        label_title.setId("label_welcome");
        String welcome_prompt = generate_welcome_prompt();
        label_title.setText(welcome_prompt + login + "!");
        vert_box2.getChildren().add(label_title);

        var spacer = new Region();
        spacer.setPrefHeight(37.5);
        vert_box2.getChildren().add(spacer);

        var grid = new GridPane();
        grid.setPadding(new Insets(30));
        grid.setHgap(10);
        grid.setVgap(10);
        grid.setAlignment(Pos.BOTTOM_CENTER);

        int i = 0;

        HttpResponse<String> role = null;
        try {
            role = RestaurantAPI.getEmployeeRole(employee_id, token);
        } catch (IOException | InterruptedException err) {
            System.out.println(err);    // is not really possible, print for detecting extreme errors
        }

        int rights_id = RightsManager.getRightsByRole(role.body());

        // rights_id = 1; // used when skipping login

        var func_manager = new FunctionManager();
        var func_list = func_manager.getFunctions(rights_id);

        for (f_interface function_obj : func_list) {
            var temp_button = new Button(function_obj.getButtonName());

            temp_button.setOnAction(function_obj.getHandler());
            temp_button.setId("func_button");
            grid.add(temp_button, i%3, i/3);
            i++;
        }

        vert_box2.getChildren().add(grid);

        var logout_button = new Button("logout");
        logout_button.setId("logout_button");
        logout_button.setAlignment(Pos.CENTER);

        logout_button.setOnAction(new EventHandler<ActionEvent>() {
            @Override
            public void handle(ActionEvent e){
                App.token = null;
                App.employee_id = -1;
                App.rest_id = -2; // -1 is not ok here

                grid.getChildren().clear();
                vert_box2.getChildren().clear();
                stackpane.getChildren().clear();
                login();
            }
        });

        vert_box2.getChildren().add(logout_button);
        stackpane.getChildren().addAll(rectangle, vert_box2);
    }

    public static void main(String[] args) {
        launch(args);
    }
}
